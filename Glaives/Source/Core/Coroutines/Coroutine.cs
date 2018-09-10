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

using System.Collections;
using System.Diagnostics;

namespace Glaives.Core.Coroutines
{
    /// <summary>
    /// <para>A coroutine contains the routine and it's callbacks</para>
    /// <para>Also provides static utility methods for yield operations</para>
    /// </summary>
    public class Coroutine
    {
        /// <summary>
        /// The running routine
        /// </summary>
        public readonly IEnumerator Routine;

        /// <summary>
        /// The callback that gets invoked when the routine terminates or is stopped
        /// </summary>
        public readonly CoroutineCallbacks.CoroutineCompletedCallback CompletedCallback;

        internal Coroutine(IEnumerator routine, CoroutineCallbacks.CoroutineCompletedCallback completedCallback)
        {
            Routine = routine;
            CompletedCallback = completedCallback;
        }

        /// <summary>
        /// Yield return this method in a coroutine to make it wait for a specified amount of seconds
        /// before continuing past the yield 
        /// </summary>
        /// <param name="seconds">The amount of seconds to wait</param>
        public static IEnumerator WaitForSeconds(float seconds)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.Elapsed.TotalSeconds < seconds)
            {
                yield return null;
            }

            stopwatch.Stop();
        }

        /// <summary>
        /// Yield return this method in a coroutine to make it wait for a specified amount of ticks
        /// before continuing past the yield
        /// </summary>
        /// <param name="ticks">The amount of ticks to wait</param>
        public static IEnumerator WaitForTicks(uint ticks)
        {
            int counter = 0;
            while (true)
            {
                // Increment first because yield break is still a yield 
                counter++; 

                if (counter == ticks)
                {
                    yield break;
                }
                
                yield return null;
            }
        }
    }
}