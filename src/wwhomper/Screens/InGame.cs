using System;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV.Structure;
using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class InGame : TemplateScreen
    {
        private readonly List<TemplateCoordinate> _gamePieces = new List<TemplateCoordinate>();

        public InGame(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\ALL\Game\gophers\burrow\Gopher_BUR_IDLE.jpg",
                15, 40, 12, 24,
                140, 581, 31, 40)
        {
            _gamePieces.Add(new TemplateCoordinate(184, 425, 58, 34));
            _gamePieces.Add(new TemplateCoordinate(261, 420, 58, 34));
            _gamePieces.Add(new TemplateCoordinate(338, 425, 58, 34));
            _gamePieces.Add(new TemplateCoordinate(415, 420, 58, 34));
            _gamePieces.Add(new TemplateCoordinate(492, 425, 58, 34));
            _gamePieces.Add(new TemplateCoordinate(569, 420, 58, 34));
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
                result = raw.Trim().Replace(" ", String.Empty);
                if (result.Length == 8)
                {
                    result = result.Replace("IVI", "M");
                }
            }

            return result;
        }
    }
}