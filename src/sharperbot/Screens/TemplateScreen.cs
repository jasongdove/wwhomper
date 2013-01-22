using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;

namespace sharperbot.Screens
{
    public abstract class TemplateScreen : GameScreen
    {
        private readonly IAutoIt _autoIt;
        private readonly string _fileName;
        private readonly Rectangle _templateRectangle;
        private readonly Rectangle _activeRectangle;
        private readonly Image<Bgra, byte> _template;

        protected TemplateScreen(
            IAutoIt autoIt,
            IAssetCatalog assetCatalog,
            ILogger logger,
            string fileName,
            int x, int y, int width, int height,
            int activeX, int activeY, int activeWidth, int activeHeight)
            : base(autoIt, assetCatalog, logger)
        {
            _autoIt = autoIt;
            _fileName = fileName;
            _templateRectangle = new Rectangle(x, y, width, height);
            _activeRectangle = new Rectangle(activeX, activeY, activeWidth, activeHeight);

            _template = AssetCatalog.GetCompositeImage(_fileName).Copy(_templateRectangle);
        }

        public Image<Bgra, byte> Template
        {
            get { return _template; }
        }

        public override ScreenSearchResult IsActive(Image<Bgra, byte> windowContents)
        {
            var template = Template;

            windowContents.ROI = _activeRectangle;
            var searchResult = _autoIt.IsTemplateInWindow(windowContents, template);

#if DEBUG
            if (searchResult.Success)
            {
                ////var copy = windowContents.Copy();
                ////copy.Draw(_activeRectangle, new Bgra(255, 255, 255, 255), 1);
                ////copy.Draw(new Rectangle(searchResult.Point.X + _activeRectangle.X, searchResult.Point.Y + _activeRectangle.Y, template.Size.Width, template.Size.Height), new Bgra(0, 255, 0, 255), 1);
                ////SaveDebugImage(copy, "activeRectangles", DateTime.Now.Ticks + ".png");
            }
#endif
            return searchResult;
        }
    }
}