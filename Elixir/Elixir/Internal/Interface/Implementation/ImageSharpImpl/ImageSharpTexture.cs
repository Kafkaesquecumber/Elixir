using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Elixir.GameFramework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace Elixir.Internal.Interface.Implementation.ImageSharpImpl
{
    internal class ImageSharpTexture : TextureInterface
    {
        internal Image<Rgba32> NativeImage;

        public override IntVector2 Size => new IntVector2(NativeImage.Width, NativeImage.Height);

        public override void Update(byte[] pixels, int width, int height, int offsetX, int offsetY)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            NativeImage.Dispose();
        }

        public override int[] GetPixelData()
        {
            int xL = NativeImage.Width;
            int yL = NativeImage.Height;
            int[] data = new int[NativeImage.Width * NativeImage.Height];
            for (int x = 0; x < xL; x++)
            {
                for (int y = 0; y < yL; y++)
                {
                    Rgba32 color = NativeImage[x, y];
                    data[y * xL + x] = (color.A << 24) | (color.B << 16) | (color.G << 8) | (color.R << 0);
                }
            }
        
            return data;
        }

        internal override void CreateTexture(int width, int height)
        {
            byte[] bytes = Enumerable.Repeat<byte>(255, width * height * 4).ToArray();
            NativeImage = Image.LoadPixelData<Rgba32>(bytes, width, height);
        }

        internal override void CreateTexture(string file)
        {
            string path = $"{Directory.GetCurrentDirectory()}/{file}";
            NativeImage = Image.Load<Rgba32>(path);
            

        }
    }
}
