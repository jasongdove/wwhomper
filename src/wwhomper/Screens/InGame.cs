using System;
using System.Collections.Generic;
using System.Linq;
using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class InGame : ScreenBase
    {
        private readonly List<TemplateCoordinate> _gamePieces = new List<TemplateCoordinate>();

        public InGame()
            : base("InGame.png")
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
            var windowContents = AutoIt.GetWindowImage(WordWhomper.WindowTitle);

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
                result = raw.Trim();
                if (result.Length == 8)
                {
                    result = result.Replace("IVI", "M");
                }
            }

            return result;
        }
    }
}