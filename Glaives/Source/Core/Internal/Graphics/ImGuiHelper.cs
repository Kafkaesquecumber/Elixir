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
using Glaives.Core.Input;
using Glaives.Core.Internal.Input;
using Glaives.Core.Internal.Windowing;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;

// Modified from original: https://github.com/emmauss/ImGuiNet.OpentK/blob/master/src/ImGuiOpenTK.cs

namespace Glaives.Core.Internal.Graphics
{
    /// <summary>
    /// Static utility functions for ImGui 
    /// </summary>
    internal static class ImGuiHelper
    {
        internal static bool Initialized { get; private set; } = false;

        private static int _fontTexture;
        
        internal static unsafe void Init(Window window)
        {
            if (!Engine.Get.RenderImGui)
            {
                return;
            }

            if (Initialized)
            {
                return;
            }

            Initialized = true;

            window.InputActionEvent += OnInputAction;
            window.InputScrollWheel += OnInputScrollWheel;

            IO io = ImGui.GetIO();
            io.KeyMap[GuiKey.Tab] = (int)Key.Tab;
            io.KeyMap[GuiKey.LeftArrow] = (int)Key.Left;
            io.KeyMap[GuiKey.RightArrow] = (int)Key.Right;
            io.KeyMap[GuiKey.UpArrow] = (int)Key.Up;
            io.KeyMap[GuiKey.DownArrow] = (int)Key.Down;
            io.KeyMap[GuiKey.PageUp] = (int)Key.PageUp;
            io.KeyMap[GuiKey.PageDown] = (int)Key.Down;
            io.KeyMap[GuiKey.Home] = (int)Key.Home;
            io.KeyMap[GuiKey.End] = (int)Key.End;
            io.KeyMap[GuiKey.Delete] = (int)Key.Delete;
            io.KeyMap[GuiKey.Backspace] = (int)Key.Backspace;
            io.KeyMap[GuiKey.Enter] = (int)Key.Enter;
            io.KeyMap[GuiKey.Escape] = (int)Key.Escape;
            io.KeyMap[GuiKey.A] = (int)Key.A;
            io.KeyMap[GuiKey.C] = (int)Key.C;
            io.KeyMap[GuiKey.V] = (int)Key.V;
            io.KeyMap[GuiKey.X] = (int)Key.X;
            io.KeyMap[GuiKey.Y] = (int)Key.Y;
            io.KeyMap[GuiKey.Z] = (int)Key.Z;

            
            // Build texture atlas
            FontTextureData texData = io.FontAtlas.GetTexDataAsAlpha8();

            // Create OpenGL texture
            _fontTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _fontTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Alpha,
                texData.Width,
                texData.Height,
                0,
                PixelFormat.Alpha,
                PixelType.UnsignedByte,
                new IntPtr(texData.Pixels));

            // Store the texture identifier in the ImFontAtlas substructure.
            io.FontAtlas.SetTexID(_fontTexture);

            // Cleanup (don't clear the input data if you want to append new fonts later)
            //io.Fonts->ClearInputData();
            io.FontAtlas.ClearTexData();
            GL.BindTexture(TextureTarget.Texture2D, 0);

        }

        private static void OnInputScrollWheel(int wheelY)
        {
            IO io = ImGui.GetIO();
            io.MouseWheel = (float)wheelY/2;
        }

        private static void OnInputAction(KeyState keyState, Key key, int gamepadId)
        {
            IO io = ImGui.GetIO();
            switch (key)
            {
                case Key.LeftControl:
                    io.CtrlPressed = keyState == KeyState.Pressed;
                    break;
                case Key.LeftShift:
                    io.ShiftPressed = keyState == KeyState.Pressed;
                    break;
                case Key.LeftAlt:
                    io.AltPressed = keyState == KeyState.Pressed;
                    break;
                case Key.LeftMouse:
                    io.MouseDown[0] = keyState == KeyState.Pressed;
                    break;
                case Key.MiddleMouse:
                    io.MouseDown[2] = keyState == KeyState.Pressed;
                    break;
                case Key.RightMouse:
                    io.MouseDown[1] = keyState == KeyState.Pressed;
                    break;
                default:
                {
                    io.KeysDown[(int) key] = keyState == KeyState.Pressed;
                    break;
                }
            }
        }

