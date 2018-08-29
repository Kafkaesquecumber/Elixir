using Elixir.GameFramework;

namespace Elixir.Graphics
{
    public static class Shape
    {
        /// <summary>
        /// Create a vertex array that represents a quad
        /// </summary>
        /// <returns></returns>
        public static Vertex[] MakeQuad()
        {
            Vertex[] vertices = new Vertex[4];

            vertices[0].Position = new Vector2(-1.0f, -1.0f);
            vertices[1].Position = new Vector2( 1.0f, -1.0f);
            vertices[2].Position = new Vector2( 1.0f,  1.0f);
            vertices[3].Position = new Vector2(-1.0f,  1.0f);

            vertices[0].TexCoords = new Vector2(0.0f, 0.0f);
            vertices[1].TexCoords = new Vector2(1.0f, 0.0f);
            vertices[2].TexCoords = new Vector2(1.0f, 1.0f);
            vertices[3].TexCoords = new Vector2(0.0f, 1.0f);

            vertices[0].Color = Color.White;
            vertices[1].Color = Color.White;
            vertices[2].Color = Color.White;
            vertices[3].Color = Color.White;

            return vertices;
        }
    }
}