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

using System.Collections.Generic;
using Glaives.Core.Internal.Content;
using Glaives.Core.Internal.Graphics;
using Glaives.Core.Internal.Windowing;
using OpenTK.Graphics.OpenGL;

namespace Glaives.Core.Graphics
{
    public abstract class GraphicsDeviceBase 
    {
        /// <summary>
        /// <para>The amount of draw calls to the GPU needed to render the level</para>
        /// <para>Actors with the same Shader, Texture, BlendMode and DrawLayer will be batched together into the same draw call</para>
        /// </summary>
        public int DrawCalls => Batches.Count;

        internal readonly List<GeometryBatch> Batches = new List<GeometryBatch>();
        internal readonly Window Window;

        internal bool BatchesAreHot; // Whether or not the batches have "begun" but not "ended" yet

        internal GraphicsDeviceBase(Window window)
        {
            Window = window;
        }

        private void RemoveBatch(GeometryBatch batch)
        {
            for (int i = Batches.Count - 1; i >= 0; i--)
            {
                if (Batches[i] == batch)
                {
                    Batches.RemoveAt(i);
                    break;
                }
            }
            SortBatchesByDrawLayer();
        }

        internal void SortBatchesByDrawLayer()
        {
            Batches.Sort((b1, b2) => b1.RenderProgram.DrawLayer.CompareTo(b2.RenderProgram.DrawLayer));
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

        internal GeometryBatch FindBatch(RenderProgram renderProgram)
        {
            foreach (GeometryBatch batch in Batches)
            {
                if (batch.RenderProgram == renderProgram)
                {
                    return batch;
                }
            }

            return null;
        }

        internal GeometryBatch CreateBatch(RenderProgram renderProgram)
        {
            GeometryBatch newBatch = new GeometryBatch(renderProgram);
            Batches.Add(newBatch);
            SortBatchesByDrawLayer();

            if (BatchesAreHot)
            {
                newBatch.Begin();
            }

            return newBatch;
        }

        internal abstract void DrawBatches();
        
        internal static void Draw(IntRect viewport, int shaderProgram, Vertex[] vertices, int length, int vbo,
            int posAttribLocation, int colorAttribLocation, int texCoordAttribLocation,
            RenderProgram renderProgram, Matrix projection, int textureOverride = -1)
        {
            GL.Viewport(viewport.X, viewport.Y, viewport.Width, viewport.Height);

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

            int texture = 0;
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
                    texture = GlobalContent.Get.TextureWhite32x32.Handle;
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
