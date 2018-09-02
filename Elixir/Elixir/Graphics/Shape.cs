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