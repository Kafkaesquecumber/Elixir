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
using Glaives.GameFramework;
using Glaives.Internal;
using Glaives.Internal.Graphics;
using Glaives.Internal.Windowing;
using OpenTK.Graphics.OpenGL;

namespace Glaives.Graphics
{
    public sealed class GraphicsDevice 
    {
        /// <summary>
        /// <para>The amount of draw calls to the GPU needed to render the level</para>
        /// <para>Actors with the same Shader, Texture, BlendMode and DrawLayer will be batched together into the same draw call</para>
        /// </summary>
        public int DrawCalls => _batches.Count;
        
        private Shader _postProcessingShader;
        /// <summary>
        /// <para>The shader used to apply full-screen effects</para>
        /// <para>It is applied after all drawing is done over the whole screen</para>
        /// </summary>
        public Shader PostProcessingShader
        {
            get => _postProcessingShader;
            set
            {
                if (_postProcessingShader != value)
                {
                    _postProcessingShader = value;
                    if (_frameBufferVertexShader != -1)
                    {
                        GL.DetachShader(_frameBufferShaderProgram, _frameBufferVertexShader);
                        GL.DeleteShader(_frameBufferVertexShader);
                    }

                    if (_frameBufferFragmentShader != -1)
                    {
                        GL.DetachShader(_frameBufferShaderProgram, _frameBufferFragmentShader);
                        GL.DeleteShader(_frameBufferFragmentShader);
                    }

                    _frameBufferVertexShader = value.VertexShaderHandle;
                    _frameBufferFragmentShader = value.FragmentShaderHandle;

                    GL.AttachShader(_frameBufferShaderProgram, _frameBufferVertexShader);
                    GL.AttachShader(_frameBufferShaderProgram, _frameBufferFragmentShader);
                    GL.LinkProgram(_frameBufferShaderProgram);

                    _positionAttributeLocation = GL.GetAttribLocation(_frameBufferShaderProgram, Shader.VertInPositionName);
                    _colorAttributeLocation = GL.GetAttribLocation(_frameBufferShaderProgram, Shader.VertInColorName);
                    _texCoordAttributeLocation = GL.GetAttribLocation(_frameBufferShaderProgram, Shader.VertInTexCoordName);

                    _frameBufferRenderProgram = new RenderProgram(BlendMode.Alpha, null, value, 0);
                }
            }
        }

        internal IEnumerable<GeometryBatch> Batches => _batches;

        private readonly List<DrawableActor> _drawables = new List<DrawableActor>();
        private readonly List<GeometryBatch> _batches = new List<GeometryBatch>();
        private readonly Window _window;

        private readonly RenderTarget _frameBuffer;
        private readonly int _frameBufferVbo;
        private readonly Vertex[] _frameBufferVertices; 
        private readonly int _frameBufferShaderProgram;
        private int _positionAttributeLocation;
        private int _colorAttributeLocation;
        private int _texCoordAttributeLocation;
        private RenderProgram _frameBufferRenderProgram;
        private int _frameBufferVertexShader = -1;
        private int _frameBufferFragmentShader = -1;
        private bool _batchesAreHot; // Whether or not the batches have "begun" but not "ended" yet

        internal GraphicsDevice(Window window)
        {
            _window = window;
            _frameBufferShaderProgram = GL.CreateProgram();
            _frameBufferVertices = Shape.MakeQuad();
            _frameBufferVbo = GL.GenBuffer();
            PostProcessingShader = Shader.Default;

            _frameBuffer = new RenderTarget(window.Size, TextureFilterMode.Smooth);
        }

        
        internal void AddDrawableActor(DrawableActor drawableActor)
        {
            _drawables.Add(drawableActor);
        }

        internal void RemoveDrawableActor(DrawableActor drawableActor)
        {
            _drawables.Remove(drawableActor);
        }

        private void RemoveBatch(GeometryBatch batch)
        {
            for (int i = _batches.Count - 1; i >= 0; i--)
            {
                if (_batches[i] == batch)
                {
                    _batches.RemoveAt(i);
                    break;
                }
            }
            SortBatchesByDrawLayer();
        }

        private void SortBatchesByDrawLayer()
        {
            _batches.Sort((b1, b2) => b1.RenderProgram.DrawLayer.CompareTo(b2.RenderProgram.DrawLayer));
        }

        internal void AddGeometry(Vertex[] vertices)
        {
            AddGeometry(vertices, RenderProgram.Default);
        }

        internal void AddGeometry(Vertex[] vertices, RenderProgram renderProgram)
        {
            // Find a batch with matching render programs or create a new one if it does not exist
            GeometryBatch batch = FindBatch(renderProgram) ?? CreateBatch(renderProgram);

            batch.AddVertices(vertices); 
        }
        
