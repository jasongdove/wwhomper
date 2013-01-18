using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using wwhomper.Pak;

namespace wwhomper.Screens.Controls
{
    public class TemplateButton : ButtonBase
    {
        private readonly Image<Bgra, byte> _template;

        public TemplateButton(PakCatalog pakCatalog, string fileName, Rectangle rectangle)
        {
            _template = pakCatalog.GetCompositeImage(fileName)
                .GetSubRect(rectangle);
        }

        public override void Click()
        {
            var searchResult = AutoIt.WaitForTemplate(WordWhomper.WindowTitle, _template);
            if (searchResult.Success)
            {
                AutoIt.Click(new Rectangle(searchResult.Point, _template.Size));
            }
            else
            {
                throw new InvalidOperationException("Button never became active!");
            }
        }
    }
}