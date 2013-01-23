using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class Paused : TemplateScreen, IDialogScreen
    {
        private readonly Button _ok;

        public Paused(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\ALL\Dialog\Dialog_565x540.jpg",
                15, 20, 580, 63,
                86, 3, 623, 122)
        {
            _ok = CreateCoordinateButton(333, 471, 124, 34);
        }

        public Button Accept
        {
            get { return _ok; }
        }
    }
}