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
using System.IO;
using System.Linq;
using Glaives.Core.Graphics;
using Glaives.Core.Utils;
using ImGuiNET;

namespace Glaives.Core.Internal.EditorGui
{
    internal class ImGuiTextureEditor : ImGuiWindow
    {
        internal int TextureIndex;

        private int _prevTextureIndex = -1;
        private List<string> _textureFiles = new List<string>();
        private bool _textureFilesCached;
        private Texture texture;
        
        internal void Draw(string contentFolder)
        {
            if (!IsOpen)
            {
                return;
            }

            ImGui.BeginWindow("Texture Editor", ref IsOpen, new Vector2(500, 300), 1.0f, WindowFlags.Default);
            

            if (!_textureFilesCached)
            {
                _textureFiles.Clear();
                IEnumerable<string> files = IOUtils.EnumerateFilesInDirectoryResursively(contentFolder);
                foreach (string file in files)
                {
                    string ext = Path.GetExtension(file)?.ToLower();
                    if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" ||
                        ext == ".bmp" || ext == ".gif")
                    {
                        _textureFiles.Add(file);
                    }
                }

                _textureFilesCached = true;
            }
            
            ImGui.Combo("Texture", ref TextureIndex, _textureFiles.ToArray());
            if (TextureIndex != _prevTextureIndex)
            {
                texture?.Dispose();
                texture = new Texture(_textureFiles[TextureIndex], TextureCreateOptions.Sharp);
            }
            _prevTextureIndex = TextureIndex;

            if (texture != null)
            {
                ImGui.Image(new IntPtr(texture.Handle), new Vector2(texture.Size.X, texture.Size.Y), Vector2.Zero, Vector2.Unit, Color.White, Color.White);
            }

            ImGui.EndWindow();
        }

        
    }
}