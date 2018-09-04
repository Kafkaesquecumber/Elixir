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

using System.Collections.Generic;

namespace Glaives.GameFramework
{
    /// <summary>
    /// Ensures there is no other UniqueString with the same string value during the same runtime
    /// Adds a postfix to make the string unique to other UniqueStrings
    /// </summary>
    public sealed class UniqueString
    {
        private static readonly List<string> UsedStrings = new List<string>();

        /// <summary>
        /// The unique string
        /// </summary>
        public readonly string String;
        
        /// <summary>
        /// Potentially modifies the input str by adding a postfix to ensure it is unique to other UniqueStrings
        /// </summary>
        /// <param name="str"></param>
        public UniqueString(string str)
        {
            int postfix = 0;
            string store = str;
            do
            {
                str = store + "_" + postfix.ToString();
                postfix++;
            } while(UsedStrings.Contains(str));

            UsedStrings.Add(str);
            String = str;
        }

        // Prevent default constructor usage
        private UniqueString() { }

        // Prevent copy constructor usage
        private UniqueString(UniqueString copy) { }

        public override string ToString()
        {
            return String;
        }
    }
}
