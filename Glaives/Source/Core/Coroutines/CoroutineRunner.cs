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
using System.Collections.Generic;
using System.Linq;

namespace Glaives.Core.Coroutines
{
    /// <summary>
    /// Runs and stops coroutines
    /// </summary>
    public class CoroutineRunner
    {
        private readonly List<Coroutine> _coroutines = new List<Coroutine>();

        /// <summary>
        /// The coroutines that are currently running on this coroutine runner
        /// </summary>
        public IEnumerable<Coroutine> RunningCoroutines => _coroutines;

        /// <summary>
        /// Run a new coroutine
        /// </summary>
        /// <param name="routine">The routine to run</param>
        /// <param name="completedCallback">The callback will be called after the routine has either terminates or is stopped</param>
        public void RunCoroutine(IEnumerator routine, CoroutineCallbacks.CoroutineCompletedCallback completedCallback = null)
        {
            _coroutines.Add(new Coroutine(routine, completedCallback));
        }

        /// <summary>
        /// Stop a running routine
        /// </summary>
        /// <param name="routine">The routine to stop (does nothing if the routine is not running)</param>
        public void StopCoroutine(IEnumerator routine)
        {
            Coroutine coroutine = _coroutines.FirstOrDefault(x => x.Routine == routine);
            if (coroutine == null)
            {
                return;
            }

            int i = _coroutines.IndexOf(coroutine);
            _coroutines.RemoveAt(i);
        }

        /// <summary>
        /// Stop all coroutines associated with this coroutine runner
        /// </summary>
        public void StopAllCoroutines()
        {
            _coroutines.Clear();
        }

        internal void Update()
        {
            for (int i = 0; i < _coroutines.Count; i++)
            {
                if (_coroutines[i].Routine.Current is IEnumerator)
                {
                    if (MoveNext((IEnumerator) _coroutines[i].Routine.Current))
                    {
                        continue;
                    }
                }
                
                if (!_coroutines[i].Routine.MoveNext())
                {
                    _coroutines[i].CompletedCallback?.Invoke();
                    _coroutines.RemoveAt(i);
                    i--;
                }
            }
        }

        private bool MoveNext(IEnumerator routine)
        {
            if (routine.Current is IEnumerator enumerator)
            {
                if (MoveNext(enumerator))
                {
                    return true;
                }
            }

            return routine.MoveNext();
        }
    }
}