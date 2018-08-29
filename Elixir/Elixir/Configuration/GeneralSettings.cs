
using System;

namespace Elixir.Configuration
{    
    [Serializable]
    public class GeneralSettings
    {
        /// <summary>
        /// <para>The desired frames per second</para>
        /// <para>This is an approximation, actual fps may vary</para>
        /// <para>0 means unlimited</para>
        /// </summary>
        public uint TargetFps { get; set; } = 200;
    }
}
