using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class BlowTorch : TextScreen, IDialogScreen
    {
        private readonly Button _ok;

        public BlowTorch(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                475, 139, 189, 22,
                "YOU FOUND A BLOW TORCH!")
        {
            AdditionalCharacters = "!";
            RequiresZoom = true;

            _ok = CreateCoordinateButton(488, 225, 96, 23);
        }

        public Button Accept
        {
            get { return _ok; }
        }
    }
}