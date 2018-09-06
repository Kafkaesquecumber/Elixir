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
using Glaives.GameFramework;
using Glaives.Input;

namespace Glaives.Internal.Interface
{
    internal abstract class WindowInterface : Interface
    {
        internal event Action<IntVector2, IntVector2> Resized = delegate { };
        internal event Action<KeyState, Key, int> InputActionEvent = delegate { };
        internal event Action<InputAxis, float, int> InputAxisEvent = delegate { };
        internal abstract IntPtr OpenWindow(IntVector2 size, string title);
        internal abstract void ResizeWindow(int width, int height);
        internal abstract void GetWindowSize(out int width, out int height);
        internal abstract void SetWindowTitle(string title);
        internal abstract void GetFrameBufferSize(out int width, out int height);
        internal abstract Func<string, IntPtr> GetProcAddress();
        internal abstract bool IsOpen();
        internal abstract void Swap();
        internal abstract void SetVSyncEnabled(bool enabled);
        internal abstract void Clear(Color color);
        internal abstract void PollEvents();
        internal abstract void CloseWindow();
        internal abstract ulong GetHighResTicks();
        internal abstract ulong GetFrequency();
        internal abstract void Sleep(uint ms);
        internal abstract void Terminate();
        internal abstract string GetCurrentGraphicsApiVersion();
        internal abstract void SetPosition(IntVector2 position);
        internal abstract IntVector2 GetPosition();
        internal abstract void Center();
        internal abstract Key ToKey(int nativeKey);

        protected void OnResized(IntVector2 windowSize, IntVector2 viewportSize)
        {
            Resized(windowSize, viewportSize);
        }

        protected void OnInputActionEvent(KeyState keyState, Key key, int gamepadId)
        {
            InputActionEvent(keyState, key, gamepadId);
        }

        protected void OnInputAxisEvent(InputAxis inputAxis, float value, int gamepadId)
        {
            InputAxisEvent(inputAxis, value, gamepadId);
        }
    }
}
