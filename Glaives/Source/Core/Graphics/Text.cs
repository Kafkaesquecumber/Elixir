﻿// MIT License
// 
// Copyright(c) 2018 Glaives Game Engine.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Glaives.Core.Internal.Graphics;
using SharpFont;

namespace Glaives.Core.Graphics
{
    /// <inheritdoc />
    /// The text drawable actor generates quads that represents characters from a font
    public class Text : DrawableActor
    {
        private struct LineInfo
        {
            internal float X;
            internal FloatRect LastCharRect;
            internal int VerticesIndex;
            internal int VerticesLength;
            internal float UnderlineY;
            internal float StrikeoutY;
            internal float LineWidth;
            internal float UnderlineThickness;
            internal float StrikeoutThickness;
        }

        private string _string;
        /// <summary>
        /// The string to be drawn by the text actor
        /// </summary>
        public string String
        {
            get => _string;
            set
            {
                if (_string != value)
                {
                    _string = value;
                    ReconstructVertices();
                }
            }
        }

        private TextStyleFlags _styleFlags;
        /// <summary>
        /// The styling applied to the text (flags)
        /// </summary>
        public TextStyleFlags StyleFlags
        {
            get => _styleFlags;
            set
            {
                if (_styleFlags != value)
                {
                    _styleFlags = value;
                    ReconstructVertices();
                }
            }
        }

        private TextAlignment _alignment;
        /// <summary>
        /// The <see cref="TextAlignment"/> anchors the lines to the left, center or right side of the text bounds
        /// </summary>
        public TextAlignment Alignment
        {
            get => _alignment;
            set
            {
                if (_alignment != value)
                {
                    _alignment = value;
                    ReconstructVertices();
                }
            }
        }

        private Font _font;
        /// <summary>
        /// The <see cref="Font"/> used to draw the text
        /// </summary>
        public Font Font
        {
            get => _font;
            set
            {
                if (_font != value)
                {
                    _font = value;
                    ReconstructVertices();
                }
            }
        }

        public Text(Font font, string text)
            : this(font, text, TextStyleFlags.Regular, TextAlignment.Left)
        {
        }

        public Text(Font font, string text, TextStyleFlags styleFlags)
            : this(font, text, styleFlags, TextAlignment.Left)
        {
            
        }

        public Text(Font font, string text, TextStyleFlags styleFlags, TextAlignment alignment)
        {
            StyleFlags = styleFlags;
            Alignment = alignment;
            Font = font;
            String = text;
            Texture = font.Texture;

            // Override the origin to be top-left (text center is very volatile)
            Origin = Vector2.Zero;

            // Cache the vertices immediately so the bounds are valid after creation
            ConstructVertices();
        }
        
        private FloatRect _localBounds;
        public override FloatRect LocalBounds => _localBounds;

