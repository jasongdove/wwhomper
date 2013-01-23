using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class BonusAcorns : TextScreen, IDialogScreen
    {
        private readonly Button _ok;

        public BonusAcorns(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                493, 160, 211, 22,
                "LET'S TAKE THEM TO THE BONUS")
        {
            AdditionalCharacters = "'";
            RequiresZoom = true;

            _ok = CreateCoordinateButton(548, 223, 96, 22);
        }

        public Button Accept
        {
            get { return _ok; }
        }
    }
}