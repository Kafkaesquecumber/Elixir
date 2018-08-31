
using Elixir.GameFramework;
using Elixir.Graphics;

// Note: do not use the ContentLoader in this class, it has not been created yet (by design)

namespace Elixir.Internal.Content
{
    internal class DefaultContent
    {
        internal Texture TextureWhite32x32;

        internal DefaultContent()
        {
            TextureWhite32x32 = new Texture(32, 32, Color.White, new TextureCreateOptions(TextureFilterMode.Nearest, TextureWrapMode.ClampToEdge));
        }
    }
}
