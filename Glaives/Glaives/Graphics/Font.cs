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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Glaives.GameFramework;
using Glaives.Internal;
using Glaives.Internal.Graphics;
using SharpFont;

namespace Glaives.Graphics
{
    public class Font : LoadableContent, IDisposable
    {
        /// <summary>
        /// The maximum size allowed for fonts
        /// </summary>
        public static int MaxFontSize => 1024;

        // The size in pixels the rows start from
        // this creates a free pixel stroke to be used for special rendering case such as lines
        private const int RowOffset = 2;

        internal override bool IsDisposed => FontFace == null;
        
        internal FontFace FontFace { get; private set; }

        /// <summary>
        /// The size of the font
        /// </summary>
        public readonly int FontSize;

        //TODO: What if the texture isnt big enough, we'll potentially need multiple texture
        internal Texture Texture { get; private set; }

        private readonly Dictionary<char, GlyphInfo> _pages = new Dictionary<char, GlyphInfo>();
        private float _nextFreeColumn;
        private float _nextFreeRow;
        
        internal Font(string file, int fontSize)
        {
            FontSize = fontSize;
            if (FontSize > MaxFontSize || FontSize < 0)
            {
                throw new GlaivesException($"Font size must be between 0 and {MaxFontSize}");
            }

            FontFace = new FontFace(File.OpenRead(file));
            Texture = new Texture(128, 128, new TextureCreateOptions(TextureFilterMode.Linear, TextureWrapMode.ClampToEdge));

            // pixels in the top-left corner is used for strikeout/underlines... :)
            // This has to be the same texture because of the way we do vertex batching
            // more than just 1 because of anti aliasing on the texture
            Texture.Update(Enumerable.Repeat<byte>(255, 4 * (RowOffset * RowOffset)).ToArray(), 
                new IntRect(0, 0, RowOffset, RowOffset));
        }
        
        internal GlyphInfo LoadGlyph(char codePoint)
        {
            Glyph glyph = FontFace.GetGlyph(codePoint, FontSize);
            if (glyph == null)
            {
                if (codePoint == '\n')
                {
                    return GlyphInfo.Empty;
                }
                Engine.Get.Debug.Warning($"Character '{codePoint}' not supported by font '{FontFace.FullName}'");
                return GlyphInfo.Empty;
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

                // Free the memory of the surface bits
                Marshal.FreeHGlobal(fontSurface.Bits);

                // Create RGB version of data bytes 
                byte[] pixels = new byte[len * 4];
                int index = 0;
                
                // Fill the RGB pixels with the data 
                for (int i = 0; i < len; i++)
                {
                    byte c = data[i];
                    pixels[index++] = c;
                    pixels[index++] = c;
                    pixels[index++] = c;
                    pixels[index++] = c; 
                }

                FaceMetrics faceMetrics = FontFace.GetFaceMetrics(FontSize);

                // padding between glyphs to avoid bleeding in rotated text
                const int padding = 3;
                
                // Check if the glyph does not fit on the column
                while (Texture.Size.X < (_nextFreeColumn + glyph.RenderWidth + padding + RowOffset))
                {
                    int newSizeX = Texture.Size.X * 2;

                    // Check if the glyph does not fit on the row
                    if (newSizeX > Texture.MaxTextureSize)
                    {                        
                        AddTextureRow();
                    }
                    else
                    {
                        // Make the texture wider
                        ResizeTexture(newSizeX, Texture.Size.Y);
                    }
                }

                while (Texture.Size.Y < (_nextFreeRow + glyph.RenderHeight + padding))
                {
                    AddTextureRow();
                }

                // Update texture with the glyph
                Texture.Update(pixels, new IntRect((int)_nextFreeColumn + RowOffset, (int)_nextFreeRow, glyph.RenderWidth, glyph.RenderHeight));

                float sourceX = Math.Max(RowOffset, (_nextFreeColumn + RowOffset) - ((float)padding / 2));
                

                // Create the rect representing the region for this glyph on the updated texture
                FloatRect sourceRect = new FloatRect(sourceX, _nextFreeRow , 
                    glyph.RenderWidth + padding, glyph.RenderHeight );
                
                // Move column to next available
                _nextFreeColumn += glyph.RenderWidth + padding;

                // Create and add glyph info to the pages
                GlyphInfo glyphInfo = new GlyphInfo
                {
                    Advance = glyph.HorizontalMetrics.Advance,
                    BearingX = glyph.HorizontalMetrics.Bearing.X,
                    BearingY = glyph.HorizontalMetrics.Bearing.Y,
                    SourceRect = sourceRect
                };
                
                DynamicTexture temp = new DynamicTexture(Texture);
                temp.Save("glyphs.png");
                _pages.Add(codePoint, glyphInfo);
            }
            
            return _pages[codePoint];
        }

        private void AddTextureRow()
        {
            int newSizeY = Texture.Size.Y * 2;

            if (newSizeY > Texture.MaxTextureSize)
            {
                // Is this a problem?
                throw new GlaivesException("Glyph atlas texture has reached it's size limit");
            }
            else
            {
                // Make the texture taller
                ResizeTexture(Texture.Size.X, newSizeY);

                // Move row to the next available
                _nextFreeRow = RowOffset;

                // Reset the column to the start of the line
                _nextFreeColumn = RowOffset;
            }
        }

        private void ResizeTexture(int width, int height)
        {
            // Create a new, bigger texture
            Texture texture = new Texture(width, height,
                new TextureCreateOptions(TextureFilterMode.Linear, TextureWrapMode.ClampToEdge));

            // Write bytes from the old texture into the new texture
            texture.Update(Texture.GetBytes(), new IntRect(0, 0, Texture.Size.X, Texture.Size.Y));

            // Swap the old with the new texture
            SwapTexture(texture);
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
