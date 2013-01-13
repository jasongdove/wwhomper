using Emgu.CV;
using Emgu.CV.Structure;

namespace wwhomper.Screens
{
    public abstract class ScreenBase
    {
        private readonly Image<Gray, byte> _template;

        protected ScreenBase(string templateName)
        {
            _template = TemplateLoader.LoadTemplate(templateName);
        }

        public Image<Gray, byte> Template
        {
            get { return _template; }
        }

        public TemplateSearchResult WaitUntilLoaded()
        {
            return AutoIt.WaitForTemplate(WordWhomper.WindowTitle, _template);
        }
    }
}