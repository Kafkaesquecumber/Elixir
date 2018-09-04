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
using System.Runtime.InteropServices;
using Glaives.Graphics;
using Glaives.Internal.Graphics;
using SharpFont;
using Font = Glaives.Graphics.Font;

namespace Glaives.GameFramework
{
    public class Text : DrawableActor
    {
        private string _string;
        
        /// <summary>
        /// The string to be drawn by the text actor
        /// </summary>
        public string String { get; set; }

        
        public TextStyle Style { get; set; }
        
        public Font Font { get; set; }
        
        public Text(Font font, string text)
        {
            Style = TextStyle.Normal;
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
            Vertex[] vertices = new Vertex[4 * String.Length];

            float advance = 0.0f;
            char prevChar = (char) 0;
            FaceMetrics faceMetrics = Font.FontFace.GetFaceMetrics(Font.CreateOptions.FontSize);

            int line = 0;
            Texture = Font.Texture; // We need to do this because the font texture might change
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
                    line++;
                    advance = 0.0f;
                }
                else
                {
                    glyphInfo = Font.LoadGlyph(curChar);
                    srcRect = glyphInfo.SourceRect;
                }

                float y = faceMetrics.LineHeight - glyphInfo.BearingY;
                y += faceMetrics.LineHeight * line;

                float kerning = Font.FontFace.GetKerning(curChar, prevChar, Font.CreateOptions.FontSize);
                float x = (float)Math.Ceiling(advance + glyphInfo.BearingX + kerning);

                Matrix mat = WorldMatrix;
                vertices[i0].Position = mat * new Vector2(x, y);
                vertices[i1].Position = mat * new Vector2(x + srcRect.Width, y);
                vertices[i2].Position = mat * new Vector2(x + srcRect.Width, srcRect.Height + y);
                vertices[i3].Position = mat * new Vector2(x, srcRect.Height + y);
                
                advance += glyphInfo.Advance;
                prevChar = curChar;

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
            

            return vertices;
        }
    }
}
