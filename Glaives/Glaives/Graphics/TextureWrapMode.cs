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

namespace Glaives.Graphics
{
    /// <summary>
    /// The way the texture is sampled outside the coordinate range of 0 to 1
    /// </summary>
    public enum TextureWrapMode
    {
        /// <summary>
        /// The integer part of the coordinate is ignored and a repeated pattern is formed
        /// </summary>
        Repeat,
        /// <summary>
        /// Same as Repeat but it will be mirrored when the integer part of the coordinate is odd
        /// </summary>
        MirroredRepeat,
        /// <summary>
        /// The coordinate will be clamped between 0 and 1
        /// </summary>
        ClampToEdge,
        /// <summary>
        /// The coordinates that fall outside the range will be given a border
        /// </summary>
        ClampToBorder
    }
}