using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class Paused : TemplateScreen
    {
        private readonly Button _ok;

        public Paused(IAutoIt autoIt, IAssetCatalog assetCatalog)
            : base(autoIt, assetCatalog, @"Images\ALL\Dialog\Dialog_565x540.jpg", 15, 20, 580, 63)
        {
            _ok = CreateCoordinateButton(333, 494, 128, 38);
        }

        public Button Ok
        {
            get { return _ok; }
        }
    }
}