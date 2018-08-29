using System.Diagnostics;
using Elixir.Internal.Interface;

namespace Elixir.Internal.Timing
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
