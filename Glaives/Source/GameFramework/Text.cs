// MIT License
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
using Glaives.Graphics;
using Glaives.Internal.Graphics;
using SharpFont;
using Font = Glaives.Graphics.Font;

namespace Glaives.GameFramework
{
    public class Text : DrawableActor
    {
        internal struct LineInfo
        {
            internal float X;
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

        private Font _font;
        /// <summary>
        /// The font used to draw the text
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
            : this(font, text, TextStyleFlags.Regular)
        {
        }

        public Text(Font font, string text, TextStyleFlags styleFlags)
        {
            StyleFlags = styleFlags;
            Font = font;
            String = text;
            Texture = font.Texture;
        }
        
        private FloatRect _localBounds;
        public override FloatRect LocalBounds
        {
            get
            {
                //TODO: Calc bounds
                return FloatRect.Zero;
            }
        }
        
        protected override Vertex[] ConstructVertices()
        {
            Vertex[] vertices = new Vertex[String.Length * 4];

            float advance = 0.0f;
            char prevChar = (char) 0;
            FaceMetrics faceMetrics = Font.FontFace.GetFaceMetrics(Font.FontSize);
            
            int line = 0;                   // The line count
            float currentLineWidth = 0.0f;  // The width of the current line (without font metric additions)
            float bearingXFirstChar = 0.0f; // Bearing x for first char on the line
            float bearingXLastChar = 0.0f;  // Bearing x for last char on the line
            Matrix mat = WorldMatrix;       // The world matrix to be used for vertex construction

            // Make sure the texture still references the glyph atlas texture
            // The font can swap the texture when loading glyphs
            // When a new glyph does not fit on the texture, a new and bigger texture is generated
            // If we do not do this, we will be left with a reference to the old, disposed texture 
            Texture = Font.Texture;         

            // New line info will be added after a line terminates
            // Line info's are used later to draw strikeouts/underlines
            List<LineInfo> lines = new List<LineInfo>();
            
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
                        break;
                    default:
                    {
                        // Load glyph info
                        glyphInfo = Font.LoadGlyph(curChar);
                        if (glyphInfo == GlyphInfo.Empty)
                        {
                            // Glyph could not be loaded and is not a special case

                            float errorVertexSize = faceMetrics.CellAscent;
                            float errorVertexY = (faceMetrics.LineHeight * line) + faceMetrics.XHeight - errorVertexSize;
                            Vertex[] errorVertices = new Vertex[4];
                            errorVertices[0].Position = mat * new Vector2(advance, errorVertexY);
                            errorVertices[1].Position = mat * new Vector2(advance + errorVertexSize, errorVertexY);
                            errorVertices[2].Position = mat * new Vector2(advance + errorVertexSize, errorVertexY + errorVertexSize);
                            errorVertices[3].Position = mat * new Vector2(advance, errorVertexY + errorVertexSize);

                            errorVertices[0].TexCoords = new Vector2(0.0f, 0.0f);
                            errorVertices[1].TexCoords = new Vector2(1.0f, 0.0f);
                            errorVertices[2].TexCoords = new Vector2(1.0f, 1.0f);
                            errorVertices[3].TexCoords = new Vector2(0.0f, 1.0f);

                            Color errorVertexColor = Color.White;
                            errorVertices[0].Color = errorVertexColor;
                            errorVertices[1].Color = errorVertexColor;
                            errorVertices[2].Color = errorVertexColor;
                            errorVertices[3].Color = errorVertexColor;

                            RenderProgram errorRenderProgram = new RenderProgram(
                                BlendMode.Alpha, Engine.Get.DefaultContent.TextureGlyphNotFound, Shader.Default, DrawLayer);
                            
                            Engine.Get.Graphics.AddGeometry(errorVertices, errorRenderProgram);
                            advance += errorVertexSize;
                            continue;
                            
                        }

                        srcRect = glyphInfo.SourceRect;
                        bearingXLastChar = glyphInfo.SourceRect.Width;
                        if (advance == 0.0f)
                        {
                            // Used to determine underline/strikeout start locations on the x-axis
                            bearingXFirstChar = glyphInfo.BearingX;
                        }

                        break;
                    }
                }
                
                // Calculate offsets from bearing
                float offsetY = (float)Math.Ceiling(glyphInfo.BearingY);
                float offsetX = (float)Math.Ceiling(glyphInfo.BearingX);
                
                // Apply x and y offsets, line offset, and advance
                float y = (float)Math.Floor((faceMetrics.XHeight - offsetY) + (faceMetrics.LineHeight * line));
                float x = (float)Math.Ceiling(offsetX + advance);
                
