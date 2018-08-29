using System;
using System.Runtime.Serialization;

namespace Elixir.Internal
{
    internal class ElixirException : Exception
    {
        internal ElixirException()
        {
        }

        internal ElixirException(string message) : base(message)
        {
        }

        internal ElixirException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected internal ElixirException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
