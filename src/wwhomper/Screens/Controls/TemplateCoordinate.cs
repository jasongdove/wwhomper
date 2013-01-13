using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace wwhomper.Screens.Controls
{
    public class TemplateCoordinate
    {
        private readonly Rectangle _rectangle;

        public TemplateCoordinate(int x, int y, int width, int height)
            : this(new Rectangle(x, y, width, height))
        {
        }

        public TemplateCoordinate(Rectangle rectangle)
        {
            _rectangle = rectangle;
        }

        public Image<Gray, byte> Grab()
        {
            var image = AutoIt.GetWindowContents(WordWhomper.WindowTitle, _rectangle);
            return new Image<Gray, byte>(image);
        }

        public Image<Gray, byte> Grab(Image<Gray, byte> windowContents)
        {
            return windowContents.GetSubRect(_rectangle);
        }
    }
}