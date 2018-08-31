using System;
using Elixir.Graphics;

namespace Elixir.Internal.Graphics
{
    internal struct RenderProgram : IEquatable<RenderProgram>
    {
        internal static RenderProgram Default => new RenderProgram(BlendMode.Alpha, null, Shader.Default, 0);

        internal readonly BlendMode BlendMode;
        internal readonly Shader Shader;
        internal readonly Texture Texture;
        internal readonly int DrawLayer;
        
        internal RenderProgram(BlendMode blendMode, Texture texture, Shader shader, int drawLayer)
        {
            this.BlendMode = blendMode;
            this.Texture = texture;
            this.Shader = shader;
            this.DrawLayer = drawLayer;
        }
        
        public bool Equals(RenderProgram other)
        {
            return BlendMode.Equals(other.BlendMode) && 
                   Equals(Shader, other.Shader) && 
                   Equals(Texture, other.Texture) && 
                   DrawLayer == other.DrawLayer;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is RenderProgram && Equals((RenderProgram) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = BlendMode.GetHashCode();
                hashCode = (hashCode * 397) ^ (Shader != null ? Shader.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Texture != null ? Texture.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ DrawLayer;
                return hashCode;
            }
        }

        public static bool operator ==(RenderProgram left, RenderProgram right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RenderProgram left, RenderProgram right)
        {
            return !left.Equals(right);
        }
    }
}
