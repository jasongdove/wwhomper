using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;

namespace wwhomper.Screens
{
    public class BonusGameWaiting : TemplateScreen
    {
        public BonusGameWaiting(IAutoIt autoIt, IAssetCatalog assetCatalog)
            : base(autoIt, assetCatalog, @"Images\ALL\Game\bonus_game\BG_Background.jpg", 98, 386, 96, 23)
        {
        }
    }
}