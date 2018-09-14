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
using System.Collections.Generic;
using Glaives.Core.Diagnostics;
using Glaives.Core.Graphics;
using ImGuiNET;

namespace Glaives.Core.Internal.EditorGui
{
    internal class ImGuiConsole : ImGuiWindow
    {
        private readonly List<Log> _logs = new List<Log>();
        private int _prevLogCount;
        private byte[] _inputTextBuffer;

        internal ImGuiConsole()
        {
            Engine.Get.Debug.OnLog += OnLogReceived;
            _inputTextBuffer = new byte[1024];
        }

        private void OnLogReceived(Log log)
        {
            _logs.Add(log);
        }

        internal void Draw()
        {
            
            if (!IsOpen)
            {
                return;
            }

            Engine.Get.Window.GetFrameBufferSize(out int w, out int h);
            Vector2 size = new Vector2(w, (float) h / 4);

            ImGui.SetNextWindowPos(new Vector2(0, h - size.Y), Condition.Always);

            ImGui.PushStyleVar(StyleVar.Alpha, 0.5f);            
            ImGui.BeginWindow("Console", ref IsOpen, size, WindowFlags.NoMove | WindowFlags.NoResize | WindowFlags.NoCollapse | WindowFlags.NoScrollbar);
            ImGui.PopStyleVar();

            ImGui.SetWindowSize(size, Condition.Always);

            float frameHeight = ImGui.GetWindowHeight();
            ImGui.BeginChild("Output", new Vector2(0, frameHeight - 55), false, WindowFlags.Default);

            foreach (Log log in _logs)
            {
                Color color = Color.White;
                switch (log.LogType)
                {
                    case LogType.Info:
                        break;
                    case LogType.Warning:
                        color = Color.Yellow;
                        break;
                    case LogType.Error:
                        color = Color.Red;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                ImGui.Text(log.Message, color);
            }
            
            // Scroll down if a new log appeared
            if (_logs.Count != _prevLogCount)
            {
                ImGui.SetScrollHere();
            }

            ImGui.EndChild();

            
            ImGui.PushItemWidth(size.X - 20);
            ImGui.PushStyleVar(StyleVar.Alpha, 0.6f);
            
            unsafe
            {
                ImGui.InputText("", _inputTextBuffer, 1024, InputTextFlags.Default, TextEditCallback);
            }
            
            ImGui.PopStyleVar();
            ImGui.PopItemWidth();
            
            ImGui.EndWindow();
       
            _prevLogCount = _logs.Count;
        }

        private unsafe int TextEditCallback(TextEditCallbackData* data)
        {
            
            return data->BufSize;
        }
    }
}