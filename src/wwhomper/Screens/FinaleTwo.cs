using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class FinaleTwo : TemplateScreen, IDialogScreen
    {
        private readonly Button _forward;

        public FinaleTwo(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\EN_US\Dialog\Dialog_Finale\Story_02.jpg",
                496, 20, 261, 22,
                468, 33, 331, 92)
        {
            _forward = CreateCoordinateButton(678, 549, 60, 30);
        }

        public Button Accept
        {
            get { return _forward; }
        }
    }
}