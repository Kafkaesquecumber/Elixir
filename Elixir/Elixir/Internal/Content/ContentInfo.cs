using System;

namespace Elixir.Internal.Content
{
    internal struct ContentInfo : IEquatable<ContentInfo>
    {
        internal readonly string File;
        internal readonly object CreateOptions;

        public ContentInfo(string file, object createOptions)
        {
            File = file;
            CreateOptions = createOptions;
        }

        public bool Equals(ContentInfo other)
        {
            return string.Equals(File, other.File) && Equals(CreateOptions, other.CreateOptions);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ContentInfo && Equals((ContentInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((File != null ? File.GetHashCode() : 0) * 397) ^ (CreateOptions != null ? CreateOptions.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ContentInfo left, ContentInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ContentInfo left, ContentInfo right)
        {
            return !left.Equals(right);
        }
    }
}