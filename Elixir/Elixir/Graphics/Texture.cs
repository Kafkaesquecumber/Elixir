using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using Elixir.GameFramework;
using Elixir.Internal;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Elixir.Internal.Interface;
using Color = Elixir.GameFramework.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Elixir.Graphics
{
    public class Texture : LoadableContent, IDisposable
    {
        /// <summary>
        /// The dimensions of the texture
        /// </summary>
        public IntVector2 Size { get; private set; }
        
        /// <summary>
        /// The OpenGL handle
        /// </summary>
        internal int Handle { get; private set; }

        internal override bool IsDisposed => Handle == 0;
        
        private TextureCreateOptions _createOptions;
        
        /// <summary>
        /// Create a new texture instance with all white pixels
        /// </summary>
        /// <param name="width">The width of the texture</param>
        /// <param name="height">The height of the texture</param>
        /// <param name="createOptions">The create options</param>
        public Texture(int width, int height, TextureCreateOptions createOptions)
        {
            Size = new IntVector2(width, height);
            LoadTexture();
        }

        private IntPtr GetPixelDataPointer()
        {
            IntPtr dataPtr;
            BitmapData data = NativeTexture.LockBits(new Rectangle(0, 0, Size.X, Size.Y), 
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            dataPtr = data.Scan0;
            NativeTexture.UnlockBits(data);
            return dataPtr;
        }

        internal void LoadTexture()
        {
            byte[] bytes = Enumerable.Repeat<byte>(255, Size.X * Size.Y * 4).ToArray();
            
            Handle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Size.X, Size.Y, 0, 
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bytes);
            
            ApplyTextureParameters();

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        
        /// <summary>
        /// Update the texture using bytes
        /// </summary>
        /// <param name="pixels">The pixels to be submitted to the texture</param>
        /// <param name="width">The width of the sub-texture to submit</param>
        /// <param name="height">The height of the sub-texture to submit</param>
        /// <param name="x">The x location in the texture to begin submitting from</param>
        /// <param name="y">The y location in the texture to begin submitting from</param>
        public void Update(byte[] pixels, int width, int height, int x, int y)
        {
            if (pixels.Length > 0 && Handle != 0)
            {
                GL.BindTexture(TextureTarget.Texture2D, Handle);
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, width, height, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
                ApplyTextureParameters();
                
                // Flush to make sure the new texture data will be up-to-date immediately
                GL.Flush();
            }
        }

        /// <summary>
        /// Update the texture using rgba colors
        /// </summary>
        /// <param name="pixels">The color pixels to be submitted to the texture</param>
        /// <param name="width">The width of the sub-texture to submit</param>
        /// <param name="height">The height of the sub-texture to submit</param>
        /// <param name="x">The x location in the texture to begin submitting from</param>
        /// <param name="y">The y location in the texture to begin submitting from</param>
        public void Update(IEnumerable<Color> pixels, int width, int height, int x, int y)
        {
            Color[] colors = pixels as Color[] ?? pixels.ToArray();
            Update(colors.ToRgbaBytes(), width, height, x, y);
        }

        /// <summary>
        /// Update all the pixels in the texture to a single color
        /// </summary>
        /// <param name="allPixels">The color for all the pixels in the texture</param>
        public void Update(Color allPixels)
        {
            Color[] colors = Enumerable.Repeat(allPixels, Size.X * Size.Y).ToArray();
            Update(colors.ToRgbaBytes(), Size.X, Size.Y, 0, 0);
        }
        
        public void Dispose()
        {
            GL.DeleteTexture(Handle);
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
