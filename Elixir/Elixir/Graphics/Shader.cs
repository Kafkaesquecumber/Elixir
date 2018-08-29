﻿using System;
using System.IO;
using Elixir.GameFramework;
using Elixir.Internal;
using OpenTK.Graphics.OpenGL;

namespace Elixir.Graphics
{
    public class Shader : ILoadableContent
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
            return shader;
        }
        
        public string PixelShaderString { get; private set; }

        private bool _disposed;

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
                throw new ElixirException($"No shader file exists in path '{pixelShaderFile}'");
            }

            PixelShaderString = File.ReadAllText(pixelShaderFile);
        }
        
        internal int CreateVertexShader()
        {
            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, VertexShaderString);
            GL.CompileShader(vertexShaderHandle);
            GL.GetShader(vertexShaderHandle, ShaderParameter.CompileStatus, out int vertexShaderCompileStatus);
            if (vertexShaderCompileStatus != 1)
            {
                GL.GetShaderInfoLog(vertexShaderHandle, out string vertexShaderInfo);
                Engine.Get.Debug.Error("Failed to compile vertex shader\n" + vertexShaderInfo);
            }
          
            return vertexShaderHandle;
        }

        internal int CreateFragmentShader()
        {
            if (!EvaluateFragmentShaderString())
            {
                Engine.Get.Debug.Error("Failed to compile pixel shader");
                return -1;
            }

            int fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderHandle, PixelShaderString);
            GL.CompileShader(fragmentShaderHandle);
            GL.GetShader(fragmentShaderHandle, ShaderParameter.CompileStatus, out int fragmentShaderCompileStatus);
            if (fragmentShaderCompileStatus != 1)
            {
                GL.GetShaderInfoLog(fragmentShaderHandle, out string fragmentShaderInfo);
                Engine.Get.Debug.Error("Failed to compile pixel shader\n" + fragmentShaderInfo);
            }
            
            return fragmentShaderHandle;
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

        public void Dispose()
        {
            _disposed = true;
        }

        public bool IsDisposed()
        {
            return _disposed;
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
