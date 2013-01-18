using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using wwhomper.Font;
using wwhomper.Pak;

namespace wwhomper.Screens
{
    public abstract class ScreenBase
    {
        private readonly Image<Bgra, byte> _template;

        protected ScreenBase(BitmapFont font, string text)
        {
            _template = font.GetImage(text);
        }

        protected ScreenBase(PakCatalog pakCatalog, string fileName, Rectangle rectangle)
        {
            _template = pakCatalog.GetCompositeImage(fileName)
                                  .GetSubRect(rectangle);
        }

        protected ScreenBase(string templateName)
        {
            _template = TemplateLoader.LoadTemplate(templateName);
        }

        public Image<Bgra, byte> Template
        {
            get { return _template; }
        }

        public TemplateSearchResult WaitUntilLoaded()
        {
            return AutoIt.WaitForTemplate(WordWhomper.WindowTitle, _template);
        }

        protected Image<Bgra, byte> Combine(IEnumerable<Image<Bgra, byte>> images)
        {
            var list = images.ToList();

            // Create a new image to hold all images side by side
            var result = new Image<Bgra, byte>(
                list.Sum(i => i.Width),
                list.Max(i => i.Height));

            // Fill the new image with all images
            int x = 0;
            foreach (var image in list)
            {
                var targetRect = new Rectangle(x, 0, image.Width, image.Height);
                image.CopyTo(result.GetSubRect(targetRect));
                x += image.Width;
            }

            return result;
        }

        protected string GetText(Image<Bgra, byte> image)
        {
            var grayscale = image.Convert<Gray, byte>();

            var tesseract = new Tesseract("tessdata", "eng", Tesseract.OcrEngineMode.OEM_TESSERACT_ONLY);
            tesseract.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            tesseract.Recognize(grayscale);
            var result = tesseract.GetText();
            Console.WriteLine("Tesseract recognized: {0}", result.Trim());

            ////var ticks = DateTime.Now.Ticks;
            ////image.Save(String.Format(@"tesseract\{0}.png", ticks));
            ////using (var file = File.CreateText(String.Format(@"tesseract\{0}.txt", ticks)))
            ////{
            ////    file.WriteLine(result);
            ////}

            return tesseract.GetText();
        }
    }
}