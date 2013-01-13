using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace wwhomper.Screens.Controls
{
    public class TemplateButton : ButtonBase
    {
        private readonly Image<Gray, byte> _template;

        public TemplateButton(string templateName)
        {
            _template = TemplateLoader.LoadTemplate(templateName);
        }

        public override void Click()
        {
            var searchResult = AutoIt.WaitForTemplate(WordWhomper.WindowTitle, _template);
            if (searchResult.Success)
            {
                Click(new Rectangle(searchResult.Point, _template.Size));
            }
            else
            {
                throw new InvalidOperationException("Button never became active!");
            }
        }
    }
}