
using System.Linq;
using Elixir.Internal;

namespace Elixir.Input
{
    /// <summary>
    /// Binding an id to an array of keys
    /// </summary>
    public struct InputActionBinding
    {
        /// <summary>
        /// The identifier for this input binding
        /// </summary>
        public string Id { get; set; }

        private Key[] _keys;

        /// <summary>
        /// The keys associated with this input binding
        /// </summary>
        public Key[] Keys
        {
            get => _keys;
            set
            {
                if (value.Any(key => key == Key.Unknown))
                {
                    throw new ElixirException($"Can not create an {nameof(InputActionBinding)} using {nameof(Key)}.{nameof(Key.Unknown)}");
                }

                _keys = value;
            }
        }
    }
}
