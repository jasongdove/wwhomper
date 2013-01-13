using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public abstract class ScreenBase
    {
        private readonly Image<Gray, byte> _template;
        private readonly List<PixelAnchor> _anchors;

        protected ScreenBase(string templateName)
        {
            _template = TemplateLoader.LoadTemplate(templateName);
            _anchors = new List<PixelAnchor>();
        }

        public List<PixelAnchor> Anchors
        {
            get { return _anchors; }
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