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
using System.Reflection;
using Glaives.GameFramework;

namespace Glaives.Configuration
{
    [Serializable]
    public class VideoSettings
    {
        private int _width = 800;
        /// <summary>
        /// The width of the window
        /// </summary>
        public int Width
        {
            get => _width;
            set
            {
                if (value != _width)
                {
                    _width = value;
                    if (Engine.Get.Initialized)
                    {
                        Engine.Get.Window.Size = new IntVector2(value, Height);
                    }
                }
            }
        }

        private int _height = 600;
        /// <summary>
        /// The height of the window
        /// </summary>
        public int Height
        {
            get => _height;
            set
            {
                if (_height != value)
                {
                    _height = value;
                    if (Engine.Get.Initialized)
                    {
                        Engine.Get.Window.Size = new IntVector2(Width, value);
                    }
                }
            }
        }

        private string _title;
        /// <summary>
        /// The title of the window
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    if (Engine.Get.Initialized)
                    {
                        Engine.Get.Window.Title = value;
                    }
                }
            }
        }

        private bool _vsync;
        /// <summary>
        /// Whether or not the window should use vertical synchronization
        /// </summary>
        public bool VSync
        {
            get => _vsync;
            set
            {
                if (_vsync != value)
                {
                    _vsync = value;
                    if (Engine.Get.Initialized)
                    {
                        Engine.Get.Window.VSync = value;
                    }
                }
            }
        }
        
        public VideoSettings()
        {
            Title = Assembly.GetEntryAssembly().GetName().Name;
        }
    }
}
