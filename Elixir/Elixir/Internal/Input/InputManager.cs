
using System.Collections.Generic;
using System.Linq;
using Elixir.Configuration;
using Elixir.Input;
using Elixir.Internal.Windowing;

namespace Elixir.Internal.Input
{
    internal sealed class InputManager
    {
        internal List<RawInputAction> RawActionsBuffer = new List<RawInputAction>();
        internal List<RawInputAxis> RawAxesBuffer = new List<RawInputAxis>();

        private readonly InputSettings _settings;

        internal InputManager(Window window, InputSettings settings)
        {
            _settings = settings;
            window.Interface.InputActionEvent += BufferInputActionEvent;
            window.Interface.InputAxisEvent += BufferInputAxisEvent;
        }

        private void BufferInputActionEvent(KeyState keyState, Key key, int gamepadId)
        {
            RawActionsBuffer.Add(new RawInputAction
            {
                Key = key,
                KeyState = keyState,
                GamepadId = gamepadId
            });
        }

        private void BufferInputAxisEvent(InputAxis axis, float value, int gamepadId)
        {
            RawAxesBuffer.Add(new RawInputAxis
            {
                Axis = axis,
                Value = value,
                GamepadId = gamepadId
            });
        }

        internal IEnumerable<RawInputAction> GetRawActionsByBindingId(string actionBindingId)
        {
            return (
                from inputActionBinding in _settings.ActionBindings
                where inputActionBinding.Id == actionBindingId
                from rawInputAction in RawActionsBuffer
                where inputActionBinding.Keys.Contains(rawInputAction.Key)
                select rawInputAction).ToList();
        }

        internal IEnumerable<RawInputAxis> GetRawAxesByBindingId(string axisBindingId)
        {
            List<RawInputAxis> result = new List<RawInputAxis>();
            foreach (InputAxisBinding inputAxisBinding in _settings.AxisBindings)
            {
                if (inputAxisBinding.Id == axisBindingId)
                {
                    foreach (RawInputAxis rawInputAxis in RawAxesBuffer)
                    {
                        foreach (InputAxisScalePair inputAxisScalePair in inputAxisBinding.AxisScalePairs)
                        {
                            if (inputAxisScalePair.Axis == rawInputAxis.Axis)
                            {
                                result.Add(rawInputAxis);
                            }
                        }
                    }
                }
            }

            return result;
        }

        internal void Flush()
        {
            RawActionsBuffer.Clear();
            RawAxesBuffer.Clear();
        }
    }
}
