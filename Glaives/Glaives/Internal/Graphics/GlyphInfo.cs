// MIT License
// 
// Copyright (c) 2018 Glaives Game Engine.
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

namespace Glaives.Internal.Graphics
{
    internal struct GlyphInfo : IEquatable<GlyphInfo>
    {
        internal static GlyphInfo Empty => new GlyphInfo();

        internal float BearingX;
        internal float BearingY;
        internal float Advance;
        internal FloatRect SourceRect;

        /// <inheritdoc />
        public bool Equals(GlyphInfo other)
        {
            return BearingX.Equals(other.BearingX) && BearingY.Equals(other.BearingY) && Advance.Equals(other.Advance) && SourceRect.Equals(other.SourceRect);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is GlyphInfo other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = BearingX.GetHashCode();
                hashCode = (hashCode * 397) ^ BearingY.GetHashCode();
                hashCode = (hashCode * 397) ^ Advance.GetHashCode();
                hashCode = (hashCode * 397) ^ SourceRect.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(GlyphInfo left, GlyphInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GlyphInfo left, GlyphInfo right)
        {
            return !left.Equals(right);
        }
    }
}