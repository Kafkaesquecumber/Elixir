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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Glaives.GameFramework;
using Glaives.Internal;
using Glaives.Internal.Graphics;
using SharpFont;

namespace Glaives.Graphics
{
    public class Font : LoadableContent, IDisposable
    {
        internal override bool IsDisposed => FontFace == null; 
        
        internal FontCreateOptions CreateOptions { get; private set; }
        
        public FontFace FontFace { get; private set; }
        
        private Dictionary<char, GlyphInfo> _pages = new Dictionary<char, GlyphInfo>();

        //TODO: What if the texture isnt big enough, we'll potentially need multiple texture
        internal Texture Texture { get; private set; }

        internal Font(string file, FontCreateOptions createOptions)
        {
            CreateOptions = createOptions;
            
            FontFace = new FontFace(File.OpenRead(file));
            Texture = new Texture(100, 100, new TextureCreateOptions(createOptions.FilterMode, TextureWrapMode.ClampToEdge));
        }

        private int _endX = 0; // HACK

        internal GlyphInfo LoadGlyph(char codePoint)
        {
            Glyph glyph = FontFace.GetGlyph(codePoint, CreateOptions.Size);
            if (glyph == null)
            {
                if (codePoint == '\n')
                {
                    return GlyphInfo.Empty;
                }
                Engine.Get.Debug.Warning($"Character '{codePoint}' not supported by font '{FontFace.FullName}'");
            }
            
            if (!_pages.ContainsKey(codePoint))
            {
                // Load the glyph into the texture
                Surface fontSurface = new Surface
                {
                    Bits = Marshal.AllocHGlobal(glyph.RenderWidth * glyph.RenderHeight),
                    Width = glyph.RenderWidth,
                    Height = glyph.RenderHeight,
                    Pitch = glyph.RenderWidth
                };
                
                // Clear the memory region of the surface bits
                Marshal.Copy(new byte[glyph.RenderWidth * glyph.RenderHeight], 0,
                    fontSurface.Bits, glyph.RenderWidth * glyph.RenderHeight);

                // Render the glyph to the surface
                glyph.RenderTo(fontSurface);


                int width = fontSurface.Width;
                int height = fontSurface.Height;
                int len = width * height;
                byte[] data = new byte[len];

                // Copy the bits into the data bytes array
                Marshal.Copy(fontSurface.Bits, data, 0, len);

                // Create RGBA version of data bytes 
                byte[] pixels = new byte[len * 4];
                
                int index = 0;

                // Fill the RGBA pixels with the data 
                for (int j = 0; j < len; j++)
                {
                    byte c = data[j];
                    pixels[index++] = c;
                    pixels[index++] = c;
                    pixels[index++] = c;
                    pixels[index++] = c;
                }

                // Free the memory of the surface bits
                Marshal.FreeHGlobal(fontSurface.Bits);

                //TODO: later: multiple pages per font

                const int padding = 1; // padding between glyphs to avoid bleeding in rotated text

                // Resize texture if the glyph does not fit
                if (Texture.Size.X < (_endX + glyph.RenderWidth + padding))
                {
                    int newSizeX = Texture.Size.X + glyph.RenderWidth + padding;
                    
                    // Create a new, bigger texture
                    Texture texture = new Texture(newSizeX, Texture.Size.Y,
                        new TextureCreateOptions(CreateOptions.FilterMode, TextureWrapMode.ClampToEdge));

                    // Write bytes from the old texture into the new texture
                    texture.Update(Texture.GetBytes(), new IntRect(0, 0, Texture.Size.X, Texture.Size.Y));

                    // Swap the old with the new texture
                    SwapTexture(texture);
                }

                // Update texture with the glyph
                Texture.Update(pixels, new IntRect(_endX, 0, glyph.RenderWidth, glyph.RenderHeight));
                
                // Create the rect representing the region for this glyph on the updated texture
                FloatRect sourceRect = new FloatRect(_endX, 0, glyph.RenderWidth, glyph.RenderHeight);
                
                // Increment HACK endX
                _endX += glyph.RenderWidth + padding;

                // Create and add glyph info to the pages
                GlyphInfo glyphInfo = new GlyphInfo
                {
                    Advance = glyph.HorizontalMetrics.Advance,
                    BearingX = glyph.HorizontalMetrics.Bearing.X,
                    BearingY = glyph.HorizontalMetrics.Bearing.Y,
                    SourceRect = sourceRect
                };

                DynamicTexture temp = new DynamicTexture(Texture);
                temp.Save("Updated.png");

                _pages.Add(codePoint, glyphInfo);
            }
            
            return _pages[codePoint];
        }

        private void SwapTexture(Texture newTexture)
        {
            Texture.Dispose();
            Texture = newTexture;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Texture?.Dispose();
            FontFace = null;
        }
    }
}
