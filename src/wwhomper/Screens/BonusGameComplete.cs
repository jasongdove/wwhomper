using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class BonusGameComplete : TextScreen, IDialogScreen
    {
        private readonly Button _ok;

        public BonusGameComplete(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                252, 198, 288, 37,
                "BONUS GAME")
        {
            _ok = CreateCoordinateButton(328, 360, 150, 38);
        }

        public Button Accept
        {
            get { return _ok; }
        }
    }
}