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
using Glaives.GameFramework;
using Glaives.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Glaives.Internal.Graphics
{
    internal class RenderTarget : IDisposable
    {
        internal IntVector2 Size { get; }
        internal int ColorTexture { get; }

        private readonly int _fbo;
        
        internal RenderTarget(IntVector2 size, TextureFilterMode textureFilterMode)
        {
            Size = size;
            ColorTexture = GL.GenTexture();
            _fbo = GL.Ext.GenFramebuffer();
            ResetFbo(textureFilterMode);
        }
        
        internal void ResetFbo(TextureFilterMode textureFilterMode)
        {
            GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Size.X, Size.Y, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            int filter;

            switch (textureFilterMode)
            {
                case TextureFilterMode.Linear:
                    filter = (int)TextureMagFilter.Linear;
                    break;
                case TextureFilterMode.Nearest:
                    filter = (int) TextureMagFilter.Nearest;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(textureFilterMode), textureFilterMode, null);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, filter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, filter);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                (int) OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, _fbo);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
                TextureTarget.Texture2D, ColorTexture, 0);

            FramebufferErrorCode errorCode = GL.Ext.CheckFramebufferStatus(FramebufferTarget.FramebufferExt);
            if (errorCode != FramebufferErrorCode.FramebufferComplete)
            {
                Engine.Get.Debug.Error($"Failed to create render target: {errorCode}");
                GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
                GL.Ext.DeleteFramebuffer(_fbo);
                return;
            }

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        internal void Bind()
        {
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, _fbo);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        internal void UnBind()
        {
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
        }

        public void Dispose()
        {
            GL.DeleteFramebuffer(_fbo);
        }
    }
}
