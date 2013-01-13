using Emgu.CV;
using Emgu.CV.Structure;

namespace wwhomper.Screens
{
    public abstract class ScreenBase
    {
        private readonly Image<Gray, byte> _icon;

        protected ScreenBase(string iconName)
        {
            _icon = IconLoader.LoadIcon(iconName);
        }

        public Image<Gray, byte> Icon
        {
            get { return _icon; }
        }

        public IconSearchResult WaitUntilLoaded()
        {
            return AutoIt.WaitUntilActive(WordWhomper.WindowTitle, _icon);
        }
    }
}