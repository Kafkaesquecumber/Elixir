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

namespace Glaives.Core.Graphics
{
    /// <summary>
    /// <para>The italic flag skews the vertices to appear to be italic (if you use an italic font, don not include the italic flag)</para>
    /// <para>The underline flag adds a underline to text lines as a generated quad</para>
    /// <para>The strikeout flag adds a line through the text as a generated quad</para>
    /// <para>At the moment, boldness can not be done artificially, if you want bold text you should load a bold font</para>
    /// </summary>
    [Flags]
    public enum TextStyleFlags
    {
        /// <summary>
        /// No modifications are done to the text
        /// </summary>
        Regular = 0,
        /// <summary>
        /// Skews the vertices to appear to be italic (if you use an italic font, don not include the italic flag)
        /// </summary>
        Italic = 1,
        /// <summary>
        /// Adds a underline to text lines as a generated quad
        /// </summary>
        Underline = 2,
        /// <summary>
        /// Adds a line through the text as a generated quad
        /// </summary>
        Strikeout = 4,
    }
    //TODO: Add emboldening support to SharpFont (not supported)
}