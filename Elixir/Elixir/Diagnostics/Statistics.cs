
namespace Elixir.Diagnostics
{
    public class Statistics
    {
        /// <summary>
        /// The current frame rate 
        /// </summary>
        public int Fps => Engine.Get.EngineTimer?.FramesPerSecond ?? 0;

        internal Statistics()
        {

        }
    }
}
