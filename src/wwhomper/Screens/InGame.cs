using System;
using System.Collections.Generic;
using System.Linq;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class InGame : TemplateScreen
    {
        private readonly List<TemplateCoordinate> _gamePieces = new List<TemplateCoordinate>();

        public InGame(IAutoIt autoIt, IAssetCatalog assetCatalog)
            : base(autoIt, assetCatalog, @"Images\EN_US\Game\Buttons\Button_Enter_Idle.jpg", 28, 11, 126, 18)
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
            var combined = Combine(gamePieceImages);

            // Convert the new image to text to find out which letters are available for play
            return FixTesseractErrors(GetText(combined));
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