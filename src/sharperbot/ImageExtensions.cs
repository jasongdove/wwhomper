using Emgu.CV;
using Emgu.CV.Structure;

namespace sharperbot
{
    public static class ImageExtensions
    {
        public static void Floor(this Image<Gray, byte> image, int floor)
        {
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    if (image[y, x].Intensity < floor)
                    {
                        image[y, x] = new Gray();
                    }
                }
            }
        }
    }
}