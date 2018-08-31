using System;
using System.Collections.Generic;
using Elixir.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Elixir.Internal.Graphics
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
                throw new ElixirException("Vertices may not be added before Begin is called");
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
                throw new ElixirException("End may not be called before Begin is called");
            }
            
            Vertex[] vertices = _vertices.ToArray(); //TODO: Optimize to be an array in the first place
            device.Draw(renderTarget.Size, _shaderProgram, vertices, _vbo, RenderProgram, Engine.Get.LevelManager.Level.CurrentView.ProjectionMatrix);
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
