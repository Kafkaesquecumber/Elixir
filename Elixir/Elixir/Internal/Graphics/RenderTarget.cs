﻿using System;
using Elixir.GameFramework;
using Elixir.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Elixir.Internal.Graphics
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
                (int) OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToBorder);

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