using System;
using System.Drawing;
using System.Drawing.Text;
using Elixir.Graphics;
using Elixir.Internal;
using OpenTK.Graphics.OpenGL;
using Font = Elixir.Graphics.Font;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using TextureWrapMode = Elixir.Graphics.TextureWrapMode;

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
            Texture = Font.GlyphTexture;
            
            String = text;
        }

        private FloatRect _localBounds;
        public override FloatRect LocalBounds
        {
            get => _localBounds;
        }

        protected internal override Vertex[] GetVertices()
        {
            Vertex[] vertices = new Vertex[String.Length * 4];

            float offsetX = 0.0f;
            float offsetY = 0.0f;

            for (int i = 0; i < String.Length; i++)
            {
                char glyph = String[i];
                FloatRect glyphRect = Font.GetGlyphSourceRect(glyph);

                if (glyph == ' ')
                {
                    glyphRect.Width = Font.SpaceWidth;
                }
                else if (glyph == '\t')
                {
                    glyphRect.Width = Font.SpaceWidth * 4.0f;
                }

                int iv0 = (i * 4) + 0;
                int iv1 = (i * 4) + 1;
                int iv2 = (i * 4) + 2;
                int iv3 = (i * 4) + 3;

                float skew = Font.CreateOptions.StyleFlags.HasFlag(FontStyleFlags.Italic) ? 12.0f : 0.0f;
                float vertexOffsetY = 0.0f;

                Matrix mat = WorldMatrix;
                vertices[iv0].Position = mat * new Vector2(offsetX + skew, vertexOffsetY);
                vertices[iv1].Position = mat * new Vector2(offsetX + glyphRect.Width + skew, vertexOffsetY);
                vertices[iv2].Position = mat * new Vector2(offsetX + glyphRect.Width, glyphRect.Height + vertexOffsetY);
                vertices[iv3].Position = mat * new Vector2(offsetX, glyphRect.Height + vertexOffsetY);

                int textureWidth = Texture.Size.X;
                int textureHeight = Texture.Size.Y;
                
                // Set vertex texture coordinates
                vertices[iv0].TexCoords.X = glyphRect.X / textureWidth;
                vertices[iv0].TexCoords.Y = glyphRect.Y / textureHeight;
                vertices[iv1].TexCoords.X = (glyphRect.X + glyphRect.Width) / textureWidth;
                vertices[iv1].TexCoords.Y = glyphRect.Y / textureHeight;
                vertices[iv2].TexCoords.X = (glyphRect.X + glyphRect.Width) / textureWidth;
                vertices[iv2].TexCoords.Y = (glyphRect.Y + glyphRect.Height) / textureHeight;
                vertices[iv3].TexCoords.X = glyphRect.X / textureWidth;
                vertices[iv3].TexCoords.Y = (glyphRect.Y + glyphRect.Height) / textureHeight;

                // Apply flips
                if (FlipX) // TODO: Should flip whole thing not per char
                {
                    Vector2 v0 = vertices[iv0].TexCoords;
                    vertices[iv0].TexCoords = vertices[iv1].TexCoords;
                    vertices[iv1].TexCoords = v0;

                    Vector2 v3 = vertices[iv3].TexCoords;
                    vertices[iv3].TexCoords = vertices[iv2].TexCoords;
                    vertices[iv2].TexCoords = v3;
                }
                if (FlipY) // TODO: Should flip whole thing not per char
                {
                    Vector2 v0 = vertices[iv0].TexCoords;
                    vertices[iv0].TexCoords = vertices[iv3].TexCoords;
                    vertices[iv3].TexCoords = v0;

                    Vector2 v1 = vertices[iv1].TexCoords;
                    vertices[iv1].TexCoords = vertices[iv2].TexCoords;
                    vertices[iv2].TexCoords = v1;
                }

                // Set vertex colors
                vertices[iv0].Color = Color;
                vertices[iv1].Color = Color;
                vertices[iv2].Color = Color;
                vertices[iv3].Color = Color;

                offsetX += glyphRect.Width;
                offsetY = Math.Max(offsetY, glyphRect.Height);
            }

            _localBounds = new FloatRect(0, 0, offsetX, offsetY); // TODO: Doesnt work?
            
            return vertices;
        }
    }
}
