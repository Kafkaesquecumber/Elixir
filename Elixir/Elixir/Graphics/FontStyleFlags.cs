using System;

namespace Elixir.Graphics
{
    [Flags]
    public enum FontStyleFlags
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4,
        Strikeout = 8,
    }
}