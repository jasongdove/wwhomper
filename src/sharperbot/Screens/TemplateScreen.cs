using System.Drawing;
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

        protected TemplateScreen(IAutoIt autoIt, IAssetCatalog assetCatalog, string fileName, int x, int y, int width, int height)
            : base(autoIt, assetCatalog)
        {
            _autoIt = autoIt;
            _template = assetCatalog.GetCompositeImage(fileName).GetSubRect(new Rectangle(x, y, width, height));
        }

        public Image<Bgra, byte> Template
        {
            get { return _template; }
        }

        public override ScreenSearchResult IsActive(Image<Bgra, byte> windowContents)
        {
            return _autoIt.IsTemplateInWindow(windowContents, _template);
        }
    }
}