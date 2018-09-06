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
using Glaives.Core.Graphics;
using Glaives.Core.Internal;
using OpenTK.Graphics.OpenGL;

namespace Glaives.Core.Internal.Graphics
{
    internal class GeometryBatch : IDisposable
    {
        private const int InitialVertexArraySize = 512;

        internal readonly RenderProgram RenderProgram;
        
        private bool _begun;

        private Vertex[] _vertexArray = new Vertex[InitialVertexArraySize];
        private readonly int _shaderProgram; 

        private readonly int _vbo;
        private readonly int _vao;
        private readonly int _vertexShader;
        private readonly int _fragmentShader;

        private readonly int _positionAttributeLocation;
        private readonly int _colorAttributeLocation;
        private readonly int _texCoordAttributeLocation;

        /// <summary>
        /// Is the same as the amount of vertices to be drawn by this batch
        /// </summary>
        internal int VertexArrayPosition { get; private set; }

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

            _vertexShader = renderProgram.Shader.VertexShaderHandle;
            _fragmentShader = renderProgram.Shader.FragmentShaderHandle;

            if (_vertexShader != -1)
            {
                GL.AttachShader(_shaderProgram, _vertexShader);
            }

            if (_fragmentShader != -1)
            {
                GL.AttachShader(_shaderProgram, _fragmentShader);
            }

            GL.LinkProgram(_shaderProgram);
            
            _positionAttributeLocation = GL.GetAttribLocation(_shaderProgram, Shader.VertInPositionName);
            _colorAttributeLocation = GL.GetAttribLocation(_shaderProgram, Shader.VertInColorName);
            _texCoordAttributeLocation = GL.GetAttribLocation(_shaderProgram, Shader.VertInTexCoordName);
        }

        internal void AddVertex(Vertex vertex)
        {
            if(!_begun)
            {
                throw new GlaivesException("Vertices may not be added before Begin is called");
            }
            
            if (VertexArrayPosition >= _vertexArray.Length - 1)
            {
                // Vertex array is too small, make it bigger first
                Array.Resize(ref _vertexArray, _vertexArray.Length * 2);
            }

            _vertexArray[VertexArrayPosition++] = vertex;
        }

        internal void AddVertices(Vertex[] vertices)
        {
            while (VertexArrayPosition >= _vertexArray.Length - vertices.Length)
            {
                Array.Resize(ref _vertexArray, _vertexArray.Length * 2);
            }

            Array.Copy(vertices, 0, _vertexArray, VertexArrayPosition, vertices.Length);
            VertexArrayPosition += vertices.Length;
        }

        internal void Begin()
        {
            // _vertexArrayPosition is used as length, no need to clear array
            // This gives us a bit of performance
            VertexArrayPosition = 0;
            _begun = true;
        }
        
        internal void End(GraphicsDevice device, RenderTarget renderTarget) 
        {
            if(!_begun)
            {
                throw new GlaivesException("End may not be called before Begin is called");
            }
            
            GL.PolygonMode(MaterialFace.Back, PolygonMode.Fill);

            device.Draw(renderTarget.Size, _shaderProgram, _vertexArray, VertexArrayPosition, _vbo, 
                _positionAttributeLocation, _colorAttributeLocation, _texCoordAttributeLocation, 
                RenderProgram, Engine.Get.LevelManager.Level.CurrentView.ProjectionMatrix);
            
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
