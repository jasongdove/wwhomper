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
                478, 164, 189, 22,
                "YOU FOUND A BLOW TORCH!")
        {
            AdditionalCharacters = "!";
            RequiresZoom = true;

            _ok = CreateCoordinateButton(491, 250, 96, 23);
        }

        public Button Accept
        {
            get { return _ok; }
        }
    }
}