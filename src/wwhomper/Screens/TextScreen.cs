using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace wwhomper.Screens
{
    public abstract class TextScreen : ScreenBase
    {
        private readonly Rectangle _rectangle;
        private readonly string _text;

        protected TextScreen(int x, int y, int width, int height, string text)
            : this(new Rectangle(x, y, width, height), text)
        {
            AdditionalCharacters = String.Empty;
        }

        protected TextScreen(Rectangle rectangle, string text)
        {
            _rectangle = rectangle;
            _text = text;
        }

        protected string AdditionalCharacters { get; set; }

        public override ScreenSearchResult IsActive(Image<Bgra, byte> windowContents)
        {
            var image = windowContents.GetSubRect(_rectangle);
            var text = GetZoomedOutText(image, AdditionalCharacters).Trim();
            
            var result = new ScreenSearchResult { Success = text == _text };
            if (result.Success)
            {
                result.Screen = this;
            }

            return result;
        }
    }
}