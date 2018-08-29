
using System.Runtime.InteropServices;
using Elixir.GameFramework;

namespace Elixir.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public const int SizeInBytes = 8 * sizeof(float);

        public Vector2 Position;
        public Color Color;
        public Vector2 TexCoords;

        public Vertex(Vector2 position)
            : this(position, Color.White, Vector2.Zero)
        {
        }

        public Vertex(Vector2 position, Color color)
            : this(position, color, Vector2.Zero)
        {
        }

        public Vertex(Vector2 position, Color color, Vector2 texCoords)
        {
            Position = position;
            Color = color;
            TexCoords = texCoords;
        }
    }
}
