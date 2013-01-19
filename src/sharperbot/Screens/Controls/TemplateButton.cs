using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using sharperbot.Assets;
using sharperbot.AutoIt;

namespace sharperbot.Screens.Controls
{
    internal class TemplateButton : Button
    {
        private readonly IAutoIt _autoIt;
        private readonly Image<Bgra, byte> _template;

        public TemplateButton(IAutoIt autoIt, IAssetCatalog assetCatalog, string fileName, Rectangle rectangle)
        {
            _autoIt = autoIt;
            _template = assetCatalog.GetCompositeImage(fileName).Copy(rectangle);
        }

        public override void Click()
        {
            var searchResult = _autoIt.WaitForTemplate(_template);
            if (searchResult.Success)
            {
                _autoIt.Click(new Rectangle(searchResult.Point, _template.Size));
            }
            else
            {
                throw new InvalidOperationException("Button never became active!");
            }
        }
    }
}