// MIT License
// 
// Copyright (c) 2018 Glaives Game Engine.
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

using System.Collections.Generic;
using Glaives.Core.Internal.Graphics;
using Glaives.Core.Internal.Windowing;
using OpenTK.Graphics.OpenGL;

namespace Glaives.Core.Graphics
{
    public class GraphicsDevice : GraphicsDeviceBase
    {
        private Shader _screenShader;
        /// <summary>
        /// <para>The screen shader is used to apply full-screen effects</para>
        /// <para>It is applied after all drawing is done</para>
        /// </summary>
        public Shader ScreenShader
        {
            get => _screenShader;
            set
            {
                if (_screenShader != value)
                {
                    _screenShader = value;
                    if (FrameBufferVertexShader != -1)
                    {
                        GL.DetachShader(FrameBufferShaderProgram, FrameBufferVertexShader);
                    }

                    if (FrameBufferFragmentShader != -1)
                    {
                        GL.DetachShader(FrameBufferShaderProgram, FrameBufferFragmentShader);
                    }

                    FrameBufferVertexShader = value.VertexShaderHandle;
                    FrameBufferFragmentShader = value.FragmentShaderHandle;

                    GL.AttachShader(FrameBufferShaderProgram, FrameBufferVertexShader);
                    GL.AttachShader(FrameBufferShaderProgram, FrameBufferFragmentShader);
                    GL.LinkProgram(FrameBufferShaderProgram);

                    PositionAttributeLocation = GL.GetAttribLocation(FrameBufferShaderProgram, Shader.VertInPositionName);
                    ColorAttributeLocation = GL.GetAttribLocation(FrameBufferShaderProgram, Shader.VertInColorName);
                    TexCoordAttributeLocation = GL.GetAttribLocation(FrameBufferShaderProgram, Shader.VertInTexCoordName);

                    FrameBufferRenderProgram = new RenderProgram(BlendMode.Alpha, null, value, 0);
                }
            }
        }

        private readonly List<DrawableActor> _drawables = new List<DrawableActor>();

        internal readonly RenderTarget FrameBuffer;
        internal readonly int FrameBufferVbo;
        internal readonly Vertex[] FrameBufferVertices;
        internal readonly int FrameBufferShaderProgram;
        internal int PositionAttributeLocation;
        internal int ColorAttributeLocation;
        internal int TexCoordAttributeLocation;
        internal RenderProgram FrameBufferRenderProgram;
        internal int FrameBufferVertexShader = -1;
        internal int FrameBufferFragmentShader = -1;
        

        internal GraphicsDevice(Window window)
            : base(window)
        {
            FrameBufferShaderProgram = GL.CreateProgram();
            FrameBufferVertices = Shapes.MakeQuad();
            FrameBufferVbo = GL.GenBuffer();
            ScreenShader = Shader.Default;

            FrameBuffer = new RenderTarget(window.Size, TextureFilterMode.Smooth);
        }

        internal void AddDrawableActor(DrawableActor drawableActor)
        {
            _drawables.Add(drawableActor);
        }

        internal void RemoveDrawableActor(DrawableActor drawableActor)
        {
            _drawables.Remove(drawableActor);
        }

        /// <inheritdoc />
        internal override void DrawBatches()
        {
            foreach (GeometryBatch batch in Batches)
            {
                batch.Begin(); // clears all vertices
            }

            BatchesAreHot = true;

            // Iterate over the drawable and submit their vertices to the appropriate batch
            foreach (DrawableActor drawable in _drawables)
            {
                if (drawable.RenderProgramIsDirty)
                {
                    // Find a batch with matching render programs, if such a batch does not exist, create a new one
                    GeometryBatch batch = FindBatch(drawable.RenderProgram) ?? CreateBatch(drawable.RenderProgram);
                    drawable.Batch = batch;
                    drawable.RenderProgramIsDirty = false;
                }

                // Submit vertices
                Vertex[] vertices = drawable.ConstructVertices();
                if (vertices != null)
                {
                    if (vertices.Length > 0)
                    {
                        drawable.Batch.AddVertices(vertices);
                    }
                }
            }

            // Remove empty batches
            for (int i = Batches.Count - 1; i >= 0; i--)
            {
                if (Batches[i].VertexArrayPosition == 0)
                {
                    Batches.RemoveAt(i);
                }
            }

            FrameBuffer.Bind();
            foreach (GeometryBatch batch in Batches)
            {
                // Draw all vertices to the frame buffer
                batch.End(new IntRect(0, 0,  FrameBuffer.Size.X, FrameBuffer.Size.Y), Engine.Get.LevelManager.Level.CurrentView.ProjectionMatrix);
            }
            BatchesAreHot = false;
            FrameBuffer.UnBind();

            // Draw the screen quad (final image) using the frame buffer color texture
            Viewport viewport = Engine.Get.Viewport;
            Draw(new IntRect(0, 0, viewport.Size.X, viewport.Size.Y), FrameBufferShaderProgram, FrameBufferVertices, FrameBufferVertices.Length, FrameBufferVbo,
                PositionAttributeLocation, ColorAttributeLocation, TexCoordAttributeLocation,
                 FrameBufferRenderProgram, Matrix.Identity, FrameBuffer.ColorTexture);
        }
    }
}