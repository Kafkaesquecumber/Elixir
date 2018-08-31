using Elixir.Graphics;
using System;
using Elixir.GameFramework;
using Elixir.Input;

namespace Elixir.Internal.Interface
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
            if(keyState == KeyState.Pressed)
                Console.WriteLine(key);
            InputActionEvent(keyState, key, gamepadId);
        }

        protected void OnInputAxisEvent(InputAxis inputAxis, float value, int gamepadId)
        {
            InputAxisEvent(inputAxis, value, gamepadId);
        }
    }
}
