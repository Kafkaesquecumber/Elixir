﻿// MIT License
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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Glaives.Core.Internal;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using Glaives.Core;

namespace Glaives.Core.Graphics
{
    /// <inheritdoc />
    /// <summary>
    /// <para>Unlike a normal texture, a dynamic texture has direct access to it's pixel data</para>
    /// <para>Use a dynamic texture if you need flexible access to read and write pixel data</para>
    /// <para>A dynamic texture can be convert to a normal texture and vise-versa (do not forget to dispose the dynamic texture when you are done with it)</para>
    /// <para>Supported formats: Png, Jpeg, Bmp and Gif</para>
    /// </summary>
    public class DynamicTexture : IDisposable
    {
        /// <summary>
        /// The dimensions of the dynamic texture
        /// </summary>
        public IntVector2 Size => new IntVector2(_bitmap.Width, _bitmap.Height);

        /// <summary>
        /// The pixel data pointer
        /// </summary>
        public unsafe IntPtr Ptr
        {
            get
            {
                fixed (Rgba32* ptr = &MemoryMarshal.GetReference(_bitmap.GetPixelSpan()))
                {
                    return new IntPtr(ptr);
                }
            }
        }

        private readonly Image<Rgba32> _bitmap;


        /// <summary>
        /// Load a dynamic texture (supported formats: Png, Jpeg, Bmp and Gif)
        /// </summary>
        /// <param name="file">The path to the file (with extension)</param>
        public DynamicTexture(string file)
        {
            if (!File.Exists(file))
            {
                throw new GlaivesException($"Failed to create dynamic texture: file '{file}' not found");
            }
            
            // Load the bitmap
            _bitmap = Image.Load(file);
        }

        /// <summary>
        /// Create a new dynamic texture
        /// </summary>
        /// <param name="width">The width of the dynamic texture</param>
        /// <param name="height">The height of the dynamic texture</param>
        public DynamicTexture(int width, int height)
        {
            _bitmap = new Image<Rgba32>(width, height);
        }

        /// <summary>
        /// Create a new dynamic texture 
        /// </summary>
        /// <param name="texture">A texture to copy the pixel data from</param>
        public DynamicTexture(Texture texture)
        {
            if (texture.Handle == 0)
            {
                throw new GlaivesException("Failed to create dynamic texture from texture: The texture has been disposed");
            }

            byte[] bytes = texture.GetBytes();
            _bitmap = new Image<Rgba32>(texture.Size.X, texture.Size.Y);
            WriteBytes(bytes);
        }

        /// <summary>
        /// <para>Read the RGBA bytes from the dynamic texture and convert them to colors</para>
        /// <para>The length of the color array is (Size.X * Size.Y)</para>
        /// </summary>
        /// <returns>The pixels as colors</returns>
        public Color[] ReadColors()
        {
            return ReadBytes().ToColors();
        }

        /// <summary>
        /// <para>Read all the RGBA bytes from the dynamic texture</para>
        /// <para>The length of the byte array is (Size.X * Size.Y * 4)</para>
        /// </summary>
        /// <returns>The pixels as RGBA bytes</returns>
        public byte[] ReadBytes()
        {
            return ReadBytes(new IntRect(0, 0, Size.X, Size.Y));
        }

        /// <summary>
        /// <para>Read the RGBA bytes from the dynamic texture and convert them to colors</para>
        /// <para>The length of the color array will be (region.Width * region.Height) unless a part of the region falls of the texture, 
        /// the region will then size down to fit the texture exactly</para>
        /// </summary>
        /// <param name="region">The region on the dynamic texture to read from</param>
        /// <returns>The pixels as colors</returns>
        public Color[] ReadColors(IntRect region)
        {
            return ReadBytes(region).ToColors();
        }

