using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class NewGear : TextScreen, IDialogScreen
    {
        private readonly Button _yes;

        public NewGear(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                489, 141, 168, 25,
                "YOU FOUND A NEW GEAR!")
        {
            AdditionalCharacters = "!";
            RequiresZoom = true;

            _yes = CreateCoordinateButton(488, 224, 97, 21);
        }

        public Button Accept
        {
            get { return _yes; }
        }
    }
}