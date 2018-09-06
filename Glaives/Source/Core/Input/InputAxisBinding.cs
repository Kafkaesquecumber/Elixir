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

namespace Glaives.Core.Input
{
    /// <summary>
    /// An axis and a scale for the value of that axis
    /// </summary>
    public struct InputAxisScalePair
    {
        /// <summary>
        /// The axis
        /// </summary>
        public InputAxis Axis { get; set; }
        /// <summary>
        /// The value by which to multiply the axis value
        /// </summary>
        public float Scale { get; set; }
    }

    /// <summary>
    /// Binding an id to an axis
    /// </summary>
    public struct InputAxisBinding
    {
        /// <summary>
        /// The identifier for this input axis binding
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The pairs contain the axes and the scales by which to multiply their values
        /// </summary>
        public InputAxisScalePair[] AxisScalePairs { get; set; }
    }
}