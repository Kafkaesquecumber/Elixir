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
