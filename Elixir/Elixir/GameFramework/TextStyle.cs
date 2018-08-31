using System;

namespace Elixir.GameFramework
{
    [Flags]
    public enum TextStyle
    {
        Normal = 0,
        Bold = 1,
        Italic = 2,
        Underlined = 4,
        StrikeThrough = 8
    }
}