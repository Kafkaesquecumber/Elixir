namespace Elixir.Diagnostics
{
    /// <summary>
    /// The type of log message
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// An info log is non-problematic message
        /// </summary>
        Info,
        /// <summary>
        /// A warning log is a message about something which is could cause issues
        /// </summary>
        Warning,
        /// <summary>
        /// An error log is a message about something that is a critical, application breaking issue
        /// </summary>
        Error
    }
}