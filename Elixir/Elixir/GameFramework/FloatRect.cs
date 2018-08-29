using System;
using System.Runtime.InteropServices;

namespace Elixir.GameFramework
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct FloatRect : IEquatable<FloatRect>
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public Vector2 Size => new Vector2(Width, Height);

        public FloatRect(Vector2 position, Vector2 size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }

        public FloatRect(float x, float y, Vector2 size)
            : this(new Vector2(x, y), size)
        {
        }

        public FloatRect(Vector2 position, float width, float height)
            : this(position, new Vector2(width, height))
        {
        }

        public FloatRect(float x, float y, float width, float height)
            : this(new Vector2(x, y), new Vector2(width, height))
        {
        }

        public bool Equals(FloatRect other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is FloatRect rect && Equals(rect);
        }

        public override int GetHashCode()
        {
            var hashCode = 466501756;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(FloatRect left, FloatRect right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FloatRect left, FloatRect right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Width}, {Height})";
        }

        public static FloatRect Zero => new FloatRect(0, 0, 0, 0);
    }
}