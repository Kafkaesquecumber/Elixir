using System;
using Elixir.GameFramework;
using Elixir.Graphics;

namespace Elixir.Internal.Interface.Implementation.OpenTKImpl
{
    internal static class OpenTKExtensions
    {
        
        internal static OpenTK.Color ToOpenTKColor(this Color color)
        {
            return new OpenTK.Color(
                (byte)(color.R * 255),
                (byte)(color.G * 255),
                (byte)(color.B * 255),
                (byte)(color.A * 255));
        }

        
    }
}
