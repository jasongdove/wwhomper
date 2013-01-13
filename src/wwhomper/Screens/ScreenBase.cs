using Emgu.CV;
using Emgu.CV.Structure;

namespace wwhomper.Screens
{
    public abstract class ScreenBase
    {
        private readonly Image<Gray, byte> _icon;

        protected ScreenBase(string iconName)
        {
            _icon = TemplateLoader.LoadTemplate(iconName);
        }

        public Image<Gray, byte> Icon
        {
            get { return _icon; }
        }

        public TemplateSearchResult WaitUntilLoaded()
        {
            return AutoIt.WaitForTemplate(WordWhomper.WindowTitle, _icon);
        }
    }
}