using System;
using System.Drawing;

namespace wwhomper.Screens.Controls
{
    public abstract class ButtonBase
    {
        private readonly Random _random;

        protected ButtonBase()
        {
            _random = new Random();
        }

        public abstract void Click();

        protected void Click(Rectangle rectangle)
        {
            var x = _random.Next(rectangle.Left, rectangle.Right);
            var y = _random.Next(rectangle.Top, rectangle.Bottom);
            var speed = _random.Next(2, 10);

            AutoIt.Click(WordWhomper.WindowTitle, x, y, speed);
        }
    }
}