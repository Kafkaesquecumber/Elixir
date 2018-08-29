using System;
using System.Runtime.Serialization;

namespace Elixir.Internal
{
    internal class SgeException : Exception
    {
        internal SgeException()
        {
        }

        internal SgeException(string message) : base(message)
        {
        }

        internal SgeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected internal SgeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
