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

namespace Elixir.Utils
{
    /// <summary>
    /// Additional math utility methods to extend System.Math
    /// </summary>
    public static class MathEx
    {
        private const int LookupSize = 2048;
        private static bool _precomputed;
        private static readonly float[] SinLookup = new float[LookupSize];
        private static readonly float[] CosLookup = new float[LookupSize];

        private static void EnsurePrecomputedSinCos()
        {
            if (!_precomputed)
            {
                for (int i = 0; i < LookupSize; i++)
                {
                    SinLookup[i] = (float)Math.Sin(i * Math.PI / LookupSize * 2.0f);
                    CosLookup[i] = (float)Math.Cos(i * Math.PI / LookupSize * 2.0f);
                }
                _precomputed = true;
            }
        }

        /// <summary>
        /// Fast sine using a precomputed lookup table
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static float FastSin(float radians)
        {
            EnsurePrecomputedSinCos();
            int i = (int)(-radians / (Math.PI * 2) * LookupSize) & (LookupSize - 1);
            return SinLookup[i];
        }

        /// <summary>
        /// Fast cosine using a precomputed lookup table
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static float FastCos(float radians)
        {
            EnsurePrecomputedSinCos();
            int i = (int)(-radians / (Math.PI * 2) * LookupSize) & (LookupSize - 1);
            return CosLookup[i];
        }

        public static float ToDegrees(float radians)
        {
            return (float)((radians * 180.0f) / Math.PI);
        }

        public static float ToRadians(float degrees)
        {
            return (float)((degrees * Math.PI) / 180.0f);
        }

        public static float Clamp(float value, float min, float max)
        {
            return value > max ? max : (value < min ? min : value);
        }

        internal static float Clamp360(float degrees)
        {
            if (degrees < -360.0f)
                degrees += 360.0f;
            if (degrees > 360.0f)
                degrees -= 360.0f;
            return degrees;
        }
    }
}
