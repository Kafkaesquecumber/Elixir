using System.Collections.Generic;

namespace Elixir.GameFramework
{
    /// <summary>
    /// Ensures there is no other UniqueString with the same string value during the same runtime
    /// Adds a postfix to make the string unique to other UniqueStrings
    /// </summary>
    public sealed class UniqueString
    {
        private static readonly List<string> UsedStrings = new List<string>();

        /// <summary>
        /// The unique string
        /// </summary>
        public readonly string String;
        
        /// <summary>
        /// Potentially modifies the input str by adding a postfix to ensure it is unique to other UniqueStrings
        /// </summary>
        /// <param name="str"></param>
        public UniqueString(string str)
        {
            int postfix = 0;
            string store = str;
            do
            {
                str = store + "_" + postfix.ToString();
                postfix++;
            } while(UsedStrings.Contains(str));

            UsedStrings.Add(str);
            String = str;
        }

        // Prevent default constructor usage
        private UniqueString() { }

        // Prevent copy constructor usage
        private UniqueString(UniqueString copy) { }

        public override string ToString()
        {
            return String;
        }
    }
}
