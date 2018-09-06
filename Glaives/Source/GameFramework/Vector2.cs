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

using System;
using System.Runtime.InteropServices;

namespace Glaives.GameFramework
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2 : IEquatable<Vector2>
    {
        public float X;
        public float Y;

        /// <summary>
        /// The length of the vector
        /// </summary>
        public float Length => (float)Math.Sqrt(X * X + Y * Y);

        /// <summary>
        /// The square length of the vector (offers better performance than Length)
        /// </summary>
        public float LengthSquared => (X * X + Y * Y);

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }
        
        public bool Equals(Vector2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            Vector2 other = (Vector2)obj;
            return other.Equals(this);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        /// <summary>
        /// Calculate the dot product of 2 vectors
        /// </summary>
        /// <param name="left">Input vector a</param>
        /// <param name="right">Input vector b</param>
        /// <returns>The dot product</returns>
        public static float Dot(Vector2 left, Vector2 right)
        {
            return left.X * right.X + left.Y * right.Y;
        }

        /// <summary>
        /// Calculate the linearly interpolated vector
        /// </summary>
        /// <param name="a">Input vector a</param>
        /// <param name="b">Input vector b</param>
        /// <param name="alpha">The amount of interpolation</param>
        /// <returns>The interpolated vector</returns>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float alpha)
        {
            return new Vector2(alpha * (b.X - a.X) + a.X, alpha * (b.Y - a.Y) + a.Y);
        }

        /// <summary>
        /// Scale the vector to unit length
        /// </summary>
        /// <param name="vector">The input vector</param>
        /// <returns>The normalized vector</returns>
        public static Vector2 Normalize(Vector2 vector)
        {
            float scale = 1.0f / vector.Length;
            return new Vector2(vector.X * scale, vector.Y * scale);
        }

        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return !left.Equals(right);
        }

        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }

        public static Vector2 operator *(Vector2 vector, float scale)
        {
            return new Vector2(vector.X * scale, vector.Y * scale);
        }

        public static Vector2 operator /(Vector2 vector, float scale)
        {
            return new Vector2(vector.X / scale, vector.Y / scale);
        }

        /// <summary>
        /// A vector2 with X and Y set to 1
        /// </summary>
        public static Vector2 Unit => new Vector2(1.0f, 1.0f);

        /// <summary>
        /// A vector2 with X set to 1 and Y set to 0
        /// </summary>
        public static Vector2 UnitX => new Vector2(1.0f, 0.0f);

        /// <summary>
        /// A vector2 with X set to 0 and Y set to 1
        /// </summary>
        public static Vector2 UnitY => new Vector2(1.0f, 0.0f);

        /// <summary>
        /// A vector2 with X and Y set to 0
        /// </summary>
        public static Vector2 Zero => new Vector2(0.0f, 0.0f);
    }
}
