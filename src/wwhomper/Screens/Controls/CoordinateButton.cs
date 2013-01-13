using System.Drawing;

namespace wwhomper.Screens.Controls
{
    public class CoordinateButton : ButtonBase
    {
        private readonly Rectangle _rectangle;

        public CoordinateButton(int x, int y, int width, int height)
            : this(new Rectangle(x, y, width, height))
        {
        }

        public CoordinateButton(Rectangle rectangle)
        {
            _rectangle = rectangle;
        }

        public override void Click()
        {
            Click(_rectangle);
        }
    }
}