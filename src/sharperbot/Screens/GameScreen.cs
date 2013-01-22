using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens.Controls;

namespace sharperbot.Screens
{
    public abstract class GameScreen : IGameScreen
    {
        private readonly IAutoIt _autoIt;
        private readonly IAssetCatalog _assetCatalog;
        private readonly ILogger _logger;

        protected GameScreen(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
        {
            _autoIt = autoIt;
            _assetCatalog = assetCatalog;
            _logger = logger;
        }

        protected IAutoIt AutoIt
        {
            get { return _autoIt; }
        }

        protected IAssetCatalog AssetCatalog
        {
            get { return _assetCatalog; }
        }

        protected ILogger Logger
        {
            get { return _logger; }
        }

        public abstract ScreenSearchResult IsActive(Image<Bgra, byte> windowContents);

        protected void SaveDebugImage<T>(Image<T, byte> image, string folder, string fileName) where T : struct, IColor
        {
            _logger.Debug(
                "Saving debug image - folder={0}, filename={1}",
                folder,
                fileName);

            var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (root != null)
            {
                var absoluteFolder = Path.Combine(root, folder);
                if (!Directory.Exists(absoluteFolder))
                {
                    Directory.CreateDirectory(absoluteFolder);
                }

                var absoluteFileName = Path.Combine(absoluteFolder, fileName);
                image.Save(absoluteFileName);
            }
        }

        protected Button CreateCoordinateButton(int x, int y, int width, int height)
        {
            return new CoordinateButton(_autoIt, _logger, x, y, width, height);
        }

        protected Button CreateTemplateButton(string assetFileName, int x, int y, int width, int height)
        {
            return new TemplateButton(_autoIt, _logger, _assetCatalog, assetFileName, new Rectangle(x, y, width, height));
        }

        protected Image<T, byte> Combine<T>(IEnumerable<Image<T, byte>> images) where T : struct, IColor
        {
            var list = images.ToList();

            // Create a new image to hold all images side by side
            var result = new Image<T, byte>(
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

        protected string GetZoomedOutTextThreshold(Image<Gray, byte> image, double scale, int threshold, int max, string additionalLetters = "", bool debug = false)
        {
            var zoomedOut = image.Resize(scale, Emgu.CV.CvEnum.INTER.CV_INTER_LANCZOS4);
            var blackWhite = zoomedOut.ThresholdBinary(new Gray(threshold), new Gray(max));
            return GetText(blackWhite, additionalLetters, debug);
        }

        protected string GetZoomedOutText<T>(Image<T, byte> image, double scale, string additionalLetters = "", bool debug = false) where T : struct, IColor
        {
            var zoomedOut = image.Resize(scale, Emgu.CV.CvEnum.INTER.CV_INTER_LANCZOS4);
            return GetText(zoomedOut, additionalLetters, debug);
        }

        protected string GetText<T>(Image<T, byte> image, string additionalLetters = "", bool debug = false) where T : struct, IColor
        {
            var grayscale = image.Convert<Gray, byte>();

            var tesseract = new Tesseract(
                "tessdata",
                "eng",
                Tesseract.OcrEngineMode.OEM_TESSERACT_ONLY);

            tesseract.SetVariable(
                "tessedit_char_whitelist",
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" + additionalLetters);

            tesseract.Recognize(grayscale);
            var result = tesseract.GetText();

            if (!String.IsNullOrWhiteSpace(result))
            {
                if (!String.IsNullOrEmpty(additionalLetters))
                {
                    _logger.Debug(
                        "Tesseract recognized text - result={0}, additionalLetters={1}",
                        result.Trim(),
                        additionalLetters);
                }
                else
                {
                    _logger.Debug(
                        "Tesseract recognized text - result={0}",
                        result.Trim());
                }
            }

            if (debug)
            {
                var ticks = DateTime.Now.Ticks;
                grayscale.Save(String.Format(@"tesseract\{0}.png", ticks));
                using (var file = File.CreateText(String.Format(@"tesseract\{0}.txt", ticks)))
                {
                    file.WriteLine(result);
                }
            }

            return result;
        }
    }
}