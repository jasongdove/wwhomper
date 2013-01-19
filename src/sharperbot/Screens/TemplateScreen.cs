using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using Emgu.CV;
using Emgu.CV.Structure;
using sharperbot.Assets;
using sharperbot.AutoIt;

namespace sharperbot.Screens
{
    public abstract class TemplateScreen : GameScreen
    {
        private readonly IAutoIt _autoIt;
        private readonly Image<Bgra, byte> _template;
        private readonly Rectangle _activeRectangle;

        protected TemplateScreen(
            IAutoIt autoIt,
            IAssetCatalog assetCatalog,
            string fileName,
            int x, int y, int width, int height,
            int activeX, int activeY, int activeWidth, int activeHeight)
            : base(autoIt, assetCatalog)
        {
            _autoIt = autoIt;
            _template = assetCatalog.GetCompositeImage(fileName).GetSubRect(new Rectangle(x, y, width, height));
            _activeRectangle = new Rectangle(activeX, activeY, activeWidth, activeHeight);
        }

        public Image<Bgra, byte> Template
        {
            get { return _template; }
        }

        public override ScreenSearchResult IsActive(Image<Bgra, byte> windowContents)
        {
            var activeContents = windowContents.GetSubRect(_activeRectangle);
            var searchResult = _autoIt.IsTemplateInWindow(activeContents, _template);

#if DEBUG
            if (searchResult.Success)
            {
                var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "activeRectangles");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var fileName = Path.Combine(path, DateTime.Now.Ticks + ".png");

                var copy = windowContents.Copy();
                copy.Draw(_activeRectangle, new Bgra(255, 255, 255, 255), 1);
                copy.Draw(new Rectangle(searchResult.Point.X + _activeRectangle.X, searchResult.Point.Y + _activeRectangle.Y, _template.Size.Width, _template.Size.Height), new Bgra(0, 255, 0, 255), 1);
                copy.Save(fileName);

                if (searchResult.Point.X == 0 && searchResult.Point.Y == 0)
                {
                    var badPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "corruption");
                    if (!Directory.Exists(badPath))
                    {
                        Directory.CreateDirectory(badPath);
                    }

                    var badFileName = Path.Combine(badPath, DateTime.Now.Ticks + ".png");
                    _template.Save(badFileName);
                }
            }
#endif
            return searchResult;
        }
    }
}