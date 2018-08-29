using System;
using System.Runtime.InteropServices;
using Elixir.GameFramework;

namespace Elixir.Internal.Interface
{
    internal abstract class TextureInterface : Interface
    {
        public abstract IntVector2 Size { get; }

        public abstract void Update(byte[] pixels, int width, int height, int offsetX, int offsetY);
        public abstract void Dispose();
        public abstract int[] GetPixelData();
        
        internal abstract void CreateTexture(int width, int height);
        internal abstract void CreateTexture(string file);

        /// <summary>
        /// Return the pointer to the pixel data, call UnlockTexture when you are done using it!
        /// </summary>
        /// <returns></returns>
        public IntPtr LockTexture()
        {
            _gcHandle = GCHandle.Alloc(GetPixelData(), GCHandleType.Pinned);
            IntPtr ptr = _gcHandle.AddrOfPinnedObject();
            return ptr;
        }

        /// <summary>
        /// Free the previously locked pointer
        /// </summary>
        public void UnlockTexture()
        {
            if (_gcHandle.IsAllocated)
            {
                _gcHandle.Free();
            }
        }

        private GCHandle _gcHandle;
    }
}
