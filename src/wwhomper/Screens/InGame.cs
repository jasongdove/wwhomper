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
        private readonly List<TemplateCoordinate> _letters = new List<TemplateCoordinate>();

        public InGame()
            : base("InGame.bmp")
        {
            _letters.Add(new TemplateCoordinate(184, 425, 58, 34));
            _letters.Add(new TemplateCoordinate(261, 420, 58, 34));
            _letters.Add(new TemplateCoordinate(338, 425, 58, 34));
            _letters.Add(new TemplateCoordinate(415, 420, 58, 34));
            _letters.Add(new TemplateCoordinate(492, 425, 58, 34));
            _letters.Add(new TemplateCoordinate(569, 420, 58, 34));
        }

        public string GetLetters()
        {
            var windowContents = new Image<Gray, byte>(AutoIt.GetWindowContents(WordWhomper.WindowTitle));

            var images = new List<Image<Gray, byte>>();
            foreach (var letter in _letters)
            {
                images.Add(letter.Grab(windowContents));
            }

            var result = new Image<Gray, byte>(images.Sum(x => x.Width), images.Max(x => x.Height));
            int imagex = 0;
            foreach (var image in images)
            {
                image.CopyTo(result.GetSubRect(new Rectangle(imagex, 0, image.Width, image.Height)));
                imagex += image.Width;
            }

            var tesseract = new Tesseract("tessdata", "eng", Tesseract.OcrEngineMode.OEM_TESSERACT_ONLY);
            tesseract.Recognize(result);
            return tesseract.GetText();
        }
    }
}