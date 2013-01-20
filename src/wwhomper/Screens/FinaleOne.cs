using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class FinaleOne : TemplateScreen, IDialogScreen
    {
        private readonly Button _forward;

        public FinaleOne(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\EN_US\Dialog\Dialog_Finale\Story_01.jpg",
                52, 149, 182, 29,
                30, 152, 227, 73)
        {
            _forward = CreateCoordinateButton(678, 549, 60, 30);
        }

        public Button Accept
        {
            get { return _forward; }
        }
    }
}