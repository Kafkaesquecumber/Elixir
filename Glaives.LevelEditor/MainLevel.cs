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
using Glaives.Core;
using Glaives.Core.Graphics;
using ImGuiNET;

namespace Glaives.LevelEditor
{
    public class MainLevel : Level
    {
        /// <inheritdoc />
        protected override void LoadLevel()
        {
            Sprite s = new Sprite(Engine.Content.LoadTexture("Content/Textures/spooky.png", TextureCreateOptions.Smooth));
            s.Position = new Vector2(300, 300);
            
        }

        /// <inheritdoc />
        protected override void Tick(float deltaTime)
        {
            
        }

        float radians = (float)Math.PI / 4; // 45 deg
        Color color = Color.Red;
        /// <inheritdoc />
        protected override void OnImGui()
        {
            
            ImGui.BeginWindow("Window", WindowFlags.AlwaysAutoResize);
            if (ImGui.Button("Im a button", new Vector2(100, 60)))
            {
                Console.WriteLine("Button pressed!");
            }

            
            ImGui.ColorPicker4("Pick color", ref color);

            
            ImGui.SliderAngle("Angle", ref radians, 0.0f, 360.0f);
            ImGui.EndWindow();
        }
    }
}