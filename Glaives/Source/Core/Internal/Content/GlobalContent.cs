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

using Glaives.Core.Graphics;

// Note: do not use the ContentLoader in this class, it has not been created yet (by design)
// The global content is used by Glaives AND Glaives.Editor

namespace Glaives.Core.Internal.Content
{
    internal class GlobalContent
    {
        private static GlobalContent _globalContent;
        internal static GlobalContent Get => _globalContent ?? (_globalContent = new GlobalContent());

        internal Texture TextureWhite32x32;
        internal Font FontConsolasRegular32;
        internal Shader ShaderTextured;

        internal GlobalContent()
        {
            TextureWhite32x32 = new Texture(32, 32, Color.White, new TextureCreateOptions(TextureFilterMode.Sharp, TextureWrapMode.ClampToEdge));
            TextureWhite32x32.Update(Color.White);
            FontConsolasRegular32 = new Font("Global/F_ConsolasRegular.ttf", new FontCreateOptions(32));
            ShaderTextured = new Shader("Global/S_Textured.vs", "Global/S_Textured.fs");
        }
    }
}
