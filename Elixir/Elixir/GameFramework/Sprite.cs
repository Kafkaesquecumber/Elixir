using Elixir.Graphics;

namespace Elixir.GameFramework
{
    public class Sprite : DrawableActor
    {
        public IntRect SourceRect { get; set; }
        
        /// <inheritdoc />
        public Sprite(Texture texture)
            : this(texture, new IntRect(0, 0, texture.Size.X, texture.Size.Y))
        {
        }

        /// <inheritdoc />
        public Sprite(Texture texture, IntRect sourceRect)
        {
            Texture = texture;
            SourceRect = sourceRect;
        }

        /// <inheritdoc />
        public override FloatRect LocalBounds => new FloatRect(0, 0, SourceRect.Width, SourceRect.Height);

        /// <inheritdoc />
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
