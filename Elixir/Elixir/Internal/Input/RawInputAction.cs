using Elixir.Input;

namespace Elixir.Internal.Input
{
    internal struct RawInputAction
    {
        internal KeyState KeyState;
        internal Key Key;
        internal int GamepadId;
    }
}