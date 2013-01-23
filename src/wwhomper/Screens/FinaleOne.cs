using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class FinaleOne : TemplateScreen, IDialogScreen
    {
        private readonly Button _accept;

        public FinaleOne(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\EN_US\Dialog\Dialog_Finale\Story_01.jpg",
                52, 149, 182, 29,
                27, 127, 227, 73)
        {
            _accept = CreateTemplateButton(@"Images\ALL\Dialog\Dialog_Arrow_Right_Idle.jpg", 10, 14, 74, 35);
        }

        public Button Accept
        {
            get { return _accept; }
        }
    }
}