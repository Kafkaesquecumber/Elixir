﻿// MIT License
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

using System.Linq;
using Glaives.Core.Internal;

namespace Glaives.Core.Input
{
    /// <summary>
    /// Binding an id to an array of keys
    /// </summary>
    public struct InputActionBinding
    {
        /// <summary>
        /// The identifier for this input binding
        /// </summary>
        public string Id { get; set; }

        private Key[] _keys;

        /// <summary>
        /// The keys associated with this input binding
        /// </summary>
        public Key[] Keys
        {
            get => _keys;
            set
            {
                if (value.Any(key => key == Key.Unknown))
                {
                    throw new GlaivesException($"Can not create an {nameof(InputActionBinding)} using {nameof(Key)}.{nameof(Key.Unknown)}");
                }

                _keys = value;
            }
        }
    }
}
