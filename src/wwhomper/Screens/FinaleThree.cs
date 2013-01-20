using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class FinaleThree : TemplateScreen, IDialogScreen
    {
        private readonly Button _ok;

        public FinaleThree(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\EN_US\Dialog\Dialog_Finale\Story_03.jpg",
                406, 290, 89, 130,
                354, 263, 213, 228)
        {
            _ok = CreateCoordinateButton(636, 545, 150, 37);
        }

        public Button Accept
        {
            get { return _ok; }
        }
    }
}