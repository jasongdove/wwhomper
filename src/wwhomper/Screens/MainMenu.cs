using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class MainMenu : TemplateScreen, IDialogScreen
    {
        private readonly Button _play;

        public MainMenu(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\EN_US\Menu\MainMenu_Button_PogoLogo.jpg",
                0, 0, 72, 40,
                0, 418, 231, 97)
        {
            _play = CreateCoordinateButton(20, 323, 128, 38);
        }

        public Button Accept
        {
            get { return _play; }
        }
    }
}