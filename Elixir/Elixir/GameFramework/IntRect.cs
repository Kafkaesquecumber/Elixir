using System;
using System.Runtime.InteropServices;

namespace Elixir.GameFramework
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct IntRect : IEquatable<IntRect>
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public IntVector2 Size => new IntVector2(Width, Height);

        public IntRect(IntVector2 position, IntVector2 size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }

        public IntRect(int x, int y, IntVector2 size)
            : this(new IntVector2(x, y), size)
        {            
        }

        public IntRect(IntVector2 position, int width, int height)
            : this(position, new IntVector2(width, height))
        {
        }

        public IntRect(int x, int y, int width, int height)
            : this(new IntVector2(x, y), new IntVector2(width, height))
        {
        }

        public bool Equals(IntRect other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is IntRect rect && Equals(rect);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Width;
                hashCode = (hashCode * 397) ^ Height;
                return hashCode;
            }
        }

        public static bool operator ==(IntRect left, IntRect right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IntRect left, IntRect right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Width}, {Height})";
        }

        public static IntRect Zero => new IntRect(0, 0, 0, 0);
    }
}