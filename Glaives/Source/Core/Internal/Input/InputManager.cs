// MIT License
// 
// Copyright(c) 2018 Glaives Game Engine.
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

using System.Collections.Generic;
using System.Linq;
using Glaives.Core.Configuration;
using Glaives.Core.Input;
using Glaives.Core.Internal.Windowing;

namespace Glaives.Core.Internal.Input
{
    internal sealed class InputManager
    {
        internal List<RawInputAction> RawActionsBuffer = new List<RawInputAction>();
        internal List<RawInputAxis> RawAxesBuffer = new List<RawInputAxis>();

        private readonly InputSettings _settings;

        internal InputManager(Window window, InputSettings settings)
        {
            _settings = settings;
            window.InputActionEvent += BufferInputActionEvent;
            window.InputAxisEvent += BufferInputAxisEvent;
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
