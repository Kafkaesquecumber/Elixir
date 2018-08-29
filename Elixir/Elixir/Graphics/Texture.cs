using System;
using System.IO;
using System.Linq;
using Elixir.GameFramework;
using Elixir.Internal;
using Elixir.Internal.Interface;
using OpenTK.Graphics.OpenGL;

namespace Elixir.Graphics
{
    public class Texture : ILoadableContent
    {
        /// <summary>
        /// The dimensions of the texture
        /// </summary>
        public IntVector2 Size => Interface.Size;

        internal TextureInterface Interface { get; private set; }

        internal int TextureGLHandle { get; private set; }

        /// <summary>
        /// Used by the content loader
        /// </summary>
        /// <param name="file"></param>
        /// <param name="createOptions">The create options</param>
        internal Texture(string file, TextureCreateOptions createOptions)
        {
            if (!File.Exists(file))
            {
                throw new SgeException($"No texture file exists in path {file}");
            }

            Interface = (TextureInterface)InterfaceManager.CreateInterface(InterfaceType.Texture);
            Interface.CreateTexture(file);
            LoadTextureGL(createOptions);
        }

        /// <summary>
        /// Create a new texture instance with all white pixels
        /// </summary>
        /// <param name="width">The width of the texture</param>
        /// <param name="height">The height of the texture</param>
        /// <param name="createOptions">The create options</param>
        public Texture(int width, int height, TextureCreateOptions createOptions)
        {
            Interface = (TextureInterface)InterfaceManager.CreateInterface(InterfaceType.Texture);
            Interface.CreateTexture(width, height);
            LoadTextureGL(createOptions);
        }

        private void LoadTextureGL(TextureCreateOptions createOptions)
        {
            TextureGLHandle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureGLHandle);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Size.X, Size.Y, 0, PixelFormat.Rgba, PixelType.UnsignedByte, Interface.LockTexture());
            Interface.UnlockTexture();

            int wrap;
            int filter;

            switch (createOptions.FilterMode)
            {
                case TextureFilterMode.Linear:
                    filter = (int) OpenTK.Graphics.OpenGL.TextureMinFilter.Linear;
                    break;
                case TextureFilterMode.Nearest:
                    filter = (int)OpenTK.Graphics.OpenGL.TextureMinFilter.Nearest;
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
                throw new SgeException("Color array length does not match texture dimensions * 4");
            }
            Interface.Update(rgbaBytes, Size.X, Size.Y, 0, 0);
        }
        
        /// <summary>
        /// Set the pixel data for the texture
        /// </summary>
        /// <param name="pixels">Length should match texture dimensions</param>
        public void Update(Color[] pixels)
        {
            if (pixels.Length != (Size.X * Size.Y))
            {
                throw new SgeException("Color array length does not match texture dimensions");
            }
            Update(pixels.ToRgbaBytes());
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
            Interface.Dispose();
            Interface = null;
        }

        public bool IsDisposed()
        {
            return Interface == null;
        }
    }
}
