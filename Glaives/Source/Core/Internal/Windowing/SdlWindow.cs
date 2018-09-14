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
using System.Collections.Generic;
using Glaives.Core.Graphics;
using Glaives.Core.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SDL2;
using Glaives.Core.Internal.Graphics;
using Glaives.Core.Internal;
using OpenTK.Input;
using Key = Glaives.Core.Input.Key;
using Mouse = Glaives.Core.Internal.Input.Mouse;

namespace Glaives.Core.Internal.Windowing
{
    internal class SdlWindow : NativeWindow
    {
        internal IntPtr NativeWindowHandle { get; private set; }
        internal IGraphicsContext GlContext { get; private set; }
        private bool _isOpen;

        private readonly Dictionary<int, IntPtr> _gamepads = new Dictionary<int, IntPtr>();
        private OpenTK.Platform.IWindowInfo _windowInfo;

        private static bool _isSdlInitialized;

        internal override bool IsOpen()
        {
            return _isOpen;
        }

        internal override IntPtr OpenWindow(IntVector2 size, string title)
        {
            // init check for multi-window purposes
            if (!_isSdlInitialized)
            {
                if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0)
                {
                    throw new GlaivesException($"Failed to initialize SDL: {SDL.SDL_GetError()}");
                }

                _isSdlInitialized = true;
            }

            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 2);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
            
            SDL.SDL_WindowFlags windowFlags= windowFlags = SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL |
                                                           SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN |
                                                           SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE;

            NativeWindowHandle = SDL.SDL_CreateWindow(title, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, size.X, size.Y, windowFlags);
            
            if (NativeWindowHandle == IntPtr.Zero)
            {
                throw new GlaivesException($"Failed to open SDL window: {SDL.SDL_GetError()}");
            }

            _isOpen = true;
            
            _windowInfo =
                OpenTK.Platform.Utilities.CreateSdl2WindowInfo(NativeWindowHandle);
            GraphicsMode graphicsMode = GraphicsMode.Default;
            
            GlContext = new GraphicsContext(graphicsMode, _windowInfo)
            {
                SwapInterval = 1
            };

            MakeContextCurrent();
            GlContext.LoadAll();

            string glVersion = GL.GetString(StringName.Version);
            int currentMajor = int.Parse(glVersion.Split('.')[0][0].ToString());
            int currentMinor = int.Parse(glVersion.Split('.')[1][0].ToString());

            if (currentMajor < 3)
            {
                throw new NotSupportedException("Your system does not support OpenGL 3.x");
            }
            else if (currentMajor == 3)
            {
                if (currentMinor < 2)
                {
                    throw new NotSupportedException("Your system does not support OpenGL 3.2");
                }
            }

