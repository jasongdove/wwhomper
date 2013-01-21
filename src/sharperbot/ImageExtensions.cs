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

        public static Image<Bgra, byte> CombineAlpha(this Image<Bgra, byte> image, Image<Bgra, byte> alpha)
        {
            var intensity = alpha.Convert<Gray, byte>();

            for (int y = 0; y < image.Data.GetLength(0); y++)
            {
                for (int x = 0; x < image.Data.GetLength(1); x++)
                {
                    var existing = image[y, x];
                    image[y, x] = new Bgra(
                        existing.Blue,
                        existing.Green,
                        existing.Red,
                        intensity[y, x].Intensity);
                }
            }

            return image;
        }
    }
}