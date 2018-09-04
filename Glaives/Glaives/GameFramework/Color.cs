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
using System.Collections.Generic;
using System.Linq;

namespace Glaives.GameFramework
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

        public override string ToString()
        {
            return $"(R:{R} G:{G} B:{B} A:{A})";
        }

        public static bool operator==(Color colorA, Color colorB)
        {
            return colorA.Equals(colorB);
        }

        public static bool operator !=(Color colorA, Color colorB)
        {
            return !colorA.Equals(colorB);
        }
        
        /// <summary>
        /// Convert a RGBA byte array to a BGRA byte array
        /// </summary>
        /// <param name="rgbaBytes">The bytes in RGBA order</param>
        /// <returns>The bytes in BGRA order</returns>
        public static byte[] ToBgra(byte[] rgbaBytes)
        {
            byte[] bgra = new byte[rgbaBytes.Length];

            for (int i = 0; i < rgbaBytes.Length; i += 4)
            {
                bgra[i + 0] = rgbaBytes[i + 2];
                bgra[i + 1] = rgbaBytes[i + 1];
                bgra[i + 2] = rgbaBytes[i + 0];
                bgra[i + 3] = rgbaBytes[i + 3];
            }

            return bgra;
        }

        /// <summary>
        /// Convert a BGRA byte array to a RGBA byte array
        /// </summary>
        /// <param name="bgraBytes">The bytes in BGRA order</param>
        /// <returns>The bytes in RGBA order</returns>
        public static byte[] ToRgba(byte[] bgraBytes)
        {
            byte[] rgba = new byte[bgraBytes.Length];

            for (int i = 0; i < bgraBytes.Length; i += 4)
            {
                rgba[i + 0] = bgraBytes[i + 2];
                rgba[i + 1] = bgraBytes[i + 1];
                rgba[i + 2] = bgraBytes[i + 0];
                rgba[i + 3] = bgraBytes[i + 3];
            }

            return rgba;
        }

    }

    public static class ColorExtensions
    {
        /// <summary>
        /// <para>Create an RGBA byte array of rgba values (between 0 and 255)</para>
        /// <para>The RGBA byte array will be 4 times the length of the color collection</para>
        /// </summary>
        /// <param name="colors">The colors to convert to RGBA bytes</param>
        /// <returns>The RGBA bytes</returns>
        public static byte[] ToBytes(this IEnumerable<Color> colors)
        {
            Color[] colorArray = colors as Color[] ?? colors.ToArray();
            byte[] bytes = new byte[colorArray.Length * 4];
            int i = 0;

            foreach (Color color in colorArray)
            {
                bytes[i + 0] = (byte)(color.R * 255);
                bytes[i + 1] = (byte)(color.G * 255);
                bytes[i + 2] = (byte)(color.B * 255);
                bytes[i + 3] = (byte)(color.A * 255);
                i += 4;
            }
            
            return bytes;
        }
        
        /// <summary>
        /// <para>Convert the RGBA bytes to colors</para>
        /// <para>The color array will be a quarter the length of the byte array</para>
        /// </summary>
        /// <param name="bytes">The RGBA bytes to convert</param>
        /// <returns>The colors</returns>
        public static Color[] ToColors(this IEnumerable<byte> bytes)
        {
            byte[] byteArray = bytes as byte[] ?? bytes.ToArray();

            Color[] colors = new Color[byteArray.Length / 4];
            for (int i = 0; i < colors.Length; i++)
            {
                float r = (float)byteArray[i + 0] / 255;
                float g = (float)byteArray[i + 1] / 255;
                float b = (float)byteArray[i + 2] / 255;
                float a = (float)byteArray[i + 3] / 255;
                colors[i] = new Color(r, g, b, a);
            }

            return colors;
        }

        
    }
}
