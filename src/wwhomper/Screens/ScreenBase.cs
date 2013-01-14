using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Emgu.CV;
using Emgu.CV.OCR;
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

        protected Image<Gray, byte> Combine(IEnumerable<Image<Gray, byte>> images)
        {
            var list = images.ToList();

            // Create a new image to hold all images side by side
            var result = new Image<Gray, byte>(list.Sum(x => x.Width), list.Max(x => x.Height));

            // Fill the new image with all images
            int imagex = 0;
            foreach (var image in list)
            {
                image.CopyTo(result.GetSubRect(new Rectangle(imagex, 0, image.Width, image.Height)));
                imagex += image.Width;
            }

            return result;
        }

        protected string GetText(Image<Gray, byte> image)
        {
            var tesseract = new Tesseract("tessdata", "eng", Tesseract.OcrEngineMode.OEM_TESSERACT_ONLY);
            tesseract.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            tesseract.Recognize(image);
            var result = tesseract.GetText();
            Console.WriteLine("Tesseract recognized: {0}", result);
            
            var ticks = DateTime.Now.Ticks;
            image.Save(String.Format(@"tesseract\{0}.png", ticks));
            using (var file = File.CreateText(String.Format(@"tesseract\{0}.txt", ticks)))
            {
                file.WriteLine(result);
            }

            return tesseract.GetText();
        }
    }
}