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

using System;
using Elixir.GameFramework;
using Elixir.Graphics;
using Elixir.Internal.Interface;
using Elixir.Internal.Graphics;

namespace Elixir.Internal.Windowing
{
    internal sealed class Window
    {
        internal IntPtr Handle { get; private set; }
        
        private IntVector2 _size;
        internal IntVector2 Size
        {
            get => _size;
            set
            {
                Interface.ResizeWindow(value.X, value.Y);
                Interface.GetWindowSize(out int newWidth, out int newHeight);
                _size = new IntVector2(newWidth, newHeight);
            }
        }
        
        private string _title;
        internal string Title
        {
            get => _title;
            set
            {
                Interface.SetWindowTitle(value);
                _title = value;
            }
        }

        private bool _vSync;
        internal bool VSync
        {
            get => _vSync;
            set
            {
                if (_vSync != value)
                {
                    _vSync = value;
                    Interface.SetVSyncEnabled(value);
                }
            }
        }

        internal Viewport Viewport { get; private set; }
        
        internal string OpenGlVersion => Interface.GetCurrentGraphicsApiVersion();
        internal readonly WindowInterface Interface;
        
        internal Window(IntVector2 size, string title)
        {
            Interface = (WindowInterface)InterfaceManager.CreateInterface(InterfaceType.Window);
            _size = size;   // set private member on purpose
            _title = title; // set private member on purpose
            
            Viewport = new Viewport();

            Handle = Interface.OpenWindow(Size, title);
            Interface.SetVSyncEnabled(VSync);
        }
        
        internal void Center()
        {
            Interface.Center();
        }        
    }
}
