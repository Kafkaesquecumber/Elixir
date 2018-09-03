// MIT License
// 
// Copyright(c) 2018 
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

namespace Glaives.Input
{
    public enum Key : int
    {
        Unknown,
        Space,
        Quote,  // '
        Comma,  // ,
        Minus,  // -
        Period,  // .
        Slash,  // /
        Key0,
        Key1,
        Key2,
        Key3,
        Key4,
        Key5,
        Key6,
        Key7,
        Key8,
        Key9,
        SemiColon,  // ;
        Equal,  // =
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z,
        LeftBracket,  // [
        Backslash,  // \
        RightBracket,  // ]
        GraveAccent,  // `
        World1, // Non-US #1
        Tilde, // Non-US #2
        Escape,
        Enter,
        Tab,
        Backspace,
        Insert,
        Delete,
        Right,
        Left,
        Down,
        Up,
        PageUp,
        PageDown,
        Home,
        End,
        CapsLock,
        ScrollLock,
        NumLock,
        PrintScreen,
        Pause,
        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7, 
        F8, 
        F9,
        F10,
        F11,
        F12,
        F13,
        F14,
        F15,
        F16,
        F17,
        F18,
        F19,
        F20,
        F21,
        F22,
        F23,
        F24,
        Numpad0,
        Numpad1,
        Numpad2,
        Numpad3,
        Numpad4,
        Numpad5,
        Numpad6,
        Numpad7,
        Numpad8,
        Numpad9,
        NumpadDecimal,
        NumpadDivide,
        NumpadMultiply,
        NumpadSubtract,
        NumpadAdd,
        NumpadEnter,
        NumpadEqual, 
        LeftShift,
        LeftControl,
        LeftAlt,
        LeftSuper,
        RightShift,
        RightControl,
        RightAlt,
        RightSuper,
        Menu,
        GamepadFaceButtonTop,
        GamepadFaceButtonBottom,
        GamepadFaceButtonRight,
        GamepadFaceButtonLeft,
        GamepadStart,
        GamepadSelect,
        GamepadRightShoulder,
        GamepadLeftShoulder,
        GamepadDPadUp,
        GamepadDPadDown,
        GamepadDPadRight,
        GamepadDPadLeft,
        /// <summary>
        /// The "press" of the thumbstick
        /// </summary>
        GamepadRightThumbstick,
        /// <summary>
        /// The "press" of the thumbstick
        /// </summary>
        GamepadLeftThumbstick,
        /// <summary>
        /// Triggered when the thumbstick's X-axis reaches a threshold
        /// </summary>
        GamepadRightThumbstickX,
        /// <summary>
        /// Triggered when the thumbstick's Y-axis reaches a threshold
        /// </summary>
        GamepadRightThumbstickY,
        /// <summary>
        /// Triggered when the thumbstick's X-axis reaches a threshold
        /// </summary>
        GamepadLeftThumbstickX,
        /// <summary>
        /// Triggered when the thumbstick's Y-axis reaches a threshold
        /// </summary>
        GamepadLeftThumbstickY,
        /// <summary>
        /// Triggered when the trigger axis has reached a threshold
        /// </summary>
        GamepadRightTrigger,
        /// <summary>
        /// Triggered when the trigger axis has reached a threshold
        /// </summary>
        GamepadLeftTrigger
    }
}
