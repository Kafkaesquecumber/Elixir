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
using System.Runtime.InteropServices;

namespace Elixir.GameFramework
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct IntVector2 : IEquatable<IntVector2>
    {
        public int X;
        public int Y;
        
        public IntVector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public bool Equals(IntVector2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is IntVector2 vector2 && Equals(vector2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public static bool operator ==(IntVector2 left, IntVector2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IntVector2 left, IntVector2 right)
        {
            return !left.Equals(right);
        }

        public static IntVector2 operator -(IntVector2 left, IntVector2 right)
        {
            return new IntVector2(left.X - right.X, left.Y - right.Y);
        }

        public static IntVector2 operator +(IntVector2 left, IntVector2 right)
        {
            return new IntVector2(left.X + right.X, left.Y + right.Y);
        }

        public static IntVector2 operator *(IntVector2 vector, int scale)
        {
            return new IntVector2(vector.X * scale, vector.Y * scale);
        }

        public static IntVector2 operator /(IntVector2 vector, int scale)
        {
            return new IntVector2(vector.X / scale, vector.Y / scale);
        }

        /// <summary>
        /// An int vector with X and Y set to 0
        /// </summary>
        public static IntVector2 Zero => new IntVector2(0, 0);

        /// <summary>
        /// An int vector with X set to 1 and Y set to 0
        /// </summary>
        public static IntVector2 UnitX => new IntVector2(1, 0);

        /// <summary>
        /// An int vector with X set to 0 and Y set to 1
        /// </summary>
        public static IntVector2 UnitY => new IntVector2(0, 1);

        /// <summary>
        /// An int vector with X and Y set to 1
        /// </summary>
        public static IntVector2 Unit => new IntVector2(1, 1);
    }
}
