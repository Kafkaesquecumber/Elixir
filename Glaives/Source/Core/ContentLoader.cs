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
using System.Linq;
using System.Reflection;
using Glaives.Core.Graphics;
using Glaives.Core.Internal.Content;

namespace Glaives.Core
{
    /// <summary>
    /// <para>Loads and caches content files</para>
    /// <para>When loading content, the content loader will first check if this file was loaded previously (with the same create options)</para>
    /// <para>If it is present in the content cache it will return the cached content, otherwise it will load it and add it to the content cache</para>
    /// </summary>
    public class ContentLoader
    {
        private readonly Dictionary<ContentInfo, LoadableContent> _contentCache = new Dictionary<ContentInfo, LoadableContent>();

        internal ContentLoader()
        {
        }

        /// <summary>
        /// Load a font file (supported formats: TTF and OTF)
        /// </summary>
        /// <param name="file">The font file</param>
        /// <param name="createOptions">The create options used to load the font</param>
        /// <returns></returns>
        public Font LoadFont(string file, FontCreateOptions createOptions)
        {
            return Load<Font>(new [] { file }, createOptions);
        }

        /// <summary>
        /// Load a texture (supported formats: Png, Jpeg, Bmp and Gif)
        /// </summary>
        /// <param name="file">The texture file to load</param>
        /// <param name="createOptions">The create options used to load the texture</param>
        /// <returns></returns>
        public Texture LoadTexture(string file, TextureCreateOptions createOptions)
        {
            return Load<Texture>(new [] { file }, createOptions);
        }

        /// <summary>
        /// Return loaded or cached shader
        /// </summary>
        /// <param name="vertexShaderFile">The vertex shader file to load</param>
        /// <param name="fragmentShaderFile">The fragment shader file to load</param>
        /// <returns>The shader</returns>
        public Shader LoadShader(string vertexShaderFile, string fragmentShaderFile)
        {
            return Load<Shader>(new [] { vertexShaderFile, fragmentShaderFile }, null);
        }

        /// <summary>
        /// Disposes all cached content
        /// </summary>
        public void DisposeAllContent()
        {
            foreach (KeyValuePair<ContentInfo, LoadableContent> content in _contentCache)
            {
                content.Value.Dispose();
            }
            _contentCache.Clear();
            GC.Collect();
        }

        // files is an array as oppose to just 1 file because things like shaders consist of multiple files
        private T Load<T>(string[] files, ContentCreateOptions createOptions) where T : LoadableContent
        {
            object[] ctorParams;

            if (createOptions != null)
            {
                // Files + 1 for create options
                ctorParams = new object[files.Length + 1];

                // Assign last element for create options
                ctorParams[ctorParams.Length - 1] = createOptions;
            }
            else
            {
                // No create options just allocate space for files
                ctorParams = new object[files.Length];
            }

            // Assign files to constructor parameters
            for (int i = 0; i < files.Length; i++)
            {
                ctorParams[i] = files[i];
            }

            // Check if we have this content cached before loading a new one
            LoadableContent cached = GetCachedContent(files, createOptions);
            if (cached != null)
            {
                return (T)cached;
            }

            // Create content instance from constructor parameters
            const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            LoadableContent content = (LoadableContent)Activator.CreateInstance(typeof(T), bindingFlags, null, ctorParams, null);

            // Cache the new content
            _contentCache.Add(new ContentInfo(files, createOptions), content);

            string logString = $"Loaded {typeof(T).Name}";
            logString = files.Aggregate(logString, (current, file) => current + $" '{file}'");

            if (createOptions != null)
            {
                logString += $" {createOptions}";
            }

            Engine.Get.Debug.Info(logString);
            return (T)content;
        }
        
        private LoadableContent GetCachedContent(string[] files, ContentCreateOptions createOptions)
        {
            LoadableContent cached = null;
            ContentInfo? contentInfo = null;

            foreach (KeyValuePair<ContentInfo, LoadableContent> loadableContent in _contentCache)
            {
                if (loadableContent.Key.Files.SequenceEqual(files))
                {
                    if (createOptions != null)
                    {
                        if (loadableContent.Key.CreateOptions.IsEqualContentInternal(createOptions)) 
                        {
                            cached = loadableContent.Value;
                            contentInfo = loadableContent.Key;
                            break;
                        }
                    }
                    else
                    {
                        contentInfo = loadableContent.Key;
                        cached = loadableContent.Value;
                        break;
                    }
                }
            }

            if (cached == null)
            {
                return null;
            }
            
            if (cached.IsDisposed)
            {
                _contentCache.Remove(contentInfo.Value);
                return null;
            }
            return cached;
        }
    }
}