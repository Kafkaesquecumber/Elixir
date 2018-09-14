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
using System.Collections.Generic;
using System.IO;

namespace Glaives.Core.Utils
{
    public static class IOUtils
    {
        /// <summary>
        /// Get a list of file paths for each file in the directory and all it's sub-directories recursively
        /// </summary>
        /// <param name="directory">The root directory</param>
        public static IEnumerable<string> EnumerateFilesInDirectoryResursively(string directory)
        {
            List<string> files = new List<string>();
            EnumerateFilesInDirectoryRecursively(directory, ref files);
            return files;
        }

        private static void EnumerateFilesInDirectoryRecursively(string directory, ref List<string> files)
        {
            foreach (string d in Directory.GetDirectories(directory))
            {
                foreach (string f in Directory.GetFiles(d))
                {
                    files.Add(f);
                }
                EnumerateFilesInDirectoryRecursively(d, ref files);
            }
        }
    }
}