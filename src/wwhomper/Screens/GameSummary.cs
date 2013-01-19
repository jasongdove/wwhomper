using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;

namespace wwhomper.Screens
{
    public class GameSummary : TemplateScreen
    {
        public GameSummary(IAutoIt autoIt, IAssetCatalog assetCatalog)
            : base(autoIt, assetCatalog, @"Images\ALL\Game\GameSummary\Dialog_GS_BG.jpg", 179, 360, 372, 71)
        {
        }
    }
}