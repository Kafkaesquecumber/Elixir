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
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Glaives.GameFramework;

namespace Glaives.Graphics
{
    /// <inheritdoc cref="LoadableContent" />
    /// <summary>
    /// <para>A texture exists only on the GPU and is therefor not as flexible as a dynamic texture</para>
    /// <para>Use a dynamic texture if you want access and/or modify the pixel data on the CPU</para>
    /// <para>A dynamic texture can be converted to a texture and vise-versa (do not forget to dispose the dynamic texture when you are done with it)</para>
    /// </summary>
    public class Texture : LoadableContent, IDisposable
    {
        internal const PixelFormat PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;

        /// <summary>
        /// The maximum width/height that a texture supports
        /// </summary>
        public static int MaxTextureSize => GL.GetInteger(GetPName.MaxTextureSize);

        /// <summary>
        /// The gl handle 
        /// </summary>
        internal int Handle { get; private set; }

        /// <summary>
        /// The dimensions of the texture
        /// </summary>
        public readonly IntVector2 Size;
        
        internal override bool IsDisposed => Handle == 0;
        
        private readonly TextureCreateOptions _createOptions;
        
        /// <summary>
        /// Create a new empty texture without any initial pixel data
        /// </summary>
        /// <param name="width">The width of the texture</param>
        /// <param name="height">The height of the texture</param>
        /// <param name="createOptions">The create options</param>
        public Texture(int width, int height, TextureCreateOptions createOptions)
        {
            _createOptions = createOptions;
            Size = new IntVector2(width, height);
            LoadTexture(null); // Do not make a pixel byte array, when the dimensions are huge we could go out of CPU memory
        }

        /// <summary>
        /// Create a new texture
        /// </summary>
        /// <param name="width">The width of the texture</param>
        /// <param name="height">The height of the texture</param>
        /// <param name="bytes">The RGBA bytes to load this texture with</param>
        /// <param name="createOptions">The create options</param>
        public Texture(int width, int height, byte[] bytes, TextureCreateOptions createOptions)
        {
            _createOptions = createOptions;
            Size = new IntVector2(width, height);
            LoadTexture(bytes);
        }

        /// <summary>
        /// Create a new texture
        /// </summary>
        /// <param name="dynamicTexture">The dynamic texture to copy the pixel data from</param>
        /// <param name="createOptions">The create options</param>
        public Texture(DynamicTexture dynamicTexture, TextureCreateOptions createOptions)
        {
            _createOptions = createOptions;
            Size = new IntVector2(dynamicTexture.Size.X, dynamicTexture.Size.Y);
            LoadTexture(dynamicTexture.ReadBytes());
        }

        // This constructor is reserved for the content loader, it's signature should not be changed
        internal Texture(string file, TextureCreateOptions createOptions)
        {
            _createOptions = createOptions;

            // Create a temporary dynamic texture so we can load the file and read the pixel data
            using (DynamicTexture temp = new DynamicTexture(file))
            {
                Size = new IntVector2(temp.Size.X, temp.Size.Y);
                // Load the texture into gl
                LoadTexture(temp.ReadBytes());
            }
        }

        /// <summary>
        /// Update the texture on the GPU using 32-bit RGBA colors
        /// </summary>
        /// <param name="colors">The color pixels to be submitted to the texture</param>
        /// <param name="region">The region of the sub texture</param>
        public void Update(IEnumerable<GameFramework.Color> colors, IntRect region)
        {
            GameFramework.Color[] colorArray = colors as GameFramework.Color[] ?? colors.ToArray();
            Update(colorArray.ToBytes(), region);
        }

        /// <summary>
        /// Update all the pixels in the texture on the GPU to a single 32-bit RGBA color
        /// </summary>
        /// <param name="color">The color for all the pixels in the texture</param>
        public void Update(GameFramework.Color color)
        {
            GameFramework.Color[] colors = Enumerable.Repeat(color, Size.X * Size.Y).ToArray();
            Update(colors.ToBytes(), new IntRect(0, 0, Size));
        }

        /// <summary>
        /// Update the texture on the GPU
        /// </summary>
        /// <param name="bytes">The bytes to be submitted to the texture</param>
        /// <param name="region">The region of the sub texture</param>
        public void Update(byte[] bytes, IntRect region)
        {
            if (bytes.Length > 0 && Handle != 0)
            {
                // Bind the texture
                GL.BindTexture(TextureTarget.Texture2D, Handle);
                
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, region.X, region.Y, region.Width, region.Height, 
                    PixelFormat, PixelType.UnsignedByte, bytes);

                ApplyTextureParameters();

                // Flush to make sure the new texture data will be up-to-date immediately
                GL.Flush();

                // Unbind the texture
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
        }

        /// <summary>
        /// Get the RGBA bytes of the texture from the GPU
        /// </summary>
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[Size.X * Size.Y * 4];
            
            // Bind texture and copy the texture data into the byte array
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat, PixelType.UnsignedByte, bytes);

            // Unbind the texture
            GL.BindTexture(TextureTarget.Texture2D, 0);

            return bytes;
        }

        /// <inheritdoc />
        /// <summary>
        /// Dispose the texture
        /// </summary>
        public void Dispose()
        {
            GL.DeleteTexture(Handle);
        }

        internal void LoadTexture(byte[] bytes)
        {
            Handle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Size.X, Size.Y, 0,
                PixelFormat, PixelType.UnsignedByte, bytes);

            ApplyTextureParameters();

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private void ApplyTextureParameters()
        {
            int wrap;
            int filter;

            switch (_createOptions.FilterMode)
            {
                case TextureFilterMode.Linear:
                    filter = (int)TextureMinFilter.Linear;
                    break;
                case TextureFilterMode.Nearest:
                    filter = (int)TextureMinFilter.Nearest;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (_createOptions.WrapMode)
            {
                case TextureWrapMode.Repeat:
                    wrap = (int)OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat;
                    break;
                case TextureWrapMode.MirroredRepeat:
                    wrap = (int)OpenTK.Graphics.OpenGL.TextureWrapMode.MirroredRepeat;
                    break;
                case TextureWrapMode.ClampToEdge:
                    wrap = (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge;
                    break;
                case TextureWrapMode.ClampToBorder:
                    wrap = (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToBorder;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, wrap);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, wrap);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, filter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, filter);
        }
    }
}
