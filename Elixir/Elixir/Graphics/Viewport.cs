using Elixir.GameFramework;

namespace Elixir.Graphics
{
    public class Viewport
    {
        public IntVector2 Size
        {
            get
            {
                Engine.Get.Window.Interface.GetFrameBufferSize(out int w, out int h);
                return new IntVector2(w, h);
            }
        }

        internal Viewport()
        {
            
        }
    }
}