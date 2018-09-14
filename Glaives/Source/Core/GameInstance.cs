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

using Glaives.Core.Configuration;
using Glaives.Core.Diagnostics;
using Glaives.Core.Internal;

namespace Glaives.Core
{
    /// <summary>
    /// A game instance is a persistent class that will exist until the program closes
    /// </summary>
    public abstract class GameInstance
    {
        /// <summary>
        /// A shorthand to the engine singleton
        /// </summary>
        protected Engine Engine => Engine.Get;

        protected GameInstance()
        {
            if (!Engine.Get.AllowGameInstanceInstantiation)
            {
                throw new GlaivesException("A game instance should not be created manually, the engine will create it when you call Engine.Initialize");
            }
        }

        internal void InitializeInternal() { Initialize(); }

        internal Settings GetSettingsInternal() { return InitializeSettings(); }

        internal void LogInternal(LogType logType, string message, string className, string methodName, int lineNumber)
        {
            ReceiveLogs(logType, message, className, methodName, lineNumber);
        }

        internal void ImGuiInternal()
        {
            OnImGui();
        }

        /// <summary>
        /// <para>The engine will obtain it's settings from this call when it initializes</para>
        /// <para>Called once before Initialize</para>
        /// </summary>  
        /// <returns>The settings you want to use for the application</returns>
        protected abstract Settings InitializeSettings();

        /// <summary>
        /// Called after the engine modules have been initialized
        /// </summary>
        protected virtual void Initialize() { }

        /// <summary>
        /// ImGui rendering 
        /// </summary>
        protected virtual void OnImGui() { }

        /// <summary>
        /// Receive engine log messages
        /// </summary>
        /// <param name="logType">The type of log message</param>
        /// <param name="message">The log message</param>
        /// <param name="className">The class which invoked this log</param>
        /// <param name="methodName">The method in the class which invoked this log</param>
        /// <param name="lineNumber">The number of the line in the method that invoked this log</param>
        protected abstract void ReceiveLogs(LogType logType, string message, string className, string methodName, int lineNumber);
    }
}
