using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens.Controls;

namespace sharperbot.Screens
{
    public abstract class GameScreen
    {
        private readonly IAutoIt _autoIt;
        private readonly IAssetCatalog _assetCatalog;

        protected GameScreen(IAutoIt autoIt, IAssetCatalog assetCatalog)
        {
            _autoIt = autoIt;
            _assetCatalog = assetCatalog;
        }

        protected IAutoIt AutoIt
        {
            get { return _autoIt; }
        }

        public abstract ScreenSearchResult IsActive(Image<Bgra, byte> windowContents);

        protected Button CreateCoordinateButton(int x, int y, int width, int height)
        {
            return new CoordinateButton(_autoIt, x, y, width, height);
        }

        protected Button CreateTemplateButton(string assetFileName, int x, int y, int width, int height)
        {
            return new TemplateButton(_autoIt, _assetCatalog, assetFileName, new Rectangle(x, y, width, height));
        }

        protected Image<Bgra, byte> Combine(IEnumerable<Image<Bgra, byte>> images)
        {
            var list = images.ToList();

            // Create a new image to hold all images side by side
            var result = new Image<Bgra, byte>(
                list.Sum(i => i.Width),
                list.Max(i => i.Height));

            // Fill the new image with all images
            int x = 0;
            foreach (var image in list)
            {
                var targetRect = new Rectangle(x, 0, image.Width, image.Height);
                image.CopyTo(result.GetSubRect(targetRect));
                x += image.Width;
            }

            return result;
        }

        protected string GetZoomedOutText<T>(Image<T, byte> image, double scale, string additionalLetters = "", bool debug = false) where T : struct, IColor
        {
            var zoomedOut = image.Resize(scale, Emgu.CV.CvEnum.INTER.CV_INTER_LANCZOS4);
            return GetText(zoomedOut, additionalLetters, debug);
        }

        protected string GetText<T>(Image<T, byte> image, string additionalLetters = "", bool debug = false) where T : struct, IColor
        {
            var grayscale = image.Convert<Gray, byte>();

            var tesseract = new Tesseract("tessdata", "eng", Tesseract.OcrEngineMode.OEM_TESSERACT_ONLY);
            tesseract.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ" + additionalLetters);
            tesseract.Recognize(grayscale);
            var result = tesseract.GetText();
            Console.WriteLine("Tesseract recognized: {0}", result.Trim());

            if (debug)
            {
                var ticks = DateTime.Now.Ticks;
                grayscale.Save(String.Format(@"tesseract\{0}.png", ticks));
                using (var file = File.CreateText(String.Format(@"tesseract\{0}.txt", ticks)))
                {
                    file.WriteLine(result);
                }
            }

            return tesseract.GetText();
        }
    }
}