        /// <summary>
        /// <para>Read the RGBA bytes from the dynamic texture</para>
        /// <para>The length of the byte array will be (region.Width * region.Height * 4) unless a part of the region falls of the texture, 
        /// the region will then size down to fit the texture exactly</para>
        /// </summary>
        /// <param name="region">The region on the dynamic texture to read from</param>
        /// <returns>The pixels as RGBA bytes</returns>
        public byte[] ReadBytes(IntRect region)
        {
            if (_bitmap == null)
            {
                Engine.Get.Debug.Error("Failed to read bytes from dynamic texture: the dynamic texture has been disposed");
                return new byte[0];
            }

            // Throw exception if the region overflows the bitmap surface area
            if ((region.X + region.Width > _bitmap.Width) || (region.Y + region.Height > _bitmap.Height))
            {
                Engine.Get.Debug.Error("Failed to read bytes from dynamic texture: " +
                                       "region overflows the dynamic texture, make sure the region fits on the dynamic texture");
                return new byte[0];
            }

            // If the region falls of the texture on the horizontal axis, size the region width down to fit the texture exactly
            if (region.X + region.Width > _bitmap.Width)
            {
                region.Width = _bitmap.Width - region.X;
            }

            // If the region falls of the texture on the vertical axis, size the region height down to fit the texture exactly
            if (region.Y + region.Height > _bitmap.Height)
            {
                region.Height = _bitmap.Height - region.Y;
            }

            // Create RGBA byte array 
            byte[] bytes = new byte[region.Width * region.Height * 4];

            Marshal.Copy(Ptr, bytes, 0, bytes.Length);

            return bytes;
        }

        /// <summary>
        /// <para>Write RGBA bytes to the dynamic texture</para>
        /// </summary>
        /// <param name="bytes">The RGBA bytes to write</param>
        public void WriteBytes(byte[] bytes)
        {
            WriteBytes(bytes, new IntRect(0, 0, Size.X, Size.Y));
        }

        /// <summary>
        /// <para>Write the colors to the dynamic texture</para>
        /// </summary>
        /// <param name="colors">The colors to write</param>
        public void WriteColors(Color[] colors)
        {
            WriteBytes(colors.ToBytes(), new IntRect(0, 0, Size.X, Size.Y));
        }

        /// <summary>
        /// <para>Write the colors to the dynamic texture in the specified region</para>
        /// </summary>
        /// <param name="colors">The colors to write</param>
        /// <param name="region">The region on the dynamic texture to write into</param>
        public void WriteColors(Color[] colors, IntRect region)
        {
            WriteBytes(colors.ToBytes(), region);
        }

        /// <summary>
        /// <para>Write a single color into all pixels in the region</para>
        /// </summary>
        /// <param name="color">The color to write</param>
        /// <param name="region">The region on the dynamic texture to write into</param>
        public void WriteColor(Color color, IntRect region)
        {
            Color[] colors = Enumerable.Repeat(color, region.Width * region.Height).ToArray();
            WriteBytes(colors.ToBytes(), region);
        }

        /// <summary>
        /// <para>Write a single color into all pixels in the dynamic texture</para>
        /// </summary>
        /// <param name="color">The color to write</param>
        public void WriteColor(Color color)
        {
            WriteColor(color, new IntRect(0, 0, Size.X, Size.Y));
        }

        /// <summary>
        /// <para>Write a dynamic texture into this dynamic texture</para>
        /// <para>The input dynamic texture must not be larger than the target dynamic texture</para>
        /// </summary>
        /// <param name="dynamicTexture">The dynamic texture to write into the target dynamic texture</param>
        /// <param name="x">The x start location on the target dynamic texture</param>
        /// <param name="y">The y start location on the target dynamic texture</param>
        public void WriteDynamicTexture(DynamicTexture dynamicTexture, int x, int y)
        {
            IntRect region = new IntRect(x, y, dynamicTexture.Size.X, dynamicTexture.Size.Y);
            byte[] bytes = dynamicTexture.ReadBytes();
            WriteBytes(bytes, region);
        }

