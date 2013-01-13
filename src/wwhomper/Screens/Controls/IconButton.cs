using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace wwhomper.Screens.Controls
{
    public class IconButton : ButtonBase
    {
        private readonly Image<Gray, byte> _icon;

        public IconButton(string iconName)
        {
            _icon = IconLoader.LoadIcon(iconName);
        }

        public override void Click()
        {
            var searchResult = AutoIt.WaitUntilActive(WordWhomper.WindowTitle, _icon);
            if (searchResult.Success)
            {
                Click(new Rectangle(searchResult.Point, _icon.Size));
            }
            else
            {
                throw new InvalidOperationException("Button never became active!");
            }
        }
    }
}