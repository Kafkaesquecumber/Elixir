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

using System.IO;
using Glaives.Core.Internal;
using OpenTK.Graphics.OpenGL;

namespace Glaives.Core.Graphics
{
    /// <summary>
    /// <para>A shader is a set of GLSL scripts that run on the GPU</para>
    /// <para>Glaives uses special variable names to identify different "in" and "out" variables</para>
    /// <para>To create your own shaders, follow the instruction at: http://www.TODOPLACELINKHERE.net/Shaders </para>
    /// </summary>
    public class Shader : LoadableContent
    {
        // vertex shader in-attributes
        internal const string VertInPositionName  = "_position";
        internal const string VertInColorName     = "_color";
        internal const string VertInTexCoordName  = "_texCoord";

        // vertex shader out-attributes / fragment shader in-attributes
        internal const string VertOutFragInColorName      = "color";
        internal const string VertOutFragInTexCoordName   = "texCoord";

        // vertex shader uniforms
        internal const string VertUniTransformName = "transform";

        // fragment shader final out
        internal const string FragOutName = "finalColor";

        /// <summary>
        /// A simple textured vertex and fragment shader
        /// </summary>
        public static Shader Default => Engine.Get.EngineContent.ShaderTextured;

        /// <summary>
        /// Construct a new shader from memory directly
        /// </summary>
        /// <param name="vertexShaderString"></param>
        /// <param name="fragmentShaderString"></param>
        /// <returns></returns>
        public static Shader FromString(string vertexShaderString, string fragmentShaderString)
        {
            
            Shader shader = new Shader
            {
                _vertexShaderString =  vertexShaderString,
                _fragmentShaderString = fragmentShaderString
            };
            shader.LoadShaders();
            return shader;
        }

        private string _fragmentShaderString;
        private string _vertexShaderString;

        internal override bool IsDisposed => VertexShaderHandle == 0;

        internal int VertexShaderHandle { get; private set; }
        internal int FragmentShaderHandle { get; private set; }

        // Used for the FromString method
        private Shader() { }

        /// <summary>
        /// <para>Create a new shader instance from files</para>
        /// <para>Actors with different shader instances will be drawn in different draw calls</para>
        /// <para>To reduce draw calls, use the same shader instance for multiple actors</para>
        /// </summary>
        /// <param name="vertexShaderFile"></param>
        /// <param name="fragmentShaderFile"></param>
        internal Shader(string vertexShaderFile, string fragmentShaderFile)
        {
            if (!File.Exists(fragmentShaderFile))
            {
                throw new GlaivesException($"No shader file exists in path '{fragmentShaderFile}'");
            }

            _vertexShaderString = File.ReadAllText(vertexShaderFile);
            _fragmentShaderString = File.ReadAllText(fragmentShaderFile);
            LoadShaders();
        }

        private void LoadShaders()
        {
            // Create and compile the vertex shader
            VertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShaderHandle, _vertexShaderString);
            GL.CompileShader(VertexShaderHandle);
            GL.GetShader(VertexShaderHandle, ShaderParameter.CompileStatus, out int vertexShaderCompileStatus);
            if (vertexShaderCompileStatus != 1)
            {
                GL.GetShaderInfoLog(VertexShaderHandle, out string vertexShaderInfo);
                Engine.Get.Debug.Error("Failed to compile vertex shader\n" + vertexShaderInfo);
            }

            // Create and compile the fragment shader
            FragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShaderHandle, _fragmentShaderString);
            GL.CompileShader(FragmentShaderHandle);
            GL.GetShader(FragmentShaderHandle, ShaderParameter.CompileStatus, out int fragmentShaderCompileStatus);
            if (fragmentShaderCompileStatus != 1)
            {
                GL.GetShaderInfoLog(FragmentShaderHandle, out string fragmentShaderInfo);
                Engine.Get.Debug.Error("Failed to compile fragment shader\n" + fragmentShaderInfo);
            }

            // Remove the strings from memory
            _vertexShaderString = null;
            _fragmentShaderString = null;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            GL.DeleteShader(VertexShaderHandle);
            GL.DeleteShader(FragmentShaderHandle);
        }
    }
}
