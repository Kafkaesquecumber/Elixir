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

using System.Diagnostics;
using Glaives.Internal.Interface;

namespace Glaives.Internal.Timing
{
    internal class EngineTimer
    {
        internal double DeltaTime { get; private set; }
        internal int FramesPerSecond { get; private set; }
        internal bool FirstIteration { get; private set; }

        private Stopwatch _fpsTimer;
        private int _frames;
        private ulong _lastFrame;
        private readonly WindowInterface _windowInterface;

        internal EngineTimer(WindowInterface windowInterface)
        {
            _windowInterface = windowInterface;
        }

        internal void Initialize()
        {
            _fpsTimer = new Stopwatch();
            _fpsTimer.Start();
            _frames = 0;
            FirstIteration = true;
        }

        internal void CaptureFrameStartTime()
        {
            _frames++;
            if (_fpsTimer.Elapsed.TotalSeconds >= 1.0f)
            {
                FramesPerSecond = _frames;
                _frames = 0;
                _fpsTimer.Restart();
            }

            _lastFrame = _windowInterface.GetHighResTicks();
        }

        internal void Sleep()
        {
            FirstIteration = false;
            ulong now = _windowInterface.GetHighResTicks();
            double workTime = (double) ((now - _lastFrame) * 1000) / _windowInterface.GetFrequency();

            uint targetFps = Engine.Get.Settings.General.TargetFps;

            if (targetFps == 0) // 0 means unlimited, don't sleep
            {
                return;
            }

            double targetMsPerFrame = 1000 / (double)targetFps;
            double actualMsPerFrame = workTime;

            if (actualMsPerFrame < targetMsPerFrame)
            {
                uint sleep = (uint)(targetMsPerFrame - actualMsPerFrame);

                if (sleep > 0)
                {
                    _windowInterface.Sleep(sleep);
                }
            }
        }

        internal void RefreshDeltaTime()
        {
            ulong now = _windowInterface.GetHighResTicks();
            DeltaTime = ((double)(now - _lastFrame) / 1000000);
        }
    }
}
