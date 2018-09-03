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

using System;
using System.Linq;
using Glaives.Input;
using Glaives.Internal;

namespace Glaives.Configuration
{
    [Serializable]
    public class InputSettings
    {
        private const float DefaultDeadzone = 0.85f;
        
        public float DeadzoneGamepadLeftThumbstickX { get; set; } = DefaultDeadzone;
        public float DeadzoneGamepadLeftThumbstickY { get; set; } = DefaultDeadzone;
        public float DeadzoneGamepadRightThumbstickX { get; set; } = DefaultDeadzone;
        public float DeadzoneGamepadRightThumbstickY { get; set; } = DefaultDeadzone;
        public float DeadzoneGamepadLeftTrigger { get; set; } = DefaultDeadzone;
        public float DeadzoneGamepadRightTrigger { get; set; } = DefaultDeadzone;

        private InputActionBinding[] _actionBindings;
        /// <summary>
        /// The input action bindings
        /// </summary>
        public InputActionBinding[] ActionBindings
        {
            get => _actionBindings;
            set
            {
                if (value.GroupBy(x => x.Id).Any(y => y.Count() > 1))
                {
                    throw new GlaivesException($"Duplicate {nameof(InputActionBinding)} id's are not allowed");
                }

                _actionBindings = value;
            }
        }

        private InputAxisBinding[] _axisBindings;
        /// <summary>
        /// The input axis bindings
        /// </summary>
        public InputAxisBinding[] AxisBindings
        {
            get => _axisBindings;
            set
            {
                if (value.GroupBy(x => x.Id).Any(y => y.Count() > 1))
                {
                    throw new GlaivesException($"Duplicate {nameof(InputAxisBinding)} id's are not allowed");
                }

                _axisBindings = value;
            }
        }

        public InputSettings()
        {
            
        }
        
        /// <summary>
        /// The deadzone for a specified axis
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public float GetDeadzoneForAxis(InputAxis axis)
        {
            switch (axis)
            {
                case InputAxis.GamepadLeftThumbstickX:
                    return DeadzoneGamepadLeftThumbstickX;
                case InputAxis.GamepadLeftThumbstickY:
                    return DeadzoneGamepadLeftThumbstickY;
                case InputAxis.GamepadRightThumbstickX:
                    return DeadzoneGamepadRightThumbstickX;
                case InputAxis.GamepadRightThumbstickY:
                    return DeadzoneGamepadRightThumbstickY;
                case InputAxis.GamepadLeftTrigger:
                    return DeadzoneGamepadLeftTrigger;
                case InputAxis.GamepadRightTrigger:
                    return DeadzoneGamepadRightTrigger;
                default:
                    return 0.0f;
            }
        }
    }
}
