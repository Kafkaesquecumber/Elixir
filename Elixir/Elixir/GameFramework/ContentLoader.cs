using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Elixir.Graphics;
using Elixir.Internal;

namespace Elixir.GameFramework
{
    /// <summary>
    /// <para>Loads and caches content files</para>
    /// <para>When loading content, the content loader will first check if this file was loaded previously</para>
    /// <para>If it is present in the content cache it will return the cached content, otherwise it will load it and add it to the content cache</para>
    /// </summary>
    public class ContentLoader
    {
        private readonly Dictionary<string, ILoadableContent> _contentCache = new Dictionary<string, ILoadableContent>();

        internal ContentLoader()
        {
        }

        /// <summary>
        /// Return loaded or cached texture
        /// </summary>
        /// <param name="file">The texture file to load</param>
        /// <param name="createOptions">The create options</param>
        /// <returns></returns>
        public Texture LoadTexture(string file, TextureCreateOptions createOptions)
        {
            return Load<Texture>(file, createOptions);
        }

        /// <summary>
        /// Return loaded or cached shader
        /// </summary>
        /// <param name="file">The shader file to load</param>
        /// <returns></returns>
        public Shader LoadShader(string file)
        {
            return Load<Shader>(file, null);
        }

        private T Load<T>(string file, object importOptions)
        {
            ILoadableContent cached = GetCachedContent(file);
            if (cached != null)
            {
                return (T)cached;
            }

            const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            object[] ctorParams = importOptions == null ? new object[] { file } : new[] { file, importOptions };
            ILoadableContent content = (ILoadableContent)Activator.CreateInstance(typeof(T), bindingFlags, null, ctorParams, null);
            _contentCache.Add(file, content);
            return (T)content;
        }

        private ILoadableContent GetCachedContent(string file)
        {
            if (_contentCache.ContainsKey(file))
            {
                ILoadableContent content = _contentCache[file];
                if (content.IsDisposed())
                {
                    _contentCache.Remove(file);
                    return null;
                }
                return content;
            }

            return null;
        }
    }
}