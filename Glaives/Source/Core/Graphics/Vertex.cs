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

using System.Runtime.InteropServices;
using Glaives.Core;

namespace Glaives.Core.Graphics
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

        public override string ToString()
        {
            return $"(P:{Position}, C:{Color}, T:{TexCoords})";
        }
    }
}
