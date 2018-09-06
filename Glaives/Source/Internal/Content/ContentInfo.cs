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
using Glaives.GameFramework;

namespace Glaives.Internal.Content
{
    internal struct ContentInfo : IEquatable<ContentInfo>
    {
        internal readonly string File;
        internal readonly ContentCreateOptions CreateOptions;

        public ContentInfo(string file, ContentCreateOptions createOptions)
        {
            File = file;
            CreateOptions = createOptions;
        }

        public bool Equals(ContentInfo other)
        {
            return string.Equals(File, other.File) && CreateOptions.IsEqualContentInternal(other.CreateOptions);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ContentInfo && Equals((ContentInfo) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((File != null ? File.GetHashCode() : 0) * 397) ^ (CreateOptions != null ? CreateOptions.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ContentInfo left, ContentInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ContentInfo left, ContentInfo right)
        {
            return !left.Equals(right);
        }
    }
}