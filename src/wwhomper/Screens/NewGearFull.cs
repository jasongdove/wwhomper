using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class NewGearFull : TextScreen, IDialogScreen
    {
        private readonly Button _ok;

        public NewGearFull(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                347, 161, 81, 23,
                "NOW FULL!")
        {
            AdditionalCharacters = "!";
            RequiresZoom = true;

            _ok = CreateCoordinateButton(418, 250, 99, 24);
        }

        public Button Accept
        {
            get { return _ok; }
        }
    }
}