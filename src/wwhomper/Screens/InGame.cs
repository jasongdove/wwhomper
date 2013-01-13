using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class InGame : ScreenBase
    {
        private readonly List<TemplateCoordinate> _gamePieces = new List<TemplateCoordinate>();

        public InGame()
            : base("InGame.bmp")
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
            var windowContents = new Image<Gray, byte>(AutoIt.GetWindowContents(WordWhomper.WindowTitle));

            // Get an image of each individual game piece
            var gamePieceImages = new List<Image<Gray, byte>>();
            foreach (var piece in _gamePieces)
            {
                gamePieceImages.Add(piece.Grab(windowContents));
            }

            // Create a new image to hold all game pieces side by side
            var result = new Image<Gray, byte>(gamePieceImages.Sum(x => x.Width), gamePieceImages.Max(x => x.Height));

            // Fill the new image with all game pieces
            int imagex = 0;
            foreach (var image in gamePieceImages)
            {
                image.CopyTo(result.GetSubRect(new Rectangle(imagex, 0, image.Width, image.Height)));
                imagex += image.Width;
            }

            // Convert the new image to text to find out which letters are available for play
            var tesseract = new Tesseract("tessdata", "eng", Tesseract.OcrEngineMode.OEM_TESSERACT_ONLY);
            tesseract.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            tesseract.Recognize(result);
            return FixTesseractErrors(tesseract.GetText());
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