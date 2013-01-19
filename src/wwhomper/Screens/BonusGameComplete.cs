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
                255, 223, 288, 37,
                "BONUS GAME")
        {
            _ok = CreateCoordinateButton(329, 382, 154, 43);
        }

        public Button Accept
        {
            get { return _ok; }
        }
    }
}