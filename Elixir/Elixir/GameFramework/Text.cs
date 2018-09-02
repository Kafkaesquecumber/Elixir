// MIT License
// 
// Copyright(c) 2018 
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

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Elixir.Graphics;
using SharpFont;
using Font = Elixir.Graphics.Font;

namespace Elixir.GameFramework
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

            string testString = "Hello";
            Texture = new Texture(8000, 1000, TextureCreateOptions.Sharp);

            List<Surface> surfaces = new List<Surface>();
            for (int i = 0; i < testString.Length; i++)
            {
                Glyph glyph = Font.FontFace.GetGlyph(testString[i], 16.0f);
                
                Surface fontSurface = new Surface
                {
                    Bits = Marshal.AllocHGlobal(glyph.RenderWidth * glyph.RenderHeight),
                    Width = glyph.RenderWidth,
                    Height = glyph.RenderHeight,
                    Pitch = glyph.RenderWidth
                };

                glyph.RenderTo(fontSurface);

                int width = fontSurface.Width;
                int height = fontSurface.Height;
                int len = width * height;
                byte[] data = new byte[len];
                Marshal.Copy(fontSurface.Bits, data, 0, len);
                byte[] pixels = new byte[len * 4];

                int index = 0;
                for (int j = 0; j < len; j++)
                {
                    byte c = data[j];
                    pixels[index++] = 255;
                    pixels[index++] = 255;
                    pixels[index++] = 255;
                    pixels[index++] = c;
                }
                
                Texture.Update(pixels, new IntRect(width * i, 0, width, height));
                
                Marshal.FreeHGlobal(fontSurface.Bits);
            }
            
            

            //Texture.NativeTexture.Save("FT.png");

            String = text;
        }

        private FloatRect _localBounds;
        public override FloatRect LocalBounds
        {
            get
            {
                if (Texture == null)
                {
                    return FloatRect.Zero;
                }
                return new FloatRect(0, 0, Texture.Size.X, Texture.Size.Y);
            }
        }

        protected internal override Vertex[] GetVertices()
        {
            Vertex[] vertices = new Vertex[4];

            Matrix mat = WorldMatrix;
            vertices[0].Position = mat * new Vector2(0.0f, 0.0f);
            vertices[1].Position = mat * new Vector2(LocalBounds.Width, 0.0f);
            vertices[2].Position = mat * new Vector2(LocalBounds.Width, LocalBounds.Height);
            vertices[3].Position = mat * new Vector2(0.0f, LocalBounds.Height);

            int textureWidth = Texture.Size.X;
            int textureHeight = Texture.Size.Y;

            // Set vertex texture coordinates
            vertices[0].TexCoords.X = LocalBounds.X / textureWidth;
            vertices[0].TexCoords.Y = LocalBounds.Y / textureHeight;
            vertices[1].TexCoords.X = (LocalBounds.X + LocalBounds.Width) / textureWidth;
            vertices[1].TexCoords.Y = LocalBounds.Y / textureHeight;
            vertices[2].TexCoords.X = (LocalBounds.X + LocalBounds.Width) / textureWidth;
            vertices[2].TexCoords.Y = (LocalBounds.Y + LocalBounds.Height) / textureHeight;
            vertices[3].TexCoords.X = LocalBounds.X / textureWidth;
            vertices[3].TexCoords.Y = (LocalBounds.Y + LocalBounds.Height) / textureHeight;

            // Apply flips
            if (FlipX)
            {
                Vector2 v0 = vertices[0].TexCoords;
                vertices[0].TexCoords = vertices[1].TexCoords;
                vertices[1].TexCoords = v0;

                Vector2 v3 = vertices[3].TexCoords;
                vertices[3].TexCoords = vertices[2].TexCoords;
                vertices[2].TexCoords = v3;
            }
            if (FlipY)
            {
                Vector2 v0 = vertices[0].TexCoords;
                vertices[0].TexCoords = vertices[3].TexCoords;
                vertices[3].TexCoords = v0;

                Vector2 v1 = vertices[1].TexCoords;
                vertices[1].TexCoords = vertices[2].TexCoords;
                vertices[2].TexCoords = v1;
            }

            // Set vertex colors
            vertices[0].Color = Color;
            vertices[1].Color = Color;
            vertices[2].Color = Color;
            vertices[3].Color = Color;

            return vertices;
        }
    }
}