        /// <summary>
        /// <para>Write the RGBA bytes to the dynamic texture</para>
        /// </summary>
        /// <param name="bytes">The RGBA bytes to write</param>
        /// <param name="region">The region on the dynamic texture to write to</param>
        public void WriteBytes(byte[] bytes, IntRect region)
        {
            if (_bitmap == null)
            {
                Engine.Get.Debug.Error("Failed to write bytes to dynamic texture: the target dynamic texture has been disposed");
                return;
            }
            
            // Throw exception if the region overflows the bitmap surface area
            if ((region.X + region.Width > _bitmap.Width) || (region.Y + region.Height > _bitmap.Height))
            {
                Engine.Get.Debug.Error("Failed to write bytes to dynamic texture: " +
                                       "region overflows the dynamic texture, make sure the region fits on the target dynamic texture");
                return;
            }
            
            // The bitmap bytes to be blended with the input bytes and ultimately copied to the bitmap
            byte[] bmpBytes = new byte[_bitmap.Width * _bitmap.Height * 4];

            // Copy the full texture to the bitmap bytes
            Marshal.Copy(Ptr, bmpBytes, 0, bmpBytes.Length);


            // The loops below blend the bitmap bytes and the input bytes together using alpha blending

            int bytesOffsetY = 0;
            for (int y = region.Y; y < region.Y + region.Height; y++, bytesOffsetY++)
            {
                int bmpStartY = y * _bitmap.Width * 4;              // The y location in the bitmap bytes
                int bytesStartY = bytesOffsetY * region.Width * 4;  // The y location in the input bytes

                int bytesX = 0;
                for (int bmpX = region.X; bmpX < region.X + region.Width; bmpX++, bytesX++)
                {
                    int bmpStartX = bmpX * 4;       // The x location in the bitmap bytes
                    int bytesStartX = bytesX * 4;   // The x location in the input bytes 
                    
                    // The pixel values of the bitmap bytes
                    byte bmpR = bmpBytes[bmpStartY + bmpStartX + 0]; // Bitmap bytes red
                    byte bmpG = bmpBytes[bmpStartY + bmpStartX + 1]; // Bitmap bytes green
                    byte bmpB = bmpBytes[bmpStartY + bmpStartX + 2]; // Bitmap bytes blue
                    byte bmpA = bmpBytes[bmpStartY + bmpStartX + 3]; // Bitmap bytes alpha

                    // Break when we reached the end of the input bytes array
                    if ((bytesStartY + bytesStartX + 3) >= bytes.Length)
                    {
                        break;
                    }

                    // The pixel values of the input bytes
                    byte bytesR = bytes[bytesStartY + bytesStartX + 0]; // Input bytes red
                    byte bytesG = bytes[bytesStartY + bytesStartX + 1]; // Input bytes green
                    byte bytesB = bytes[bytesStartY + bytesStartX + 2]; // Input bytes blue
                    byte bytesA = bytes[bytesStartY + bytesStartX + 3]; // Input bytes alpha

                    byte rem = (byte)(255 - bytesA);

                    // Assign pixel values from the input bytes to the bitmap bytes with alpha blending
                    bmpBytes[bmpStartY + bmpStartX + 0] = (byte)((bmpR * rem + bytesR * bytesA) / 255); // Alpha-blended bitmap bytes red
                    bmpBytes[bmpStartY + bmpStartX + 1] = (byte)((bmpG * rem + bytesG * bytesA) / 255); // Alpha-blended bitmap bytes green
                    bmpBytes[bmpStartY + bmpStartX + 2] = (byte)((bmpB * rem + bytesB * bytesA) / 255); // Alpha-blended bitmap bytes blue
                    bmpBytes[bmpStartY + bmpStartX + 3] = (byte)((bmpA * rem + bytesA * bytesA) / 255); // Alpha-blended bitmap bytes alpha
                }
            }

            Marshal.Copy(bmpBytes, 0, Ptr, bmpBytes.Length);
        }

        /// <summary>
        /// Save the dynamic texture to a file
        /// </summary>
        /// <param name="file">The path and file name + extension</param>
        public void Save(string file)
        {
            if (_bitmap == null)
            {
                Engine.Get.Debug.Error("Failed to save dynamic texture: texture has been disposed");
                return;
            }

            _bitmap.Save(file);
        }
        
        /// <inheritdoc />
        /// <summary>
        /// Dispose the dynamic texture
        /// </summary>
        public void Dispose()
        {
            _bitmap?.Dispose();
        }
    }
}