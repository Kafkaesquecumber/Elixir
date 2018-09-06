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
using Glaives.GameFramework;
using Glaives.Internal;
using OpenTK.Graphics.OpenGL;

namespace Glaives.Graphics
{
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
        
        private static Shader _defaultShader;
        public static Shader Default
        {
            get
            {
                if(_defaultShader == null)
                {
                    _defaultShader = FromString(DefaultFragmentShaderString);
                }
                return _defaultShader;
            }
        }

        /// <summary>
        /// Construct a new shader from memory directly
        /// </summary>
        /// <param name="pixelShaderString"></param>
        /// <returns></returns>
        public static Shader FromString(string pixelShaderString)
        {
            Shader shader = new Shader
            {
                PixelShaderString = pixelShaderString
            };
            shader.LoadShaders();
            return shader;
        }
        
        public string PixelShaderString { get; private set; }

        // We dont dispose shaders
        internal override bool IsDisposed => false;

        internal int VertexShaderHandle { get; private set; }
        internal int FragmentShaderHandle { get; private set; }

        // Used for the FromString method
        private Shader() { }

        /// <summary>
        /// <para>Create a new shader instance from file</para>
        /// <para>Actors with different shader instances will be drawn in different draw calls</para>
        /// <para>To reduce draw calls, use the same shader instance for multiple actors</para>
        /// </summary>
        /// <param name="pixelShaderFile"></param>
        internal Shader(string pixelShaderFile)
        {
            if (!File.Exists(pixelShaderFile))
            {
                throw new GlaivesException($"No shader file exists in path '{pixelShaderFile}'");
            }

            PixelShaderString = File.ReadAllText(pixelShaderFile);
            LoadShaders();
        }

        private void LoadShaders()
        {
            VertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShaderHandle, VertexShaderString);
            GL.CompileShader(VertexShaderHandle);
            GL.GetShader(VertexShaderHandle, ShaderParameter.CompileStatus, out int vertexShaderCompileStatus);
            if (vertexShaderCompileStatus != 1)
            {
                GL.GetShaderInfoLog(VertexShaderHandle, out string vertexShaderInfo);
                Engine.Get.Debug.Error("Failed to compile vertex shader\n" + vertexShaderInfo);
            }

            if (!EvaluateFragmentShaderString())
            {
                Engine.Get.Debug.Error("Failed to compile pixel shader");
            }

            FragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShaderHandle, PixelShaderString);
            GL.CompileShader(FragmentShaderHandle);
            GL.GetShader(FragmentShaderHandle, ShaderParameter.CompileStatus, out int fragmentShaderCompileStatus);
            if (fragmentShaderCompileStatus != 1)
            {
                GL.GetShaderInfoLog(FragmentShaderHandle, out string fragmentShaderInfo);
                Engine.Get.Debug.Error("Failed to compile pixel shader\n" + fragmentShaderInfo);
            }
        }
        
        private bool EvaluateFragmentShaderString()
        {
            string[] requirements = 
            {
                $"in vec4 {VertOutFragInColorName};",
                $"in vec2 {VertOutFragInTexCoordName};",
                $"out vec4 {FragOutName};"
            };

            bool valid = true;
            foreach (string requirement in requirements)
            {
                if (!PixelShaderString.Contains(requirement))
                {
                    Engine.Get.Debug.Error($"Pixel shader is missing required line: {requirement}");
                    valid = false;
                }
            }

            string unspaced = PixelShaderString.Replace(" ", "");
            if (!unspaced.Contains($"{FragOutName}="))
            {
                Engine.Get.Debug.Error($"Pixel shader is missing out color assignment ({FragOutName} = <your vec4 value>), " +
                                     $"out color name must be '{FragOutName}' and should not be declared");
                valid = false;
            }

            return valid;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            GL.DeleteShader(VertexShaderHandle);
            GL.DeleteShader(FragmentShaderHandle);
        }

        internal static string VertexShaderString =
        $@"
        #version 150 core
        in vec2 {VertInPositionName};
        in vec4 {VertInColorName};
        in vec2 {VertInTexCoordName};

        out vec4 {VertOutFragInColorName};
        out vec2 {VertOutFragInTexCoordName};

        uniform mat4 {VertUniTransformName};
        void main()
        {{
            {VertOutFragInColorName} = {VertInColorName};
            {VertOutFragInTexCoordName} = {VertInTexCoordName};
            gl_Position = {VertUniTransformName} * vec4({VertInPositionName}, 0.0, 1.0);
        }}";

        internal static string DefaultFragmentShaderString =
        $@" 
        #version 150 core 
        in vec4 {VertOutFragInColorName};
        in vec2 {VertOutFragInTexCoordName};
        out vec4 {FragOutName};

        uniform sampler2D textureSampler;
        void main() 
        {{ 
            {FragOutName} = texture(textureSampler, {VertOutFragInTexCoordName}.xy) * {VertOutFragInColorName};
        }}";

        
    }
}
