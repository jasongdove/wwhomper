using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;

namespace sharperbot.Screens.Controls
{
    internal class TemplateButton : Button
    {
        private readonly IAutoIt _autoIt;
        private readonly ILogger _logger;
        private readonly Image<Bgra, byte> _template;

        public TemplateButton(IAutoIt autoIt, ILogger logger, IAssetCatalog assetCatalog, string fileName, Rectangle rectangle)
        {
            _autoIt = autoIt;
            _logger = logger;
            _template = assetCatalog.GetCompositeImage(fileName).Copy(rectangle);
        }

        public override void Click()
        {
            var searchResult = _autoIt.WaitForTemplate(_template);
            if (searchResult.Success)
            {
                var rect = new Rectangle(searchResult.Point, _template.Size);

                _logger.Debug(
                    "Clicking template button - x={0}, y={1}, width={2}, height={3}",
                    rect.X,
                    rect.Y,
                    rect.Width,
                    rect.Height);

                _autoIt.Click(rect);
            }
            else
            {
                throw new InvalidOperationException("Button never became active!");
            }
        }
    }
}