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
using Glaives.GameFramework;
using Glaives.Utils;

namespace Glaives.Graphics
{
    /// <inheritdoc />
    /// <summary>
    /// <para>A view is used to project the geometry onto the viewport</para>
    /// <para>It contains the projection matrix which is used in the vertex shaders to transform the vertices to view-space</para>
    /// </summary>
    public class View : Actor
    {
        private IntVector2 _size;
        internal IntVector2 Size
        {
            get => _size;
            private set
            {
                if (_size != value)
                {
                    _size = value;
                    _isDirty = true;
                }
            }
        }

        //TODO: view determines frame buffer texture sample mode?
        
        
        private Matrix _projectionMatrix;
        /// <summary>
        /// The projection matrix of the current view is passed to the vertex shaders to transform the vertices to view-space
        /// </summary>
        public Matrix ProjectionMatrix
        {
            get
            {
                if (_isDirty)
                {
                    Center = Position + new GameFramework.Vector2((float)Size.X / 2, (float)Size.Y / 2);
                    float angle = Rotation * (float) Math.PI / 180.0f;
                    float cosine = MathEx.FastCos(angle);
                    float sine = MathEx.FastSin(angle);
                    float tx = -Center.X * cosine - Center.Y * sine + Center.X;
                    float ty = Center.X * sine - Center.Y * cosine + Center.Y;

                    float a = 2.0f / Size.X;
                    float b = -2.0f / Size.Y;
                    float c = -a * Center.X;
                    float d = -b * Center.Y;

                    _projectionMatrix = new Matrix(
                        a * cosine, a * sine, a * tx + c,
                        -b * sine, b * cosine, b * ty + d,
                        0.0f, 0.0f, 1.0f);
                }

                return _projectionMatrix;
            }
        }

        /// <summary>
        /// The center of the view
        /// </summary>
        public GameFramework.Vector2 Center { get; private set; }
        
        private bool _isDirty;
        
        public View(IntVector2 size)
            : this(false, size)
        {
            
        }
        
        internal View(bool immutable, IntVector2 size)
            : base(immutable)
        {
            Size = size;
            Center = Position + new GameFramework.Vector2((float)Size.X / 2, (float)Size.Y / 2);
            _isDirty = true;
        }
        
    }
}