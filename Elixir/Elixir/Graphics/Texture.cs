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
        public IntVector2 Size => new IntVector2(NativeTexture.Size.Width, NativeTexture.Size.Height);
        
        internal int TextureGLHandle { get; private set; }

        internal override bool IsDisposed => NativeTexture == null;

        internal Bitmap NativeTexture;

        /// <summary>
        /// Used by the content loader
        /// </summary>
        /// <param name="file"></param>
        /// <param name="createOptions">The create options</param>
        internal Texture(string file, TextureCreateOptions createOptions)
        {
            if (!File.Exists(file))
            {
                throw new ElixirException($"No texture file exists in path {file}");
            }

            NativeTexture = new Bitmap(Image.FromFile(file));
            LoadTextureGL(createOptions);
        }

        /// <summary>
        /// Create a new texture instance with all white pixels
        /// </summary>
        /// <param name="width">The width of the texture</param>
        /// <param name="height">The height of the texture</param>
        /// <param name="color">The color of all pixels in the texture</param>
        /// <param name="createOptions">The create options</param>
        public Texture(int width, int height, Color color, TextureCreateOptions createOptions)
        {
            NativeTexture = new Bitmap(width, height);
            Update(color);
            LoadTextureGL(createOptions);
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

        internal void LoadTextureGL(TextureCreateOptions createOptions)
        {
            IntPtr dataPtr = GetPixelDataPointer();

            TextureGLHandle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureGLHandle);
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Size.X, Size.Y, 0, 
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, dataPtr);
            
            int wrap;
            int filter;

            switch (createOptions.FilterMode)
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

            switch (createOptions.WrapMode)
            {
                case TextureWrapMode.Repeat:
                    wrap = (int) OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat;
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

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Set the pixel data for the texture
        /// </summary>
        /// <param name="rgbaBytes">Length should match texture dimensions * 4</param>
        public void Update(byte[] rgbaBytes)
        {
            if (rgbaBytes.Length != (Size.X * Size.Y * 4))
            {
                throw new ElixirException("Color array length does not match texture dimensions * 4");
            }

            BitmapData data = NativeTexture.LockBits(new Rectangle(0, 0, Size.X, Size.Y),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            
            
            Marshal.Copy(rgbaBytes, 0, data.Scan0, rgbaBytes.Length);
            
            NativeTexture.UnlockBits(data);
        }
        
        /// <summary>
        /// Set the pixel data for the texture
        /// </summary>
        /// <param name="pixels">Count should match texture dimensions</param>
        public void Update(IEnumerable<Color> pixels)
        {
            Color[] colors = pixels as Color[] ?? pixels.ToArray();
            if (colors.Length != (Size.X * Size.Y))
            {
                throw new ElixirException("Color array length does not match texture dimensions");
            }
            Update(colors.ToRgbaBytes());
        }

        /// <summary>
        /// Set all pixels in the texture to the given color
        /// </summary>
        /// <param name="allPixels"></param>
        public void Update(Color allPixels)
        {
            Color[] colors = Enumerable.Repeat(allPixels, Size.X * Size.Y).ToArray();
            Update(colors);
        }
        
        public void Dispose()
        {
            GL.DeleteTexture(TextureGLHandle);
            NativeTexture?.Dispose();
            NativeTexture = null;
        }
    }
}