        protected override void FillVertexArray(ref Vertex[] vertices)
        {
            if (string.IsNullOrEmpty(String))
            {
                vertices = new Vertex[0];
                _localBounds = FloatRect.Zero;
                return;
            }

            int underline = StyleFlags.HasFlag(TextStyleFlags.Underline) ? 1 : 0;
            int strikeout = StyleFlags.HasFlag(TextStyleFlags.Strikeout) ? 1 : 0;

            int totalLines = String.Count(x => x == '\n') + 1;
            // Grow the vertex array to fit the underlines and strikeout quads
            int styleVerticesLength = (underline + strikeout) * totalLines * 4;

            int arrayLength = (String.Length * 4) + styleVerticesLength;
            if (vertices.Length != arrayLength)
            {
                vertices = new Vertex[arrayLength];
            }

            float minX = Font.FontSize;
            float minY = Font.FontSize;
            float maxX = 0.0f;
            float maxY = 0.0f;

            float advance = 0.0f;
            char prevChar = (char) 0;
            FaceMetrics faceMetrics = Font.FontFace.GetFaceMetrics(Font.FontSize);
            
            int line = 0;                   // The line count
            float currentLineWidth = 0.0f;  // The width of the current line (without font metric additions)
            float bearingXFirstChar = 0.0f; // Bearing x for first char on the line
            float bearingXLastChar = 0.0f;  // Bearing x for last char on the line
            Matrix mat = WorldMatrix;       // The world matrix to be used for vertex construction
            FloatRect lastCharRect = FloatRect.Zero; // The rect of the last char (stored when a line is terminated to use as last char on line)
            int verticesLineLength = 0;
            int verticesLineIndex = 0;

            // New line info will be added after a line terminates
            // Line info's are used later to draw strikeouts/underlines
            List<LineInfo> lines = new List<LineInfo>();
            GlyphInfo[] glyphInfos = new GlyphInfo[String.Length];
            
            // Load the glyphs first
            // This can not be done in the big loop because the texture might resize during a glyph load (to make it fit)
            // When the texture is resized all tex coords of previous iterations are no longer valid because the texture size has changed
            for (int i = 0; i < String.Length; i++)
            {
                char c = String[i];
                if (c == '\r' || c == '\n' || c == '\t')
                {
                    glyphInfos[i] = GlyphInfo.Empty;
                    continue;
                }
                glyphInfos[i] = Font.LoadGlyph(c);
            }
            
            // Make sure the texture still references the glyph atlas texture
            // The font can swap the texture when loading glyphs
            // When a new glyph does not fit on the texture, a new and bigger texture is generated
            // If we do not do this, we will be left with a reference to the old, disposed texture 
            Texture = Font.Texture;

            #region Fill Vertices
            // Create the vertices
            for (int i = 0; i < String.Length; i++)
            {
                int i0 = (i * 4);
                int i1 = i0 + 1;
                int i2 = i0 + 2;
                int i3 = i0 + 3;

                char curChar = String[i];
                bool newLine = false;
                FloatRect srcRect = FloatRect.Zero;    
                GlyphInfo glyphInfo = GlyphInfo.Empty;

                switch (curChar)
                {
                    case '\r':
                        // Environment.Newline on windows results in a \r\n string, so we ignore \r and use \n for new line
                        continue; // Continue the for loop
                    case '\n':
                    {
                        // new line
                        line++;
                        advance = 0.0f;
                        newLine = true;
                        bearingXLastChar = glyphInfo.BearingX;
                        break;
                    }
                    case '\t':
                        // tab = 4 spaces
                        advance += Font.LoadGlyph(' ').Advance * 4;
                        maxX = Math.Max(maxX, advance);
                        break;
                    default:
                    {
                        // Get glyph info from previously loaded array
                        glyphInfo = glyphInfos[i];
                            
                        if (glyphInfo == GlyphInfo.Empty)
                        {
                            // Glyph could not be loaded and is not a special case
                            float errorVertexSize = faceMetrics.CellAscent;
                            float errorVertexY = (faceMetrics.LineHeight * line) + faceMetrics.XHeight - errorVertexSize;

                            // Add a red quad to indicate a missing glyph
                            AddQuad(i0, advance, errorVertexY, errorVertexSize, errorVertexSize, Color.Red, ref vertices);
                            advance += errorVertexSize;
                            maxX = Math.Max(maxX, advance);
                            continue; // Next char
                        }
                        
                        srcRect = glyphInfo.SourceRect;
                        bearingXLastChar = glyphInfo.SourceRect.Width;
                        if (advance == 0.0f)
                        {    
                            bearingXFirstChar = glyphInfo.BearingX;
                        }
                        break;
                    }
                }

                // Calculate offsets from bearing
                float offsetY = (float)Math.Ceiling(glyphInfo.BearingY);
                float offsetX = (float)Math.Floor(glyphInfo.BearingX);
                
                // Apply x and y offsets, line offset, and advance
                float y = (float)Math.Floor((faceMetrics.XHeight - offsetY) + (float)Math.Floor(faceMetrics.LineHeight * line));
                float x = (float)Math.Round(offsetX + advance);
                
                // Apply kerning
                float kerning = Font.FontFace.GetKerning(prevChar, curChar, Font.FontSize);
                x += (float)Math.Ceiling(kerning);
                
                // Advance
                advance += (float)Math.Round(glyphInfo.Advance);
                prevChar = curChar;
                verticesLineLength += 4;

                // New line added or last line
                if (newLine || (i == String.Length - 1))
                {
                    float lineX = bearingXFirstChar;
                    float lineY = y - faceMetrics.LineHeight;

                    // Last line?
                    if (i == String.Length - 1)
                    {
                        // The line was not incremented so we need to do this manually for the last line
                        lineY += faceMetrics.LineHeight + offsetY;
                        maxY = Math.Max(maxY, (float)Math.Floor(faceMetrics.LineHeight * (line + 1)));
                    }

                    // Store the line info for adding underline/strikeout lines later
                    lines.Add(new LineInfo()
                    {
                        X = lineX,
                        LastCharRect = lastCharRect,
                        VerticesIndex = verticesLineIndex,
                        VerticesLength = verticesLineLength,
                        UnderlineY = lineY - faceMetrics.UnderlinePosition,
                        StrikeoutY = lineY - faceMetrics.StrikeoutPosition,
                        LineWidth = currentLineWidth + bearingXLastChar,
                        StrikeoutThickness = faceMetrics.StrikeoutSize,
                        UnderlineThickness = faceMetrics.UnderlineSize
                    });

                    verticesLineIndex = verticesLineLength;
                }

                currentLineWidth = advance;

                float skew = 0.0f;

                // Do not skew these characters
                if (curChar != '.' && 
                    curChar != '|' &&
                    curChar != ',')
                {
                    skew = StyleFlags.HasFlag(TextStyleFlags.Italic) ? (float)Font.FontSize / 4 : 0.0f;
                }
                
                vertices[i0].Position = mat * new Vector2(x + skew, y);
                vertices[i1].Position = mat * new Vector2(x + srcRect.Width + skew, y);
                vertices[i2].Position = mat * new Vector2(x + srcRect.Width, srcRect.Height + y);
                vertices[i3].Position = mat * new Vector2(x, srcRect.Height + y);
                

                lastCharRect = new FloatRect(x, y, srcRect.Width + skew, srcRect.Height);

                // for char 'space', x and y are max float, ignore this character
                if (curChar != ' ')
                {
                    minX = Math.Min(minX, x);
                    maxX = Math.Max(maxX, x + glyphInfo.BearingX + srcRect.Width - skew * glyphInfo.BearingY);
                    minY = Math.Min(minY, y);
                    maxY = Math.Max(maxY, faceMetrics.LineHeight * line); // Maxed by line height
                }

                int textureWidth = Texture.Size.X;
                int textureHeight = Texture.Size.Y;

                // Set vertex texture coordinates
                vertices[i0].TexCoords.X = srcRect.X / textureWidth;
                vertices[i0].TexCoords.Y = srcRect.Y / textureHeight;
                vertices[i1].TexCoords.X = (srcRect.X + srcRect.Width) / textureWidth;
                vertices[i1].TexCoords.Y = srcRect.Y / textureHeight;
                vertices[i2].TexCoords.X = (srcRect.X + srcRect.Width) / textureWidth;
                vertices[i2].TexCoords.Y = (srcRect.Y + srcRect.Height) / textureHeight;
                vertices[i3].TexCoords.X = srcRect.X / textureWidth;
                vertices[i3].TexCoords.Y = (srcRect.Y + srcRect.Height) / textureHeight;

                // Set vertex colors
                vertices[i0].Color = Color;
                vertices[i1].Color = Color;
                vertices[i2].Color = Color;
                vertices[i3].Color = Color;
            }
            #endregion
            
            #region Apply Stlying and Alignment

            // Find the widest line (alignment positioning is based on the widest line)
            LineInfo widestLine = lines.Aggregate((l0, l1) => l0.LineWidth > l1.LineWidth ? l0 : l1);

            // Position in the vertex used to insert styling quads
            int arrayPosition = vertices.Length - styleVerticesLength;
            
            // Apply styling and alignment on the lines
            foreach (LineInfo lineInfo in lines)
            {
                float alignmentOffsetX = 0.0f;
                float gap = widestLine.LineWidth - lineInfo.LineWidth;

                switch (Alignment)
                {
                    case TextAlignment.Left: // No need to do anything, vertices are left-aligned by default
                        break;
                    case TextAlignment.Center:
                    {        
                        if (gap > 0.0f)
                        {
                            alignmentOffsetX = gap / 2;
                        }

                        break;
                    }
                    case TextAlignment.Right:
                    {
                        alignmentOffsetX = gap;
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (underline == 1)
                {
                    AddQuad(arrayPosition,
                        (float)Math.Ceiling(lineInfo.X + alignmentOffsetX), 
                        (float)Math.Ceiling(lineInfo.UnderlineY),
                        (float)Math.Ceiling(lineInfo.LineWidth), 
                        (float)Math.Ceiling(lineInfo.UnderlineThickness), 
                        Color,
                        ref vertices);
                    arrayPosition += 4;
                }

                if (strikeout == 1)
                {
                    AddQuad(arrayPosition,
                        (float)Math.Ceiling(lineInfo.X + alignmentOffsetX), 
                        (float)Math.Ceiling(lineInfo.StrikeoutY),
                        (float)Math.Ceiling(lineInfo.LineWidth), 
                        (float)Math.Ceiling(lineInfo.StrikeoutThickness),
                        Color,
                        ref vertices);
                    arrayPosition += 4;
                }

                // Align the vertices
                for (int i = lineInfo.VerticesLength - 1; i >= lineInfo.VerticesIndex; i--)
                {
                    vertices[i].Position.X += alignmentOffsetX;
                }
            }

            #endregion

            // Set the local bounds
            _localBounds = new FloatRect(minX, minY, maxX, maxY);
        }

        private void AddQuad(int index, float x, float y, float width, float height, Color color, ref Vertex[] vertices)
        {
            int i0 = index;
            int i1 = i0 + 1;
            int i2 = i0 + 2;
            int i3 = i0 + 3;
            
            // Quad vertex positions
            Matrix mat = WorldMatrix;
            vertices[i0].Position = mat * new Vector2(x, y);
            vertices[i1].Position = mat * new Vector2(x + width, y);
            vertices[i2].Position = mat * new Vector2(x + width, height + y);
            vertices[i3].Position = mat * new Vector2(x, height + y);

            int textureWidth = Texture.Size.X;
            int textureHeight = Texture.Size.Y;

            // Quad texture coords represent the first pixel in the glyph atlas
            // This pixel is put there by the font in any font
            const float pixelLocationMax = 1.0f;
            vertices[i0].TexCoords.X = 0.0f;
            vertices[i0].TexCoords.Y = 0.0f;
            vertices[i1].TexCoords.X = pixelLocationMax / textureWidth;
            vertices[i1].TexCoords.Y = 0.0f;
            vertices[i2].TexCoords.X = pixelLocationMax / textureWidth;
            vertices[i2].TexCoords.Y = pixelLocationMax / textureHeight;
            vertices[i3].TexCoords.X = 0.0f;
            vertices[i3].TexCoords.Y = pixelLocationMax / textureHeight;

            // Quad vertex color
            vertices[i0].Color = color;
            vertices[i1].Color = color;
            vertices[i2].Color = color;
            vertices[i3].Color = color;
        }
    }
}
