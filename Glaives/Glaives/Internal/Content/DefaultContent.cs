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

using Glaives.GameFramework;
using Glaives.Graphics;

// Note: do not use the ContentLoader in this class, it has not been created yet (by design)

namespace Glaives.Internal.Content
{
    internal class DefaultContent
    {
        internal Texture TextureWhite32x32;
        internal Texture TextureGlyphNotFound;
        internal Font FontConsolasRegular32;

        internal DefaultContent()
        {
            TextureWhite32x32 = new Texture(32, 32, new TextureCreateOptions(TextureFilterMode.Sharp, TextureWrapMode.ClampToEdge));
            TextureWhite32x32.Update(Color.White);
            TextureGlyphNotFound = new Texture("EngineContent/T_GlyphNotFound.png", TextureCreateOptions.Smooth);
            FontConsolasRegular32 = new Font("EngineContent/F_ConsolasRegular.ttf", 32);
        }
    }
}