        internal static void NewFrame(IntVector2 size, Vector2 scale, float deltaTime)
        {
            if (!Engine.Get.RenderImGui)
            {
                return;
            }

            IO io = ImGui.GetIO();
            io.DisplaySize = new Vector2(size.X, size.Y);
            io.DisplayFramebufferScale = new Vector2(scale.X, scale.Y);
            io.DeltaTime = deltaTime;

            io.MousePosition = Mouse.Position;
            
            ImGui.NewFrame();
        }

        internal static unsafe void Render(IntVector2 size)
        {
            if (!Engine.Get.RenderImGui)
            {
                return;
            }

            ImGui.Render();
            if (ImGuiNative.igGetIO()->RenderDrawListsFn == IntPtr.Zero)
                RenderDrawData(ImGuiNative.igGetDrawData(), size.X, size.Y);
        }

        private static unsafe void RenderDrawData(DrawData* drawData, int displayW, int displayH)
        {
            if (!Engine.Get.RenderImGui)
            {
                return;
            }

            GL.Viewport(0, 0, displayW, displayH);

            GL.GetInteger(GetPName.TextureBinding2D, out int last_texture);
            GL.PushAttrib(AttribMask.EnableBit | AttribMask.ColorBufferBit | AttribMask.TransformBit);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ScissorTest);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.Enable(EnableCap.Texture2D);

            GL.UseProgram(0);

            // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
            IO io = ImGui.GetIO();
            ImGui.ScaleClipRects(drawData, io.DisplayFramebufferScale);

            // Setup orthographic projection matrix
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(
                0.0f,
                io.DisplaySize.X / io.DisplayFramebufferScale.X,
                io.DisplaySize.Y / io.DisplayFramebufferScale.Y,
                0.0f,
                -1.0f,
                1.0f);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            // Render command lists

            for (int n = 0; n < drawData->CmdListsCount; n++)
            {
                NativeDrawList* cmd_list = drawData->CmdLists[n];
                byte* vtx_buffer = (byte*)cmd_list->VtxBuffer.Data;
                ushort* idx_buffer = (ushort*)cmd_list->IdxBuffer.Data;

                DrawVert vert0 = *((DrawVert*)vtx_buffer);
                DrawVert vert1 = *(((DrawVert*)vtx_buffer) + 1);
                DrawVert vert2 = *(((DrawVert*)vtx_buffer) + 2);

                GL.VertexPointer(2, VertexPointerType.Float, sizeof(DrawVert), new IntPtr(vtx_buffer + DrawVert.PosOffset));
                GL.TexCoordPointer(2, TexCoordPointerType.Float, sizeof(DrawVert), new IntPtr(vtx_buffer + DrawVert.UVOffset));
                GL.ColorPointer(4, ColorPointerType.UnsignedByte, sizeof(DrawVert), new IntPtr(vtx_buffer + DrawVert.ColOffset));

                for (int cmd_i = 0; cmd_i < cmd_list->CmdBuffer.Size; cmd_i++)
                {
                    DrawCmd* pcmd = &(((DrawCmd*)cmd_list->CmdBuffer.Data)[cmd_i]);
                    if (pcmd->UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        GL.BindTexture(TextureTarget.Texture2D, pcmd->TextureId.ToInt32());
                        GL.Scissor(
                            (int)pcmd->ClipRect.X,
                            (int)(io.DisplaySize.Y - pcmd->ClipRect.W),
                            (int)(pcmd->ClipRect.Z - pcmd->ClipRect.X),
                            (int)(pcmd->ClipRect.W - pcmd->ClipRect.Y));
                        ushort[] indices = new ushort[pcmd->ElemCount];
                        for (int i = 0; i < indices.Length; i++) { indices[i] = idx_buffer[i]; }
                        GL.DrawElements(PrimitiveType.Triangles, (int)pcmd->ElemCount, DrawElementsType.UnsignedShort, new IntPtr(idx_buffer));
                    }
                    idx_buffer += pcmd->ElemCount;
                }
            }

            // Restore modified state
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.BindTexture(TextureTarget.Texture2D, last_texture);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.PopAttrib();
        }

        

    }
}