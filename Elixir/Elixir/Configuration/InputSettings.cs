using System;
using System.Linq;
using Elixir.Input;
using Elixir.Internal;

namespace Elixir.Configuration
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
                    throw new SgeException($"Duplicate {nameof(InputActionBinding)} id's are not allowed");
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
                    throw new SgeException($"Duplicate {nameof(InputAxisBinding)} id's are not allowed");
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
