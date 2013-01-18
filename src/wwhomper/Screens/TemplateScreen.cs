using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using wwhomper.Font;
using wwhomper.Pak;

namespace wwhomper.Screens
{
    public abstract class TemplateScreen : ScreenBase
    {
        private readonly Image<Bgra, byte> _template;

        protected TemplateScreen(BitmapFont font, string text)
        {
            _template = font.GetImage(text);
        }

        protected TemplateScreen(PakCatalog pakCatalog, string fileName, Rectangle rectangle)
        {
            _template = pakCatalog.GetCompositeImage(fileName)
                                  .GetSubRect(rectangle);
        }

        protected TemplateScreen(string templateName)
        {
            _template = TemplateLoader.LoadTemplate(templateName);
        }

        public override ScreenSearchResult IsActive(Image<Bgra, byte> windowContents)
        {
            return AutoIt.IsTemplateInWindow(windowContents, _template);
        }
    }
}