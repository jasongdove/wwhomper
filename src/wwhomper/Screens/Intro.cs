using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class Intro : TemplateScreen, IDialogScreen
    {
        private readonly Button _accept;

        public Intro(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\ALL\Dialog\Dialog_Loc\Dialog_Loc_BG.jpg",
                274, 469, 198, 106,
                0, 400, 800, 200)
        {
            _accept = CreateTemplateButton(@"Images\ALL\Dialog\Dialog_Arrow_Right_Idle.jpg", 10, 14, 74, 35);
        }

        public Button Accept
        {
            get { return _accept; }
        }
    }
}