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
using System.Linq;
using System.Runtime.InteropServices;
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
            internal float Width;
            internal float UnderlineThickness;
            internal float StrikeoutThickness;
        }

        /// <summary>
        /// The string to be drawn by the text actor
        /// </summary>
        public string String { get; set; }

        
        public TextStyleFlags StyleFlags { get; set; }
        
        public Font Font { get; set; }

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

        protected internal override Vertex[] GetVertices()
        {
            Vertex[] vertices = new Vertex[String.Length * 4];

            float advance = 0.0f;
            char prevChar = (char) 0;
            FaceMetrics faceMetrics = Font.FontFace.GetFaceMetrics(Font.FontSize);
            
            int line = 0;
            float lineWidth = 0.0f;
            Texture = Font.Texture; // We need to do this because the font texture might change
            List<LineInfo> lines = new List<LineInfo>();

            for (int i = 0; i < String.Length; i++)
            {
                int i0 = (i * 4);
                int i1 = i0 + 1;
                int i2 = i0 + 2;
                int i3 = i0 + 3;

                char curChar = String[i];
                FloatRect srcRect = FloatRect.Zero;
                
                GlyphInfo glyphInfo = GlyphInfo.Empty;
                 
                if (curChar == '\n')
                {
                    // new line
                    line++;
                    advance = 0.0f;
                }
                else if (curChar == '\t')
                {
                    // tab = 4 spaces
                    advance += Font.LoadGlyph(' ').Advance * 4;
                }
                else
                {
                    // Load glyph info
                    glyphInfo = Font.LoadGlyph(curChar);
                    srcRect = glyphInfo.SourceRect;
                }

                // Calculate offsets from bearing
                float offsetY = (float)Math.Ceiling(glyphInfo.BearingY);
                float offsetX = (float)Math.Floor(glyphInfo.BearingX);

                // Apply x and y offsets, line offset, and advance
                float y = (faceMetrics.XHeight - offsetY) + (faceMetrics.LineHeight * line);
                float x = (offsetX + advance);
                
                // Apply kerning
                float kerning = Font.FontFace.GetKerning(prevChar, curChar, Font.FontSize);
                x += kerning;

                
                if ((advance == 0.0f && i != 0)  // A line was added with \n
                    || 
                    (i == String.Length - 1)) // Last loop iteration, text is only single line
                {
                    float lineX = x;
                    float lineY = y - faceMetrics.LineHeight;

                    if (i == String.Length - 1)
                    {
                        lineY += faceMetrics.LineHeight + offsetY;
                        lineX = 0.0f;
                    }

                    // Store the line info for adding underline/strikeout lines later
                    lines.Add(new LineInfo()
                    {
                        X = lineX,
                        UnderlineY = lineY - faceMetrics.UnderlinePosition,
                        StrikeoutY = lineY - faceMetrics.StrikeoutPosition,
                        Width = lineWidth + glyphInfo.Advance, 
                        StrikeoutThickness = faceMetrics.StrikeoutSize,
                        UnderlineThickness = faceMetrics.UnderlineSize
                    });

                    throw new Exception("TODO: Why does the + advance in line 154 cause the line to bleed into the start location????");
                }
                
                // Advance
                advance += glyphInfo.Advance;
                lineWidth = advance;
                prevChar = curChar;

                float skew = 0.0f;

                // Do not skew these characters
                if (curChar != '.' && 
                    curChar != '|')
                {
                    skew = StyleFlags.HasFlag(TextStyleFlags.Italic) ? (float)Font.FontSize / 4 : 0.0f;
                }
                
                Matrix mat = WorldMatrix;
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
                        lineInfo.X, 
                        (float)Math.Ceiling(lineInfo.UnderlineY),
                        (float)Math.Ceiling(lineInfo.Width), 
                        (float)Math.Ceiling(lineInfo.UnderlineThickness), 
                        ref vertices);
                }

                if (strikeout)
                {
                    AddLine(
                        lineInfo.X, 
                        (float)Math.Ceiling(lineInfo.StrikeoutY),
                        (float)Math.Ceiling(lineInfo.Width), 
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

            int i0 = vertices.Length - 1;
            int i1 = i0 - 1;
            int i2 = i0 - 2;
            int i3 = i0 - 3;
            
            Matrix mat = WorldMatrix;
            vertices[i0].Position = mat * new Vector2(x, y);
            vertices[i1].Position = mat * new Vector2(x + width, y);
            vertices[i2].Position = mat * new Vector2(x + width, thickness + y);
            vertices[i3].Position = mat * new Vector2(x, thickness + y);

            int textureWidth = Texture.Size.X;
            int textureHeight = Texture.Size.Y;

            float pixelLocationMax = 1.0f;

            vertices[i0].TexCoords.X = 0.0f;
            vertices[i0].TexCoords.Y = 0.0f;
            vertices[i1].TexCoords.X = pixelLocationMax / textureWidth;
            vertices[i1].TexCoords.Y = 0.0f;
            vertices[i2].TexCoords.X = pixelLocationMax / textureWidth;
            vertices[i2].TexCoords.Y = pixelLocationMax / textureHeight;
            vertices[i3].TexCoords.X = 0.0f;
            vertices[i3].TexCoords.Y = pixelLocationMax / textureHeight;

            vertices[i0].Color = Color;
            vertices[i1].Color = Color;
            vertices[i2].Color = Color;
            vertices[i3].Color = Color;
        }
    }
}
