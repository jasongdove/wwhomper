using System.Drawing;
using sharperbot.AutoIt;

namespace sharperbot.Screens.Controls
{
    internal class CoordinateButton : Button
    {
        private readonly IAutoIt _autoIt;
        private readonly Rectangle _rectangle;

        public CoordinateButton(IAutoIt autoIt, int x, int y, int width, int height)
            : this(new Rectangle(x, y, width, height))
        {
            _autoIt = autoIt;
        }

        public CoordinateButton(Rectangle rectangle)
        {
            _rectangle = rectangle;
        }

        public override void Click()
        {
            _autoIt.Click(_rectangle);
        }
    }
}