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
using Glaives.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Glaives.Internal.Graphics
{
    internal class GeometryBatch : IDisposable
    {
        internal readonly RenderProgram RenderProgram;
        
        private bool _begun;

        private readonly List<Vertex> _vertices = new List<Vertex>();
        private readonly int _shaderProgram; 

        private readonly int _vbo;
        private readonly int _vao;
        private readonly int _vertexShader;
        private readonly int _fragmentShader;

        internal GeometryBatch()
            : this(RenderProgram.Default)
        {
        }

        internal GeometryBatch(RenderProgram renderProgram)
        {
            RenderProgram = renderProgram;

            _shaderProgram = GL.CreateProgram();
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            _vertexShader = renderProgram.Shader.CreateVertexShader();
            _fragmentShader = renderProgram.Shader.CreateFragmentShader();

            if (_vertexShader != -1)
            {
                GL.AttachShader(_shaderProgram, _vertexShader);
            }

            if (_fragmentShader != -1)
            {
                GL.AttachShader(_shaderProgram, _fragmentShader);
            }

            GL.LinkProgram(_shaderProgram);
        }

        internal void AddVertex(Vertex vertex)
        {
            if(!_begun)
            {
                throw new GlaivesException("Vertices may not be added before Begin is called");
            }
            _vertices.Add(vertex);
        }

        internal void AddVertices(IEnumerable<Vertex> vertices)
        {
            _vertices.AddRange(vertices);
        }

        internal void Begin()
        {
            _vertices.Clear();
            _begun = true;
        }
        
        internal void End(GraphicsDevice device, RenderTarget renderTarget) 
        {
            if(!_begun)
            {
                throw new GlaivesException("End may not be called before Begin is called");
            }
            
            Vertex[] vertices = _vertices.ToArray(); 

            GL.PolygonMode(MaterialFace.Back, PolygonMode.Fill);
            device.Draw(renderTarget.Size, _shaderProgram, vertices, _vbo, RenderProgram, 
                Engine.Get.LevelManager.Level.CurrentView.ProjectionMatrix);
            
            _begun = false;
        }

        public void Dispose()
        {
            GL.DetachShader(_shaderProgram, _vertexShader);
            GL.DetachShader(_shaderProgram, _fragmentShader);

            GL.DeleteShader(_vertexShader);
            GL.DeleteShader(_fragmentShader);
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_vao);
            GL.DeleteProgram(_shaderProgram);
        }
    }
}
