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
using Glaives.Core.Internal;

namespace Glaives.Core.Input
{
    public sealed class InputBinder
    {
        public delegate void InputActionEvent(KeyState keyState, Key key, int gamepadId);
        public delegate void InputAxisEvent(InputAxis axis, float value, int gamepadId);
        
        internal readonly Dictionary<string, InputActionEvent> InputActionEvents = new Dictionary<string, InputActionEvent>();
        internal readonly Dictionary<string, InputAxisEvent> InputAxisEvents = new Dictionary<string, InputAxisEvent>();
        
        internal InputBinder()
        {
            
        }

        /// <summary>
        /// Bind an input action to a function that handles the input event
        /// </summary>
        /// <param name="inputActionId">The id of the input action binding</param>
        /// <param name="eventHandler">The function that will handle this input action event</param>
        public void BindAction(string inputActionId, InputActionEvent eventHandler)
        {
            if (Engine.Get.Settings.Input.ActionBindings.Count(x => x.Id == inputActionId) == 0)
            {
                throw new GlaivesException($"No action binding exists in input settings with id '{inputActionId}'");
            }

            if (InputActionEvents.ContainsKey(inputActionId))
            {
                throw new GlaivesException($"Failed to bind '{inputActionId}', can not bind to the same input action multiple times for the same actor");
            }
            InputActionEvents.Add(inputActionId, eventHandler);
        }

        /// <summary>
        /// Bind an input axis to a function that handles the input event
        /// </summary>
        /// <param name="inputAxisId">The id of the input axis binding</param>
        /// <param name="eventHandler">The function that will handle this input axis event</param>
        public void BindAxis(string inputAxisId, InputAxisEvent eventHandler)
        {
            if (Engine.Get.Settings.Input.AxisBindings.Count(x => x.Id == inputAxisId) == 0)
            {
                throw new GlaivesException($"No axis binding exists in input settings with id '{inputAxisId}'");
            }

            if (InputAxisEvents.ContainsKey(inputAxisId))
            {
                throw new GlaivesException($"Failed to bind '{inputAxisId}', can not bind to the same input axis multiple times for the same actor");
            }
            InputAxisEvents.Add(inputAxisId, eventHandler);
        }

        /// <summary>
        /// Unbind a specific action (does nothing if the action is not bound)
        /// </summary>
        /// <param name="inputActionId"></param>
        public void UnbindAction(string inputActionId)
        {
            if (InputActionEvents.ContainsKey(inputActionId))
            {
                InputActionEvents.Remove(inputActionId);
            }
        }

        /// <summary>
        /// Unbind a specific axis (does nothing if the axis is not bound)
        /// </summary>
        /// <param name="inputAxisId"></param>
        public void UnbindAxis(string inputAxisId)
        {
            if (InputAxisEvents.ContainsKey(inputAxisId))
            {
                InputAxisEvents.Remove(inputAxisId);
            }
        }

        /// <summary>
        /// Unbind all actions
        /// </summary>
        public void UnbindAllActions()
        {
            InputActionEvents.Clear();
        }

        /// <summary>
        /// Unbind all axes
        /// </summary>
        public void UnbindAllAxes()
        {
            InputAxisEvents.Clear();
        }

        /// <summary>
        /// Unbind all actions and axes
        /// </summary>
        public void UnbindAll()
        {
            UnbindAllActions();
            UnbindAllAxes();
        }
    }
}
