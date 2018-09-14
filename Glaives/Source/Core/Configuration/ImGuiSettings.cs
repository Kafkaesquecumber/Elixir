// MIT License
// 
// Copyright (c) 2018 Glaives Game Engine.
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
using Glaives.Core.Internal.EditorGui;

namespace Glaives.Core.Configuration
{
    [Serializable]
    public class ImGuiSettings
    {
        private ImGuiTheme _theme = ImGuiTheme.Dark;
        /// <summary>
        /// The appearance style of ImGui
        /// </summary>
        public ImGuiTheme Theme
        {
            get => _theme;
            set
            {
                if (_theme != value)
                {
                    _theme = value;
                    if (Engine.Get.Initialized)
                    {
                        ImGuiInternal.SetupImGuiStyle(value, Alpha);
                    }
                }
            }
        }

        private float _alpha = 0.75f;
        /// <summary>
        /// The overall alpha used together with the theme
        /// </summary>
        public float Alpha
        {
            get => _alpha;
            set
            {
                if (_alpha != value)
                {
                    _alpha = value;
                    if (Engine.Get.Initialized)
                    {
                        ImGuiInternal.SetupImGuiStyle(Theme, value);
                    }
                }
            }
        }
    }
}