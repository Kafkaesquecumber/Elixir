
using Elixir.Configuration;
using Elixir.Diagnostics;
using Elixir.Internal;

namespace Elixir.GameFramework
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
                throw new SgeException("A game instance should not be created manually, the engine will create it when you call Engine.Initialize");
            }
        }

        internal void InitializeInternal() { Initialize(); }

        internal Settings GetSettingsInternal() { return InitializeSettings(); }

        internal void LogInternal(LogType logType, string message, string className, string methodName, int lineNumber)
        {
            ReceiveLogs(logType, message, className, methodName, lineNumber);
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
