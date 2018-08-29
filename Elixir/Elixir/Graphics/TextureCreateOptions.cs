namespace Elixir.Graphics
{
    /// <summary>
    /// The import options to use for the texture
    /// </summary>
    public struct TextureCreateOptions
    {
        /// <summary>
        /// <para>FilterMode : Linear</para>
        /// <para>WrapMode : ClampToEdge</para>
        /// </summary>
        public static TextureCreateOptions Smooth => new TextureCreateOptions(TextureFilterMode.Linear, TextureWrapMode.ClampToEdge);

        /// <summary>
        /// <para>FilterMode : Nearest</para>
        /// <para>WrapMode : ClampToEdge</para>
        /// </summary>
        public static TextureCreateOptions Sharp => new TextureCreateOptions(TextureFilterMode.Nearest, TextureWrapMode.ClampToEdge);

        /// <summary>
        /// The way the texture is filtered
        /// </summary>
        public readonly TextureFilterMode FilterMode;

        /// <summary>
        /// The way the texture is wrapped outside of the texture coordinates
        /// </summary>
        public readonly TextureWrapMode WrapMode;

        /// <summary>
        /// Create new texture create options
        /// </summary>
        /// <param name="filterMode">The way the texture is filtered</param>
        /// <param name="wrapMode">The way the texture is wrapped outside of the texture coordinates</param>
        public TextureCreateOptions(TextureFilterMode filterMode, TextureWrapMode wrapMode)
        {
            FilterMode = filterMode;
            WrapMode = wrapMode;
        }
    }
}