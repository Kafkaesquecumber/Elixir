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

namespace Elixir.GameFramework
{
    public struct Matrix
    {
        public static Matrix Identity
        {
            get
            {
                return new Matrix(1, 0, 0,
                                  0, 1, 0,
                                  0, 0, 1);
            }
        }

        private float m00, m01, m02;
        private float m10, m11, m12;
        private float m20, m21, m22;

        public Matrix(float a00, float a01, float a02,
                      float a10, float a11, float a12,
                      float a20, float a21, float a22)
        {
            m00 = a00; m01 = a01; m02 = a02;
            m10 = a10; m11 = a11; m12 = a12;
            m20 = a20; m21 = a21; m22 = a22;
        }

        public Matrix(Matrix matrix)
        {
            m00 = matrix.m00; m01 = matrix.m01; m02 = matrix.m02;
            m10 = matrix.m10; m11 = matrix.m11; m12 = matrix.m12;
            m20 = matrix.m20; m21 = matrix.m21; m22 = matrix.m22;
        }

        public Vector2 TransformPoint(Vector2 point)
        {
            return TransformPoint(point.X, point.Y);
        }

        public Vector2 TransformPoint(float x, float y)
        {
            return new Vector2(m00 * x + m01 * y + m02, m10 * x + m11 * y + m12);
        }

        public FloatRect TransformRect(FloatRect rect)
        {
            // Transform the 4 corners of the rectangle
            Vector2[] points = 
            {
                TransformPoint(rect.X, rect.Y),
                TransformPoint(rect.X, rect.Y + rect.Height),
                TransformPoint(rect.X + rect.Width, rect.Y),
                TransformPoint(rect.X + rect.Width, rect.Y + rect.Height)
            };

            // Compute the bounding rectangle of the transformed points
            float left = points[0].X;
            float top = points[0].Y;
            float right = points[0].X;
            float bottom = points[0].Y;
            for (int i = 1; i < 4; ++i)
            {
                if (points[i].X < left)
                {
                    left = points[i].X;
                }
                else if (points[i].X > right)
                {
                    right = points[i].X;
                }

                if (points[i].Y < top)
                {
                    top = points[i].Y;
                }
                else if (points[i].Y > bottom)
                {
                    bottom = points[i].Y;
                }
            }

            return new FloatRect(left, top, right - left, bottom - top);
        }

        public Matrix GetInverse()
        {
            // Compute the determinant
            float det = m00 * (m22 * m11 - m21 * m12) -
                        m10 * (m22 * m01 - m21 * m02) +
                        m20 * (m12 * m01 - m11 * m02);

            // Compute the inverse if the determinant is not zero
            // (don't use an epsilon because the determinant may *really* be tiny)
            if (det != 0.0f)
            {
                return new Matrix((m22 * m11 - m21 * m12) / det,
                                 -(m22 * m01 - m21 * m02) / det,
                                  (m12 * m01 - m11 * m02) / det,
                                 -(m22 * m10 - m20 * m12) / det,
                                  (m22 * m00 - m20 * m02) / det,
                                 -(m12 * m00 - m10 * m02) / det,
                                  (m21 * m10 - m20 * m11) / det,
                                 -(m21 * m00 - m20 * m01) / det,
                                  (m11 * m00 - m10 * m01) / det);
            }
            else
            {
                return Identity;
            }
        }

        public Matrix Scale(Vector2 factors)
        {
            return Scale(factors.X, factors.Y);
        }

        public Matrix Scale(float scaleX, float scaleY)
        {
            Matrix scaling = new Matrix(
                        scaleX, 0,      0,
                        0,      scaleY, 0,
                        0,      0,      1);

            return Combine(scaling);
        }

        public Matrix Scale(float scaleX, float scaleY, float centerX, float centerY)
        {
            Matrix scaling = new Matrix(
                        scaleX, 0,      centerX * (1 - scaleX),
                        0,      scaleY, centerY * (1 - scaleY),
                        0,      0,      1);

            return Combine(scaling);
        }

        public Matrix Combine(Matrix matrix)
        {
            Matrix a = this;
            Matrix b = matrix;
            
            Matrix combined = new Matrix
                             (a.m00 * b.m00 + a.m01 * b.m10 + a.m02 * b.m20,
                              a.m00 * b.m01 + a.m01 * b.m11 + a.m02 * b.m21,
                              a.m00 * b.m02 + a.m01 * b.m12 + a.m02 * b.m22,
                              a.m10 * b.m00 + a.m11 * b.m10 + a.m12 * b.m20,
                              a.m10 * b.m01 + a.m11 * b.m11 + a.m12 * b.m21,
                              a.m10 * b.m02 + a.m11 * b.m12 + a.m12 * b.m22,
                              a.m20 * b.m00 + a.m21 * b.m10 + a.m22 * b.m20,
                              a.m20 * b.m01 + a.m21 * b.m11 + a.m22 * b.m21,
                              a.m20 * b.m02 + a.m21 * b.m12 + a.m22 * b.m22);

            this = combined;
            return this;
        }

        public float[] ToMatrix4()
        {
            return new float[]
            {
                m00, m10, 0, m20,
                m01, m11, 0, m21,
                0,   0,   1, 0,
                m02, m12, 0, m22
            };
        }
        
        
        public override string ToString()
        {
            return "(" + $"{m00}, {m01}, {m02}," + $"{m10}, {m11}, {m12}," + $"{m20}, {m21}, {m22})";
        }
        
        public static Matrix operator*(Matrix left, Matrix right)
        {
            return new Matrix(left).Combine(right);
        }

        public static Vector2 operator *(Matrix left, Vector2 right)
        {
            return left.TransformPoint(right);
        }
    }
}