            return NativeWindowHandle;
        }

        internal void MakeContextCurrent()
        {
            GlContext.MakeCurrent(_windowInfo);
        }

        internal override void PollEvents()
        {
            while (SDL.SDL_PollEvent(out SDL.SDL_Event ev) != 0)
            {
                switch (ev.type)
                {
                    case SDL.SDL_EventType.SDL_FIRSTEVENT:
                        break;
                    case SDL.SDL_EventType.SDL_QUIT:
                        break;
                    case SDL.SDL_EventType.SDL_WINDOWEVENT:
                        if (ev.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED)
                        {
                            GetWindowSize(out int w, out int h);
                            GetFrameBufferSize(out int fbW, out int fbH);
                            OnResized(new IntVector2(w, h), new IntVector2(fbW, fbH));
                        }
                        break;
                    case SDL.SDL_EventType.SDL_SYSWMEVENT:
                        break;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                    {
                        if (ev.key.repeat == 0)
                        {
                            OnInputActionEvent(KeyState.Pressed, ToKey((int) ev.key.keysym.scancode), 0);
                        }

                        break;
                    }
                    case SDL.SDL_EventType.SDL_KEYUP:
                        OnInputActionEvent(KeyState.Released, ToKey((int)ev.key.keysym.scancode), 0);
                        break;
                    case SDL.SDL_EventType.SDL_TEXTEDITING:
                        break;
                    case SDL.SDL_EventType.SDL_TEXTINPUT:
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEMOTION:
                        Mouse.Position = new Vector2(ev.motion.x, ev.motion.y);
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    {
                        if (ev.key.repeat == 0)
                        {
                            OnInputActionEvent(KeyState.Pressed, MouseButtonToKey(ev.button.button), 0);
                            Console.WriteLine(MouseButtonToKey(ev.button.button));
                        }
                        break;
                    }
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                    {
                        OnInputActionEvent(KeyState.Released, MouseButtonToKey(ev.button.button), 0);
                        break;
                    }
                    case SDL.SDL_EventType.SDL_MOUSEWHEEL:
                        break;
                    case SDL.SDL_EventType.SDL_JOYAXISMOTION:
                        OnInputAxisEvent((InputAxis)ev.jaxis.axis, ((float)ev.jaxis.axisValue / short.MaxValue), ev.jaxis.which);
                        break;
                    case SDL.SDL_EventType.SDL_JOYBALLMOTION: break;
                    case SDL.SDL_EventType.SDL_JOYHATMOTION: // sdl thought the d-pad should go here.
                        Key hatKey = JHatToKey(ev.jhat.hatValue);
                        // Can return unknown when the d-pad is pressed diagonally, we dont want to receive this behaviour
                        if (hatKey == Key.Unknown)
                        {
                            break;
                        }
                        OnInputActionEvent(KeyState.Pressed, hatKey, ev.jbutton.which);
                        break;
                    case SDL.SDL_EventType.SDL_JOYBUTTONDOWN:
                        OnInputActionEvent(KeyState.Pressed, JButtonToKey(ev.jbutton.button), ev.jbutton.which);
                        break;
                    case SDL.SDL_EventType.SDL_JOYBUTTONUP:
                        OnInputActionEvent(KeyState.Released, JButtonToKey(ev.jbutton.button), ev.jbutton.which);
                        break;
                    case SDL.SDL_EventType.SDL_JOYDEVICEADDED:
                    {
                        int deviceIndex = _gamepads.Count;
                        _gamepads.Add(deviceIndex, SDL.SDL_JoystickOpen(deviceIndex));
                        string jdeviceName = SDL.SDL_JoystickNameForIndex(deviceIndex);
                        Engine.Get.Debug.Info($"Gamepad connected: {jdeviceName}");
                        break;
                    }
                    case SDL.SDL_EventType.SDL_JOYDEVICEREMOVED:
                        int index = ev.jdevice.which;
                        SDL.SDL_JoystickClose(_gamepads[index]);
                        _gamepads.Remove(index);
                        Engine.Get.Debug.Info("Gamepad disconnected");
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERAXISMOTION:
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERBUTTONDOWN:
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERBUTTONUP:
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERDEVICEADDED:
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
                        break;
                    case SDL.SDL_EventType.SDL_CONTROLLERDEVICEREMAPPED:
                        break;
                    case SDL.SDL_EventType.SDL_FINGERDOWN:
                        break;
                    case SDL.SDL_EventType.SDL_FINGERUP:
                        break;
                    case SDL.SDL_EventType.SDL_FINGERMOTION:
                        break;
                    case SDL.SDL_EventType.SDL_DOLLARGESTURE:
                        break;
                    case SDL.SDL_EventType.SDL_DOLLARRECORD:
                        break;
                    case SDL.SDL_EventType.SDL_MULTIGESTURE:
                        break;
                    case SDL.SDL_EventType.SDL_CLIPBOARDUPDATE:
                        break;
                    case SDL.SDL_EventType.SDL_DROPFILE:
                        break;
                    case SDL.SDL_EventType.SDL_DROPTEXT:
                        break;
                    case SDL.SDL_EventType.SDL_DROPBEGIN:
                        break;
                    case SDL.SDL_EventType.SDL_DROPCOMPLETE:
                        break;
                    case SDL.SDL_EventType.SDL_AUDIODEVICEADDED:
                        break;
                    case SDL.SDL_EventType.SDL_AUDIODEVICEREMOVED:
                        break;
                    case SDL.SDL_EventType.SDL_RENDER_TARGETS_RESET:
                        break;
                    case SDL.SDL_EventType.SDL_RENDER_DEVICE_RESET:
                        break;
                    case SDL.SDL_EventType.SDL_USEREVENT:
                        break;
                    case SDL.SDL_EventType.SDL_LASTEVENT:
                        break;
                    default:
                        break;
                }
            }
        }

        internal override void Center()
        {
            if (NativeWindowHandle != IntPtr.Zero)
            {
                SDL.SDL_SetWindowPosition(NativeWindowHandle, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED);
            }
        }

        internal override void SetVSyncEnabled(bool enabled)
        {
            GlContext.SwapInterval = enabled ? 1 : 0;
        }

        internal override void Clear(Color color)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(color.ToOpenTKColor());
        }

        internal override void CloseWindow()
        {
            _isOpen = false;
        }

        internal override ulong GetHighResTicks()
        {
            return SDL.SDL_GetPerformanceCounter();
        }

        internal override ulong GetFrequency()
        {
            return SDL.SDL_GetPerformanceFrequency();
        }

        internal override void Sleep(uint ms)
        {
            SDL.SDL_Delay(ms);
        }

        internal override string GetCurrentGraphicsApiVersion()
        {
            return GL.GetString(StringName.Version);
        }

        internal override void GetFrameBufferSize(out int width, out int height)
        {
            if (NativeWindowHandle != IntPtr.Zero)
            {
                SDL.SDL_GL_GetDrawableSize(NativeWindowHandle, out width, out height);
            }
            else
            {
                width = 0;
                height = 0;
            }
        }

        internal override IntVector2 GetPosition()
        {
            if (NativeWindowHandle != IntPtr.Zero)
            {
                SDL.SDL_GetWindowPosition(NativeWindowHandle, out int x, out int y);
                return new IntVector2(x, y);
            }
            return IntVector2.Zero;
        }

        internal override Func<string, IntPtr> GetProcAddress()
        {
            return GetProcAddress;
        }

        private IntPtr GetProcAddress(string arg)
        {
            return SDL.SDL_GL_GetProcAddress(arg);
        }

        internal override void GetWindowSize(out int width, out int height)
        {
            if (NativeWindowHandle != IntPtr.Zero)
            {
                SDL.SDL_GetWindowSize(NativeWindowHandle, out width, out height);
            }
            else
            {
                width = 0;
                height = 0;
            }
        }
        
        internal override void ResizeWindow(int width, int height)
        {
            if (NativeWindowHandle != IntPtr.Zero)
            {
                SDL.SDL_SetWindowSize(NativeWindowHandle, width, height);
            }
        }

        internal override void SetPosition(IntVector2 position)
        {
            if (NativeWindowHandle != IntPtr.Zero)
            {
                SDL.SDL_SetWindowPosition(NativeWindowHandle, position.X, position.Y);
            }
        }

        internal override void SetWindowTitle(string title)
        {
            if (NativeWindowHandle != IntPtr.Zero)
            {
                SDL.SDL_SetWindowTitle(NativeWindowHandle, title);
            }
        }

        internal override void Swap()
        {
            if (!GlContext.IsDisposed)
            {
                GlContext.SwapBuffers();
            }
        }

        internal override void Terminate()
        {
            GlContext.Dispose();
            SDL.SDL_DestroyWindow(NativeWindowHandle);
            //SDL.SDL_Quit(); this is not window specific (will break the multi-window editor) where should we put this??
        }

        private Key MouseButtonToKey(byte button)
        {
            switch (button)
            {
                case 1: return Key.LeftMouse;
                case 2: return Key.MiddleMouse;
                case 3: return Key.RightMouse;
                case 4: return Key.XMouse1;
                case 5: return Key.XMouse2;
                default:
                    return Key.Unknown;
            }
        }

        private Key JHatToKey(byte jhat)
        {
            switch (jhat)
            {
                case 1: return Key.GamepadDPadUp;
                case 2: return Key.GamepadDPadRight;
                case 4: return Key.GamepadDPadDown;
                case 8: return Key.GamepadDPadLeft;
                default:
                    return Key.Unknown;
            }
        }

        private Key JButtonToKey(byte jbutton)
        {
            switch (jbutton)
            {
                case 0: return Key.GamepadFaceButtonBottom;
                case 1: return Key.GamepadFaceButtonRight;
                case 2: return Key.GamepadFaceButtonLeft;
                case 3: return Key.GamepadFaceButtonTop;
                case 4: return Key.GamepadLeftShoulder;
                case 5: return Key.GamepadRightShoulder;
                case 6: return Key.GamepadSelect;
                case 7: return Key.GamepadStart;
                case 8: return Key.GamepadRightThumbstick;
                case 9: return Key.GamepadLeftThumbstick;
                default:
                    return Key.Unknown;
            }
        }

        internal override Key ToKey(int nativeKey)
        {
            SDL.SDL_Scancode sdlScancode = (SDL.SDL_Scancode)nativeKey;
            switch (sdlScancode)
            {
                case SDL.SDL_Scancode.SDL_SCANCODE_UNKNOWN:             return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_A:                   return Key.A;
                case SDL.SDL_Scancode.SDL_SCANCODE_B:                   return Key.B;
                case SDL.SDL_Scancode.SDL_SCANCODE_C:                   return Key.C;
                case SDL.SDL_Scancode.SDL_SCANCODE_D:                   return Key.D;
                case SDL.SDL_Scancode.SDL_SCANCODE_E:                   return Key.E;
                case SDL.SDL_Scancode.SDL_SCANCODE_F:                   return Key.F;
                case SDL.SDL_Scancode.SDL_SCANCODE_G:                   return Key.G;
                case SDL.SDL_Scancode.SDL_SCANCODE_H:                   return Key.H;
                case SDL.SDL_Scancode.SDL_SCANCODE_I:                   return Key.I;
                case SDL.SDL_Scancode.SDL_SCANCODE_J:                   return Key.J;
                case SDL.SDL_Scancode.SDL_SCANCODE_K:                   return Key.K;
                case SDL.SDL_Scancode.SDL_SCANCODE_L:                   return Key.L;
                case SDL.SDL_Scancode.SDL_SCANCODE_M:                   return Key.M;
                case SDL.SDL_Scancode.SDL_SCANCODE_N:                   return Key.N;
                case SDL.SDL_Scancode.SDL_SCANCODE_O:                   return Key.O;
                case SDL.SDL_Scancode.SDL_SCANCODE_P:                   return Key.P;
                case SDL.SDL_Scancode.SDL_SCANCODE_Q:                   return Key.Q;
                case SDL.SDL_Scancode.SDL_SCANCODE_R:                   return Key.R;
                case SDL.SDL_Scancode.SDL_SCANCODE_S:                   return Key.S;
                case SDL.SDL_Scancode.SDL_SCANCODE_T:                   return Key.T;
                case SDL.SDL_Scancode.SDL_SCANCODE_U:                   return Key.U;
                case SDL.SDL_Scancode.SDL_SCANCODE_V:                   return Key.V;
                case SDL.SDL_Scancode.SDL_SCANCODE_W:                   return Key.W;
                case SDL.SDL_Scancode.SDL_SCANCODE_X:                   return Key.X;
                case SDL.SDL_Scancode.SDL_SCANCODE_Y:                   return Key.Y;
                case SDL.SDL_Scancode.SDL_SCANCODE_Z:                   return Key.Z;
                case SDL.SDL_Scancode.SDL_SCANCODE_1:                   return Key.Key1;
                case SDL.SDL_Scancode.SDL_SCANCODE_2:                   return Key.Key2;
                case SDL.SDL_Scancode.SDL_SCANCODE_3:                   return Key.Key3;
                case SDL.SDL_Scancode.SDL_SCANCODE_4:                   return Key.Key4;
                case SDL.SDL_Scancode.SDL_SCANCODE_5:                   return Key.Key5;
                case SDL.SDL_Scancode.SDL_SCANCODE_6:                   return Key.Key6;
                case SDL.SDL_Scancode.SDL_SCANCODE_7:                   return Key.Key7;
                case SDL.SDL_Scancode.SDL_SCANCODE_8:                   return Key.Key8;
                case SDL.SDL_Scancode.SDL_SCANCODE_9:                   return Key.Key9;
                case SDL.SDL_Scancode.SDL_SCANCODE_0:                   return Key.Key0;
                case SDL.SDL_Scancode.SDL_SCANCODE_RETURN:              return Key.Enter;
                case SDL.SDL_Scancode.SDL_SCANCODE_ESCAPE:              return Key.Escape;
                case SDL.SDL_Scancode.SDL_SCANCODE_BACKSPACE:           return Key.Backspace;
                case SDL.SDL_Scancode.SDL_SCANCODE_TAB:                 return Key.Tab;
                case SDL.SDL_Scancode.SDL_SCANCODE_SPACE:               return Key.Space;
                case SDL.SDL_Scancode.SDL_SCANCODE_MINUS:               return Key.Minus;
                case SDL.SDL_Scancode.SDL_SCANCODE_EQUALS:              return Key.Equal;
                case SDL.SDL_Scancode.SDL_SCANCODE_LEFTBRACKET:         return Key.LeftBracket;
                case SDL.SDL_Scancode.SDL_SCANCODE_RIGHTBRACKET:        return Key.RightBracket;
                case SDL.SDL_Scancode.SDL_SCANCODE_BACKSLASH:           return Key.Backslash;
                case SDL.SDL_Scancode.SDL_SCANCODE_NONUSHASH:           return Key.Backslash;
                case SDL.SDL_Scancode.SDL_SCANCODE_SEMICOLON:           return Key.SemiColon;
                case SDL.SDL_Scancode.SDL_SCANCODE_APOSTROPHE:          return Key.Quote;
                case SDL.SDL_Scancode.SDL_SCANCODE_GRAVE:               return Key.Tilde;
                case SDL.SDL_Scancode.SDL_SCANCODE_COMMA:               return Key.Comma;
                case SDL.SDL_Scancode.SDL_SCANCODE_PERIOD:              return Key.Period;
                case SDL.SDL_Scancode.SDL_SCANCODE_SLASH:               return Key.Slash;
                case SDL.SDL_Scancode.SDL_SCANCODE_CAPSLOCK:            return Key.CapsLock;
                case SDL.SDL_Scancode.SDL_SCANCODE_F1:                  return Key.F1;
                case SDL.SDL_Scancode.SDL_SCANCODE_F2:                  return Key.F2;
                case SDL.SDL_Scancode.SDL_SCANCODE_F3:                  return Key.F3;
                case SDL.SDL_Scancode.SDL_SCANCODE_F4:                  return Key.F4;
                case SDL.SDL_Scancode.SDL_SCANCODE_F5:                  return Key.F5;
                case SDL.SDL_Scancode.SDL_SCANCODE_F6:                  return Key.F6;
                case SDL.SDL_Scancode.SDL_SCANCODE_F7:                  return Key.F7;
                case SDL.SDL_Scancode.SDL_SCANCODE_F8:                  return Key.F8;
                case SDL.SDL_Scancode.SDL_SCANCODE_F9:                  return Key.F9;
                case SDL.SDL_Scancode.SDL_SCANCODE_F10:                 return Key.F10;
                case SDL.SDL_Scancode.SDL_SCANCODE_F11:                 return Key.F11;
                case SDL.SDL_Scancode.SDL_SCANCODE_F12:                 return Key.F12;
                case SDL.SDL_Scancode.SDL_SCANCODE_PRINTSCREEN:         return Key.PrintScreen;
                case SDL.SDL_Scancode.SDL_SCANCODE_SCROLLLOCK:          return Key.ScrollLock;
                case SDL.SDL_Scancode.SDL_SCANCODE_PAUSE:               return Key.Pause;
                case SDL.SDL_Scancode.SDL_SCANCODE_INSERT:              return Key.Insert;
                case SDL.SDL_Scancode.SDL_SCANCODE_HOME:                return Key.Home;
                case SDL.SDL_Scancode.SDL_SCANCODE_PAGEUP:              return Key.PageUp;
                case SDL.SDL_Scancode.SDL_SCANCODE_DELETE:              return Key.Delete;
                case SDL.SDL_Scancode.SDL_SCANCODE_END:                 return Key.End;
                case SDL.SDL_Scancode.SDL_SCANCODE_PAGEDOWN:            return Key.PageDown;
                case SDL.SDL_Scancode.SDL_SCANCODE_RIGHT:               return Key.Right;
                case SDL.SDL_Scancode.SDL_SCANCODE_LEFT:                return Key.Left;
                case SDL.SDL_Scancode.SDL_SCANCODE_DOWN:                return Key.Down;
                case SDL.SDL_Scancode.SDL_SCANCODE_UP:                  return Key.Up;
                case SDL.SDL_Scancode.SDL_SCANCODE_NUMLOCKCLEAR:        return Key.NumLock;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_DIVIDE:           return Key.NumpadDivide;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_MULTIPLY:         return Key.NumpadMultiply;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_MINUS:            return Key.NumpadSubtract;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_PLUS:             return Key.NumpadAdd;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_ENTER:            return Key.NumpadEnter;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_1:                return Key.Numpad1;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_2:                return Key.Numpad2;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_3:                return Key.Numpad3;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_4:                return Key.Numpad4;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_5:                return Key.Numpad5;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_6:                return Key.Numpad6;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_7:                return Key.Numpad7;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_8:                return Key.Numpad8;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_9:                return Key.Numpad9;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_0:                return Key.Numpad0;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_PERIOD:           return Key.NumpadDecimal;
                case SDL.SDL_Scancode.SDL_SCANCODE_NONUSBACKSLASH:      return Key.GraveAccent;
                case SDL.SDL_Scancode.SDL_SCANCODE_APPLICATION:         return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_POWER:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_EQUALS:           return Key.NumpadEqual;
                case SDL.SDL_Scancode.SDL_SCANCODE_F13:                 return Key.F13;
                case SDL.SDL_Scancode.SDL_SCANCODE_F14:                 return Key.F14;
                case SDL.SDL_Scancode.SDL_SCANCODE_F15:                 return Key.F15;
                case SDL.SDL_Scancode.SDL_SCANCODE_F16:                 return Key.F16;
                case SDL.SDL_Scancode.SDL_SCANCODE_F17:                 return Key.F17;
                case SDL.SDL_Scancode.SDL_SCANCODE_F18:                 return Key.F18;
                case SDL.SDL_Scancode.SDL_SCANCODE_F19:                 return Key.F19;
                case SDL.SDL_Scancode.SDL_SCANCODE_F20:                 return Key.F20;
                case SDL.SDL_Scancode.SDL_SCANCODE_F21:                 return Key.F21;
                case SDL.SDL_Scancode.SDL_SCANCODE_F22:                 return Key.F22;
                case SDL.SDL_Scancode.SDL_SCANCODE_F23:                 return Key.F23;
                case SDL.SDL_Scancode.SDL_SCANCODE_F24:                 return Key.F24;
                case SDL.SDL_Scancode.SDL_SCANCODE_EXECUTE:             return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_HELP:                return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_MENU:                return Key.Menu;
                case SDL.SDL_Scancode.SDL_SCANCODE_SELECT:              return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_STOP:                return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_AGAIN:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_UNDO:                return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_CUT:                 return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_COPY:                return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_PASTE:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_FIND:                return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_MUTE:                return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_VOLUMEUP:            return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_VOLUMEDOWN:          return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_COMMA:            return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_EQUALSAS400:      return Key.NumpadEqual;
                case SDL.SDL_Scancode.SDL_SCANCODE_INTERNATIONAL1:      return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_INTERNATIONAL2:      return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_INTERNATIONAL3:      return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_INTERNATIONAL4:      return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_INTERNATIONAL5:      return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_INTERNATIONAL6:      return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_INTERNATIONAL7:      return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_INTERNATIONAL8:      return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_INTERNATIONAL9:      return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_LANG1:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_LANG2:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_LANG3:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_LANG4:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_LANG5:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_LANG6:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_LANG7:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_LANG8:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_LANG9:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_ALTERASE:            return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_SYSREQ:              return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_CANCEL:              return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_CLEAR:               return Key.NumpadEqual;
                case SDL.SDL_Scancode.SDL_SCANCODE_PRIOR:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_RETURN2:             return Key.Enter;
                case SDL.SDL_Scancode.SDL_SCANCODE_SEPARATOR:           return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_OUT:                 return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_OPER:                return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_CLEARAGAIN:          return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_CRSEL:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_EXSEL:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_00:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_000:              return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_THOUSANDSSEPARATOR:  return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_DECIMALSEPARATOR:    return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_CURRENCYUNIT:        return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_CURRENCYSUBUNIT:     return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_LEFTPAREN:        return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_RIGHTPAREN:       return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_LEFTBRACE:        return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_RIGHTBRACE:       return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_TAB:              return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_BACKSPACE:        return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_A:                return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_B:                return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_C:                return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_D:                return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_E:                return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_F:                return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_XOR:              return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_POWER:            return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_PERCENT:          return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_LESS:             return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_GREATER:          return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_AMPERSAND:        return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_DBLAMPERSAND:     return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_VERTICALBAR:      return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_DBLVERTICALBAR:   return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_COLON:            return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_HASH:             return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_SPACE:            return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_AT:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_EXCLAM:           return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_MEMSTORE:         return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_MEMRECALL:        return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_MEMCLEAR:         return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_MEMADD:           return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_MEMSUBTRACT:      return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_MEMMULTIPLY:      return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_MEMDIVIDE:        return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_PLUSMINUS:        return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_CLEAR:            return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_CLEARENTRY:       return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_BINARY:           return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_OCTAL:            return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_DECIMAL:          return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KP_HEXADECIMAL:      return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_LCTRL:               return Key.LeftControl;
                case SDL.SDL_Scancode.SDL_SCANCODE_LSHIFT:              return Key.LeftShift;
                case SDL.SDL_Scancode.SDL_SCANCODE_LALT:                return Key.LeftAlt;
                case SDL.SDL_Scancode.SDL_SCANCODE_LGUI:                return Key.LeftSuper;
                case SDL.SDL_Scancode.SDL_SCANCODE_RCTRL:               return Key.RightControl;
                case SDL.SDL_Scancode.SDL_SCANCODE_RSHIFT:              return Key.RightShift;
                case SDL.SDL_Scancode.SDL_SCANCODE_RALT:                return Key.RightAlt;
                case SDL.SDL_Scancode.SDL_SCANCODE_RGUI:                return Key.RightSuper;
                case SDL.SDL_Scancode.SDL_SCANCODE_MODE:                return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_AUDIONEXT:           return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_AUDIOPREV:           return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_AUDIOSTOP:           return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_AUDIOPLAY:           return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_AUDIOMUTE:           return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_MEDIASELECT:         return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_WWW:                 return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_MAIL:                return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_CALCULATOR:          return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_COMPUTER:            return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_AC_SEARCH:           return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_AC_HOME:             return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_AC_BACK:             return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_AC_FORWARD:          return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_AC_STOP:             return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_AC_REFRESH:          return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_AC_BOOKMARKS:        return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_BRIGHTNESSDOWN:      return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_BRIGHTNESSUP:        return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_DISPLAYSWITCH:       return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KBDILLUMTOGGLE:      return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KBDILLUMDOWN:        return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_KBDILLUMUP:          return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_EJECT:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_SLEEP:               return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_APP1:                return Key.Unknown;
                case SDL.SDL_Scancode.SDL_SCANCODE_APP2:                return Key.Unknown;
                case SDL.SDL_Scancode.SDL_NUM_SCANCODES:                return Key.Unknown;
                default:
                    return Key.Unknown;
            }
        }
    }
}
