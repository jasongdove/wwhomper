using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class FinaleTwo : TemplateScreen, IDialogScreen
    {
        private readonly Button _accept;

        public FinaleTwo(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\EN_US\Dialog\Dialog_Finale\Story_02.jpg",
                496, 20, 261, 22,
                465, 8, 331, 92)
        {
            _accept = CreateTemplateButton(@"Images\ALL\Dialog\Dialog_Arrow_Right_Idle.jpg", 10, 14, 74, 35);
        }

        public Button Accept
        {
            get { return _accept; }
        }
    }
}