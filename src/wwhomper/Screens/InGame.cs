using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;
using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class InGame : TemplateScreen
    {
        private readonly List<TemplateCoordinate> _gamePieces = new List<TemplateCoordinate>();

        private readonly Button _menuButton;
        private readonly Button _mapButton;
        private readonly Button _exitConfirmButton;

        private readonly List<Image<Bgra, byte>> _backgrounds;

        private readonly Rectangle _backgroundSearchArea;

        public InGame(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\ALL\Game\gophers\burrow\Gopher_BUR_IDLE.jpg",
                15, 40, 12, 24,
                133, 517, 45, 79)
        {
            _gamePieces.Add(new TemplateCoordinate(182, 399, 53, 35));
            _gamePieces.Add(new TemplateCoordinate(259, 394, 53, 35));
            _gamePieces.Add(new TemplateCoordinate(336, 399, 53, 35));
            _gamePieces.Add(new TemplateCoordinate(413, 394, 53, 35));
            _gamePieces.Add(new TemplateCoordinate(490, 399, 53, 35));
            _gamePieces.Add(new TemplateCoordinate(567, 394, 53, 35));

            _menuButton = CreateCoordinateButton(12, 537, 74, 18);
            _mapButton = CreateCoordinateButton(12, 472, 73, 14);
            _exitConfirmButton = CreateCoordinateButton(208, 343, 149, 34);

            var backgroundRectangle = new Rectangle(351, 274, 69, 26);
            _backgrounds = new List<Image<Bgra, byte>>
            {
                assetCatalog.GetCompositeImage(@"Images\ALL\Game\backgrounds\BG_01.jpg")
                    .Copy(backgroundRectangle),
                assetCatalog.GetCompositeImage(@"Images\ALL\Game\backgrounds\BG_02.jpg")
                    .Copy(backgroundRectangle),
                assetCatalog.GetCompositeImage(@"Images\ALL\Game\backgrounds\BG_03.jpg")
                    .Copy(backgroundRectangle),
                assetCatalog.GetCompositeImage(@"Images\ALL\Game\backgrounds\BG_04.jpg")
                    .Copy(backgroundRectangle),
                assetCatalog.GetCompositeImage(@"Images\ALL\Game\backgrounds\BG_05.jpg")
                    .Copy(backgroundRectangle),
                assetCatalog.GetCompositeImage(@"Images\ALL\Game\backgrounds\BG_06.jpg")
                    .Copy(backgroundRectangle),
            };

            _backgroundSearchArea = new Rectangle(338, 264, 104, 68);
        }

        public Button Menu
        {
            get { return _menuButton; }
        }

        public Button Map
        {
            get { return _mapButton; }
        }

        public Button ExitConfirm
        {
            get { return _exitConfirmButton; }
        }

        public List<Image<Bgra, byte>> Backgrounds
        {
            get { return _backgrounds; }
        }

        public Rectangle BackgroundSearchArea
        {
            get { return _backgroundSearchArea; }
        }

        public string GetLetters()
        {
            // Get the entire window contents
            var windowContents = AutoIt.GetWindowImage();

            // Get an image of each individual game piece
            var gamePieceImages = _gamePieces.Select(piece => piece.Grab(windowContents));

            // Combine all of the game pieces into a single image
            var combined = Combine(gamePieceImages).Convert<Gray, byte>().ThresholdBinary(new Gray(60), new Gray(255));

            // Convert the new image to text to find out which letters are available for play
            return FixTesseractErrors(GetZoomedOutText(combined, 3));
        }

        // TODO: Train tesseract rather than fudging fixes, though it may be hard to train with an all-caps font
        private string FixTesseractErrors(string raw)
        {
            string result = String.Empty;
            
            if (!String.IsNullOrEmpty(raw))
            {
                result = raw.Trim()
                    .Replace("\r\n", String.Empty)
                    .Replace("\n", String.Empty)
                    .Replace(" ", String.Empty);

                if (result.Length == 8 || result.Length == 10)
                {
                    result = result.Replace("IVI", "M");
                }

                Logger.Debug(
                    "Fixed tesseract errors - in={0}, out={1}",
                    raw.Replace("\r\n", @"\r\n").Replace("\n", @"\n"),
                    result);
            }

            return result;
        }
    }
}