        private GeometryBatch FindBatch(RenderProgram renderProgram)
        {
            foreach (GeometryBatch batch in _batches)
            {
                if (batch.RenderProgram == renderProgram)
                {
                    return batch;
                }
            }

            return null;
        }

        private GeometryBatch CreateBatch(RenderProgram renderProgram)
        {
            GeometryBatch newBatch = new GeometryBatch(renderProgram);
            _batches.Add(newBatch);
            SortBatchesByDrawLayer();

            if (_batchesAreHot)
            {
                newBatch.Begin();
            }

            return newBatch;
        }

        internal void DrawBatches()
        {
            foreach(GeometryBatch batch in _batches)
            {
                batch.Begin(); // clears all vertices
            }

            _batchesAreHot = true;

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
                Vertex[] vertices = drawable.GetVertices();
                if (vertices != null)
                {
                    if (vertices.Length > 0)
                    {
                        drawable.Batch.AddVertices(vertices);
                    }
                }
            }

            // Remove empty batches
            for (int i = _batches.Count - 1; i >= 0; i--)
            {
                if (_batches[i].VertexArrayPosition == 0)
                {
                    _batches.RemoveAt(i);
                }
            }
            
            _frameBuffer.Bind();
            foreach (GeometryBatch batch in _batches)
            {
                // Draw all vertices to the frame buffer
                batch.End(this, _frameBuffer); 
            }
            _batchesAreHot = false;
            _frameBuffer.UnBind();

            // Draw the screen quad (final image) using the frame buffer color texture
            Viewport viewport = Engine.Get.Viewport;
            Draw(viewport.Size, _frameBufferShaderProgram, _frameBufferVertices, _frameBufferVertices.Length, _frameBufferVbo, 
                _positionAttributeLocation, _colorAttributeLocation, _texCoordAttributeLocation,
                 _frameBufferRenderProgram, Matrix.Identity, _frameBuffer.ColorTexture);
        }

        internal void Draw(IntVector2 viewport, int shaderProgram, Vertex[] vertices, int length, int vbo, 
            int posAttribLocation, int colorAttribLocation, int texCoordAttribLocation, 
            RenderProgram renderProgram, Matrix projection, int textureOverride = -1)
        {
            GL.Viewport(0, 0, viewport.X, viewport.Y);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.UseProgram(shaderProgram);
            GL.BindFragDataLocation(shaderProgram, 0, Shader.FragOutName);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertex.SizeInBytes * length, vertices, BufferUsageHint.StaticDraw);
            
            GL.EnableVertexAttribArray(posAttribLocation);
            GL.VertexAttribPointer(posAttribLocation, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 0);
            
            GL.EnableVertexAttribArray(colorAttribLocation);
            GL.VertexAttribPointer(colorAttribLocation, 4, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 2 * sizeof(float));
            
            GL.EnableVertexAttribArray(texCoordAttribLocation);
            GL.VertexAttribPointer(texCoordAttribLocation, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 6 * sizeof(float));

            int uniTrans = GL.GetUniformLocation(shaderProgram, Shader.VertUniTransformName);
            float[] trans = projection.ToMatrix4();
            GL.UniformMatrix4(uniTrans, 1, false, trans);

            GL.Enable(EnableCap.Blend);

            GL.BlendEquationSeparate(
                (BlendEquationMode)renderProgram.BlendMode.ColorEquation,
                (BlendEquationMode)renderProgram.BlendMode.AlphaEquation);

            GL.BlendFuncSeparate(
                (BlendingFactorSrc)renderProgram.BlendMode.ColorSrcFactor, (BlendingFactorDest)renderProgram.BlendMode.ColorDstFactor,
                (BlendingFactorSrc)renderProgram.BlendMode.AlphaSrcFactor, (BlendingFactorDest)renderProgram.BlendMode.AlphaDstFactor);

            int texture;
            if (textureOverride != -1)
            {
                texture = textureOverride;
            }
            else
            {
                if (renderProgram.Texture != null)
                {
                    texture = renderProgram.Texture.Handle;
                }
                else
                {
                    texture = Engine.Get.EngineContent.TextureWhite32x32.Handle;
                }
            }

            GL.Enable(EnableCap.Texture2D);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.DrawArrays(PrimitiveType.Quads, 0, length);

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.DisableVertexAttribArray(posAttribLocation);
            GL.DisableVertexAttribArray(colorAttribLocation);
            GL.DisableVertexAttribArray(texCoordAttribLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.UseProgram(0);
        }
    }
}