                // Apply kerning
                float kerning = Font.FontFace.GetKerning(prevChar, curChar, Font.FontSize);
                x += (float)Math.Ceiling(kerning);
                
                // New line added or last line
                if (newLine  || (i == String.Length - 1))
                {
                    float lineX = bearingXFirstChar;
                    float lineY = y - faceMetrics.LineHeight;

                    // Last line?
                    if (i == String.Length - 1)
                    {
                        // The line was not incremented so we need to do this manually for the last line
                        lineY += faceMetrics.LineHeight + offsetY;
                        //lineX = 0.0f;
                    }

                    // Store the line info for adding underline/strikeout lines later
                    lines.Add(new LineInfo()
                    {
                        X = lineX,
                        UnderlineY = lineY - faceMetrics.UnderlinePosition,
                        StrikeoutY = lineY - faceMetrics.StrikeoutPosition,
                        LineWidth = currentLineWidth + bearingXLastChar,
                        StrikeoutThickness = faceMetrics.StrikeoutSize,
                        UnderlineThickness = faceMetrics.UnderlineSize
                    });
                }
                
                // Advance
                advance += (float)Math.Round(glyphInfo.Advance);
                currentLineWidth = advance;
                prevChar = curChar;

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

                // Apply flips
                if (FlipX)
                {
                    Vector2 v0 = vertices[i0].TexCoords;
                    vertices[i0].TexCoords = vertices[i1].TexCoords;
                    vertices[i1].TexCoords = v0;

                    Vector2 v3 = vertices[i3].TexCoords;
                    vertices[i3].TexCoords = vertices[i2].TexCoords;
                    vertices[i2].TexCoords = v3;
                }
                if (FlipY)
                {
                    Vector2 v0 = vertices[i0].TexCoords;
                    vertices[i0].TexCoords = vertices[i3].TexCoords;
                    vertices[i3].TexCoords = v0;

                    Vector2 v1 = vertices[i1].TexCoords;
                    vertices[i1].TexCoords = vertices[i2].TexCoords;
                    vertices[i2].TexCoords = v1;
                }

                // Set vertex colors
                vertices[i0].Color = Color;
                vertices[i1].Color = Color;
                vertices[i2].Color = Color;
                vertices[i3].Color = Color;
            }

            bool underline = StyleFlags.HasFlag(TextStyleFlags.Underline);
            bool strikeout = StyleFlags.HasFlag(TextStyleFlags.Strikeout);
            
            // Add strikeout/underline lines
            foreach (LineInfo lineInfo in lines)
            {
                if (underline)
                {
                    AddLine(
                        (float)Math.Ceiling(lineInfo.X), 
                        (float)Math.Ceiling(lineInfo.UnderlineY),
                        (float)Math.Ceiling(lineInfo.LineWidth), 
                        (float)Math.Ceiling(lineInfo.UnderlineThickness), 
                        ref vertices);
                }

                if (strikeout)
                {
                    AddLine(
                        (float)Math.Ceiling(lineInfo.X), 
                        (float)Math.Ceiling(lineInfo.StrikeoutY),
                        (float)Math.Ceiling(lineInfo.LineWidth), 
                        (float)Math.Ceiling(lineInfo.StrikeoutThickness), 
                        ref vertices);
                }
            }
            
            return vertices;
        }

        private void AddLine(float x, float y, float width, float thickness, ref Vertex[] vertices)
        {
            // Grow the vertex array to fit the line quad
            Array.Resize(ref vertices, vertices.Length + 4);

            // The indices of the last 4 vertices that we appended to the vertex array
            int i0 = vertices.Length - 4;
            int i1 = i0 + 1;
            int i2 = i0 + 2;
            int i3 = i0 + 3;
            
            // Line quad vertex positions
            Matrix mat = WorldMatrix;
            vertices[i0].Position = mat * new Vector2(x, y);
            vertices[i1].Position = mat * new Vector2(x + width, y);
            vertices[i2].Position = mat * new Vector2(x + width, thickness + y);
            vertices[i3].Position = mat * new Vector2(x, thickness + y);

            int textureWidth = Texture.Size.X;
            int textureHeight = Texture.Size.Y;

            // Line quad texture coords represent the first pixel in the glyph atlas
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

            // Line quad vertex color
            vertices[i0].Color = Color;
            vertices[i1].Color = Color;
            vertices[i2].Color = Color;
            vertices[i3].Color = Color;
        }
    }
}
