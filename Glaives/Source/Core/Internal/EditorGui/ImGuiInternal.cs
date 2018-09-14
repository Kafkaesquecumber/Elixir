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
using System.Reflection;
using System.Runtime.InteropServices;
using Glaives.Core.Configuration;
using Glaives.Core.Graphics;
using ImGuiNET;

namespace Glaives.Core.Internal.EditorGui
{
    internal static class ImGuiInternal
    {
        private static Dictionary<string, Type> _levels = new Dictionary<string, Type>();
        private static bool _menuBarInitialized;

        private static ImGuiTextureEditor _textureEditor;
        private static ImGuiConsole _console;

        internal static void SetupImGuiStyle(ImGuiTheme theme, float alpha)
        {
            Style style = ImGui.GetStyle();
            
            // light style from Pacôme Danhiez (user itamago) https://github.com/ocornut/imgui/pull/511#issuecomment-175719267
            style.Alpha = 1.0f;
            style.FrameRounding = 3.0f;
            style.SetColor(ColorTarget.Text, ThemeColor(new Color(0.00f, 0.00f, 0.00f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.TextDisabled, ThemeColor(new Color(0.60f, 0.60f, 0.60f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.WindowBg, ThemeColor(new Color(0.94f, 0.94f, 0.94f, 0.94f), theme, alpha));
            style.SetColor(ColorTarget.ChildBg,  ThemeColor(new Color(0.00f, 0.00f, 0.00f, 0.00f), theme, alpha));
            style.SetColor(ColorTarget.PopupBg,  ThemeColor(new Color(1.00f, 1.00f, 1.00f, 0.94f), theme, alpha));
            style.SetColor(ColorTarget.Border,  ThemeColor(new Color(0.00f, 0.00f, 0.00f, 0.39f), theme, alpha));
            style.SetColor(ColorTarget.BorderShadow,  ThemeColor(new Color(1.00f, 1.00f, 1.00f, 0.10f), theme, alpha));
            style.SetColor(ColorTarget.FrameBg,  ThemeColor(new Color(1.00f, 1.00f, 1.00f, 0.94f), theme, alpha));
            style.SetColor(ColorTarget.FrameBgHovered,  ThemeColor(new Color(0.26f, 0.59f, 0.98f, 0.40f), theme, alpha));
            style.SetColor(ColorTarget.FrameBgActive,  ThemeColor(new Color(0.26f, 0.59f, 0.98f, 0.67f), theme, alpha));
            style.SetColor(ColorTarget.TitleBg,  ThemeColor(new Color(0.96f, 0.96f, 0.96f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.TitleBgCollapsed,  ThemeColor(new Color(1.00f, 1.00f, 1.00f, 0.51f), theme, alpha));
            style.SetColor(ColorTarget.TitleBgActive,  ThemeColor(new Color(0.82f, 0.82f, 0.82f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.MenuBarBg,  ThemeColor(new Color(0.86f, 0.86f, 0.86f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.ScrollbarBg,  ThemeColor(new Color(0.98f, 0.98f, 0.98f, 0.53f), theme, alpha));
            style.SetColor(ColorTarget.ScrollbarGrab,  ThemeColor(new Color(0.69f, 0.69f, 0.69f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.ScrollbarGrabHovered,  ThemeColor(new Color(0.59f, 0.59f, 0.59f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.ScrollbarGrabActive,  ThemeColor(new Color(0.49f, 0.49f, 0.49f, 1.00f), theme, alpha));
            //style.SetColor(ColorTarget.Combo,  ThemeColor(new Color(0.86f, 0.86f, 0.86f, 0.99f), theme, alpha));
            style.SetColor(ColorTarget.CheckMark,  ThemeColor(new Color(0.26f, 0.59f, 0.98f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.SliderGrab,  ThemeColor(new Color(0.24f, 0.52f, 0.88f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.SliderGrabActive,  ThemeColor(new Color(0.26f, 0.59f, 0.98f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.Button,  ThemeColor(new Color(0.26f, 0.59f, 0.98f, 0.40f), theme, alpha));
            style.SetColor(ColorTarget.ButtonHovered,  ThemeColor(new Color(0.26f, 0.59f, 0.98f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.ButtonActive,  ThemeColor(new Color(0.06f, 0.53f, 0.98f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.Header,  ThemeColor(new Color(0.26f, 0.59f, 0.98f, 0.31f), theme, alpha));
            style.SetColor(ColorTarget.HeaderHovered,  ThemeColor(new Color(0.26f, 0.59f, 0.98f, 0.80f), theme, alpha));
            style.SetColor(ColorTarget.HeaderActive,  ThemeColor(new Color(0.26f, 0.59f, 0.98f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.Separator,  ThemeColor(new Color(0.39f, 0.39f, 0.39f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.SeparatorHovered,  ThemeColor(new Color(0.26f, 0.59f, 0.98f, 0.78f), theme, alpha));
            style.SetColor(ColorTarget.SeparatorActive,  ThemeColor(new Color(0.26f, 0.59f, 0.98f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.ResizeGrip,  ThemeColor(new Color(1.00f, 1.00f, 1.00f, 0.50f), theme, alpha));
            style.SetColor(ColorTarget.ResizeGripHovered,  ThemeColor(new Color(0.26f, 0.59f, 0.98f, 0.67f), theme, alpha));
            style.SetColor(ColorTarget.ResizeGripActive,  ThemeColor(new Color(0.26f, 0.59f, 0.98f, 0.95f), theme, alpha));
            style.SetColor(ColorTarget.CloseButton,  ThemeColor(new Color(0.59f, 0.59f, 0.59f, 0.50f), theme, alpha));
            style.SetColor(ColorTarget.CloseButtonHovered,  ThemeColor(new Color(0.98f, 0.39f, 0.36f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.CloseButtonActive,  ThemeColor(new Color(0.98f, 0.39f, 0.36f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.PlotLines,  ThemeColor(new Color(0.39f, 0.39f, 0.39f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.PlotLinesHovered,  ThemeColor(new Color(1.00f, 0.43f, 0.35f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.PlotHistogram,  ThemeColor(new Color(0.90f, 0.70f, 0.00f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.PlotHistogramHovered,  ThemeColor(new Color(1.00f, 0.60f, 0.00f, 1.00f), theme, alpha));
            style.SetColor(ColorTarget.TextSelectedBg,  ThemeColor(new Color(0.26f, 0.59f, 0.98f, 0.35f), theme, alpha));
            style.SetColor(ColorTarget.ModalWindowDarkening,  ThemeColor(new Color(0.20f, 0.20f, 0.20f, 0.35f), theme, alpha));
        }

        private static Color ThemeColor(Color color, ImGuiTheme theme, float alpha)
        {
            switch (theme)
            {
                case ImGuiTheme.Dark:
                {
                    ImGui.ColorConvertRGBToHSV(color.R, color.G, color.B, out var h, out var s, out var v);

                    if (s < 0.1f)
                    {
                        v = 1.0f - v;
                    }

                    ImGui.ColorConvertHSVToRGB(h, s, v, out var r, out var g, out var b);
                    color = new Color(r, g, b, color.A);

                    if (color.A < 1.00f)
                    {
                        color.A *= alpha;
                    }
                    break;
                }
                case ImGuiTheme.Light:
                {
                    if (color.A < 1.00f)
                    {
                        color.R *= alpha;
                        color.G *= alpha;
                        color.B *= alpha;
                        color.A *= alpha;
                    }
                    break;
                }
                default:
                    throw new NotImplementedException();
            }

            return color;
        }

        internal static void GlaivesMainMenuBar()
        {
            
            if (!_menuBarInitialized)
            {
                _console = new ImGuiConsole();
                _textureEditor = new ImGuiTextureEditor();
                foreach (Type type in Assembly.GetEntryAssembly().GetTypes())
                {
                    if (type.IsSubclassOf(typeof(Level)))
                    {
                        _levels.Add(type.Name, type);
                    }
                }

                _menuBarInitialized = true;
            }

            ImGui.BeginMainMenuBar();
            
            if (ImGui.BeginMenu("Windows"))
            {
                if (ImGui.MenuItem("Texture Editor", "CTRL+T"))
                {
                    _textureEditor.IsOpen = true;
                }

                if (ImGui.MenuItem("Console", "~"))
                {
                    _console.IsOpen = true;
                }
                
                ImGui.EndMenu();
            }
            
            _textureEditor.Draw(Engine.Get.ContentFolder);
            _console.Draw();

            if (ImGui.BeginMenu("Game"))
            {
                if (ImGui.BeginMenu("Load Level"))
                {
                    foreach (KeyValuePair<string, Type> levelKvp in _levels)
                    {
                        if (ImGui.MenuItem(levelKvp.Key))
                        {
                            Engine.Get.LoadLevel(levelKvp.Value);
                        }
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMenu();
            }
            
            ImGui.EndMainMenuBar();
            
        }
    }
}