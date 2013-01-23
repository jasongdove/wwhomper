using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class IntroComplete : TemplateScreen, IDialogScreen
    {
        private readonly Button _accept;

        public IntroComplete(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\ALL\Dialog\Dialog_Loc\Dialog_Loc_BG.jpg",
                641, 464, 150, 127,
                590, 420, 210, 180)
        {
            _accept = CreateCoordinateButton(326, 519, 152, 37);
        }

        public Button Accept
        {
            get { return _accept; }
        }
    }
}