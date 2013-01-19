using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class LocIntro : TemplateScreen
    {
        private readonly Button _forward;

        public LocIntro(IAutoIt autoIt, IAssetCatalog assetCatalog)
            : base(
                autoIt,
                assetCatalog,
                @"Images\ALL\Dialog\Dialog_Loc\Dialog_Loc_BG.jpg",
                274, 469, 198, 106,
                0, 276, 806, 353)
        {
            _forward = CreateTemplateButton(@"Images\ALL\Dialog\Dialog_Arrow_Right_Idle.jpg", 10, 14, 74, 35);
        }

        public Button Forward
        {
            get { return _forward; }
        }
    }
}