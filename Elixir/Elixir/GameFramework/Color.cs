using System;

namespace Elixir.GameFramework
{
    public struct Color : IEquatable<Color>
    {
        /// <summary>
        /// (R=1, G=1, B=1, A=1);
        /// </summary>
        public static Color White => new Color(1.0f, 1.0f, 1.0f, 1.0f);

        /// <summary>
        /// (R=0, G=0, B=0, A=1);
        /// </summary>
        public static Color Black => new Color(0.0f, 0.0f, 0.0f, 1.0f);

        /// <summary>
        /// (R=1, G=0, B=0, A=1);
        /// </summary>
        public static Color Red => new Color(1.0f, 0.0f, 0.0f, 1.0f);

        /// <summary>
        /// (R=0, G=1, B=0, A=1);
        /// </summary>
        public static Color Green => new Color(0.0f, 1.0f, 0.0f, 1.0f);

        /// <summary>
        /// (R=0, G=0, B=1, A=1);
        /// </summary>
        public static Color Blue => new Color(0.0f, 0.0f, 1.0f, 1.0f);

        /// <summary>
        /// (R=0, G=0, B=0, A=0);
        /// </summary>
        public static Color Transparant => new Color(0.0f, 0.0f, 0.0f, 0.0f);

        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public Color(float r, float g, float b)
            : this(r, g, b, 1.0f)
        { }

        public Color(float r, float g)
            : this(r, g, 0.0f, 1.0f)
        { }

        public Color(float r)
            : this(r, 0.0f, 0.0f, 1.0f)
        { }

        public Color(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Create a byte array of rgba values (between 0 and 255)
        /// </summary>
        /// <returns></returns>
        public byte[] ToRgbaBytes()
        {
            byte[] bytes = 
            {
                ((byte)(R*255)),
                ((byte)(G*255)),
                ((byte)(B*255)),
                ((byte)(A*255))
            };
            return bytes;
        }

        public bool Equals(Color other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
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

            Color other = (Color) obj;
            return other.Equals(this);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = R.GetHashCode();
                hashCode = (hashCode * 397) ^ G.GetHashCode();
                hashCode = (hashCode * 397) ^ B.GetHashCode();
                hashCode = (hashCode * 397) ^ A.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator==(Color colorA, Color colorB)
        {
            return colorA.Equals(colorB);
        }

        public static bool operator !=(Color colorA, Color colorB)
        {
            return !colorA.Equals(colorB);
        }

        public override string ToString()
        {
            return $"(R:{R} G:{G} B:{B} A:{A})";
        }
    }

    public static class ColorExtensions
    {
        /// <summary>
        /// <para>Create a byte array of rgba values (between 0 and 255)</para>
        /// <para>The byte array will be 4 times the length of the color array</para>
        /// </summary>
        /// <param name="colors"></param>
        /// <returns></returns>
        public static byte[] ToRgbaBytes(this Color[] colors)
        {
            byte[] bytes = new byte[colors.Length * 4];
            int j = 0;
            for (int i = 0; i < colors.Length; i++, j+=4)
            {
                bytes[j + 0] = (byte)(colors[0].R * 255);
                bytes[j + 1] = (byte)(colors[0].G * 255);
                bytes[j + 2] = (byte)(colors[0].B * 255);
                bytes[j + 3] = (byte)(colors[0].A * 255);
            }

            return bytes;
        }
    }
}
