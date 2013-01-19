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
                593, 451, 213, 178)
        {
            _accept = CreateCoordinateButton(329, 545, 152, 38);
        }

        public Button Accept
        {
            get { return _accept; }
        }
    }
}