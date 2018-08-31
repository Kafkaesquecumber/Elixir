using System;
using System.Collections.Generic;
using System.Reflection;
using Elixir.Graphics;
using Elixir.Internal.Content;

namespace Elixir.GameFramework
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
        /// Return loaded or cached font
        /// </summary>
        /// <param name="file">The font file</param>
        /// <param name="createOptions">The create options</param>
        /// <returns></returns>
        public Font LoadFont(string file, FontCreateOptions createOptions)
        {
            return Load<Font>(file, createOptions);
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

        private T Load<T>(string file, object importOptions) where T : LoadableContent
        {
            object[] ctorParams = importOptions == null ? new object[] { file } : new[] { file, importOptions };
            object createOptions = ctorParams.Length < 2 ? null : ctorParams[1];

            LoadableContent cached = GetCachedContent(file, createOptions);
            if (cached != null)
            {
                return (T)cached;
            }

            const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            LoadableContent content = (LoadableContent)Activator.CreateInstance(typeof(T), bindingFlags, null, ctorParams, null);

            _contentCache.Add(new ContentInfo(file, createOptions), content);

            Engine.Get.Debug.Info(createOptions != null
                ? $"Loaded {typeof(T).Name} '{file}' {createOptions}"
                : $"Loaded {typeof(T).Name} '{file}'");
            return (T)content;
        }

        private LoadableContent GetCachedContent(string file, object createOptions)
        {
            LoadableContent cached = null;
            ContentInfo? contentInfo = null;

            foreach (KeyValuePair<ContentInfo, LoadableContent> loadableContent in _contentCache)
            {
                if (loadableContent.Key.File == file)
                {
                    if (createOptions != null)
                    {
                        if (loadableContent.Key.CreateOptions == createOptions)
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