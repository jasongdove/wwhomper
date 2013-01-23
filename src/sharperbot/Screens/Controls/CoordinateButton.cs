using System.Drawing;
using Ninject.Extensions.Logging;
using sharperbot.AutoIt;

namespace sharperbot.Screens.Controls
{
    internal class CoordinateButton : Button
    {
        private readonly IAutoIt _autoIt;
        private readonly ILogger _logger;
        private readonly Rectangle _rectangle;

        public CoordinateButton(IAutoIt autoIt, ILogger logger, int x, int y, int width, int height)
            : this(new Rectangle(x, y, width, height))
        {
            _autoIt = autoIt;
            _logger = logger;
        }

        public CoordinateButton(Rectangle rectangle)
        {
            _rectangle = rectangle;
        }

        public override void Click()
        {
            ////_logger.Debug(
            ////    "Clicking coordinate button - x={0}, y={1}, width={2}, height={3}",
            ////    _rectangle.X,
            ////    _rectangle.Y,
            ////    _rectangle.Width,
            ////    _rectangle.Height);

            _autoIt.Click(_rectangle);
        }
    }
}