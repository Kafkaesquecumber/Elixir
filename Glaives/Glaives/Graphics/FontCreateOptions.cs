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

namespace Glaives.Graphics
{
    /// <summary>
    /// The create options to use for the font
    /// </summary>
    public struct FontCreateOptions : IEquatable<FontCreateOptions>
    {
        /// <summary>
        /// The size of the font, must be between 0 and 256
        /// </summary>
        public readonly int FontSize;

        /// <summary>
        /// The styling of the font (flags)
        /// </summary>
        public readonly FontStyleFlags StyleFlags;

        /// <summary>
        /// The alphabets supported by the font (flags)
        /// </summary>
        public readonly AlphabetFlags SupportedAlphabetsFlags;

        /// <summary>
        /// The filter mode for the font texture
        /// </summary>
        public readonly TextureFilterMode FilterMode;

        /// <inheritdoc />
        /// <summary>
        /// Create new font create options
        /// </summary>
        /// <param name="fontSize">The size of the font</param>
        public FontCreateOptions(int fontSize)
            : this(fontSize, FontStyleFlags.Regular, TextureFilterMode.Linear, AlphabetFlags.Latin)
        {
            FontSize = fontSize;
        }

        /// <inheritdoc />
        /// <summary>
        /// Create new font create options
        /// </summary>
        /// <param name="fontSize">The size of the font</param>
        /// <param name="styleFlags">The styling of the font (flags)</param>
        public FontCreateOptions(int fontSize, FontStyleFlags styleFlags)
            : this(fontSize, styleFlags, TextureFilterMode.Linear, AlphabetFlags.Latin)
        {
            FontSize = fontSize;
            StyleFlags = styleFlags;
        }

        /// <inheritdoc />
        /// <summary>
        /// Create new font create options
        /// </summary>
        /// <param name="fontSize">The size of the font</param>
        /// <param name="styleFlags">The styling of the font (flags)</param>
        /// <param name="filterMode">The filter mode for the font texture</param>
        public FontCreateOptions(int fontSize, FontStyleFlags styleFlags, TextureFilterMode filterMode)
            : this(fontSize, styleFlags, filterMode, AlphabetFlags.Latin)
        {
            FontSize = fontSize;
            FilterMode = filterMode;
            StyleFlags = styleFlags;
        }

        /// <summary>
        /// Create new font create options
        /// </summary>
        /// <param name="fontSize">The size of the font</param>
        /// <param name="styleFlags">The styling of the font (flags)</param>
        /// <param name="filterMode">The filter mode for the font texture</param>
        /// <param name="supportedAlphabetsFlags">The alphabets supported by the font (flags)</param>
        public FontCreateOptions(int fontSize, FontStyleFlags styleFlags, TextureFilterMode filterMode, AlphabetFlags supportedAlphabetsFlags)
        {
            FontSize = fontSize;
            FilterMode = filterMode;
            StyleFlags = styleFlags;
            SupportedAlphabetsFlags = supportedAlphabetsFlags;
        }

        public bool Equals(FontCreateOptions other)
        {
            return FontSize.Equals(other.FontSize) && StyleFlags == other.StyleFlags && SupportedAlphabetsFlags == other.SupportedAlphabetsFlags && FilterMode == other.FilterMode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is FontCreateOptions && Equals((FontCreateOptions) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = FontSize.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) StyleFlags;
                hashCode = (hashCode * 397) ^ (int) SupportedAlphabetsFlags;
                hashCode = (hashCode * 397) ^ (int) FilterMode;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"(Size: {FontSize}, StyleFlags: {StyleFlags}, Filter: {FilterMode}, Alphabets: {SupportedAlphabetsFlags})";
        }

        public static bool operator ==(FontCreateOptions left, FontCreateOptions right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FontCreateOptions left, FontCreateOptions right)
        {
            return !left.Equals(right);
        }
    }
}