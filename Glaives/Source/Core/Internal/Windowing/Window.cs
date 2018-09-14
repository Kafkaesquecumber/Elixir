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
using System.Runtime.CompilerServices;

namespace Glaives.Core.Internal.Windowing
{
    internal class Window : SdlWindow
    {
        internal IntPtr Handle { get; private set; }
        
        private IntVector2 _size;
        internal IntVector2 Size
        {
            get
            {
                GetWindowSize(out int newWidth, out int newHeight);
                _size = new IntVector2(newWidth, newHeight);
                return _size;
            }
            set
            {
                ResizeWindow(value.X, value.Y);
                GetWindowSize(out int newWidth, out int newHeight);
                _size = new IntVector2(newWidth, newHeight);
            }
        }

        private string _title;
        internal string Title
        {
            get => _title;
            set
            {
                SetWindowTitle(value);
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
                    SetVSyncEnabled(value);
                }
            }
        }

        internal Viewport Viewport { get; private set; }
        
        internal string OpenGlVersion => GetCurrentGraphicsApiVersion();
        
        internal Window(IntVector2 size, string title)
        {
            // setting/getting private properties on purpose since the window is not created yet

            _size = size;   
            _title = title; 
            
            Viewport = new Viewport();

            Handle = OpenWindow(_size, _title); 
            SetVSyncEnabled(VSync);
        }
        
    }
}
