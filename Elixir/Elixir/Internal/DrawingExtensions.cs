using Elixir.GameFramework;

namespace Elixir.Internal
{
    internal static class DrawingExtensions
    {
        internal static System.Drawing.Color ToDrawingColor(this Color color)
        {
            return System.Drawing.Color.FromArgb(
                (int)(color.A * 255),
                (int)(color.R * 255), 
                (int)(color.G * 255), 
                (int)(color.B * 255)
            );
        }
    }
}