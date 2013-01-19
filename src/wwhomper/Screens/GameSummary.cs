using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;

namespace wwhomper.Screens
{
    public class GameSummary : TemplateScreen
    {
        public GameSummary(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\ALL\Game\GameSummary\Dialog_GS_BG.jpg",
                506, 97, 79, 32,
                527, 116, 93, 45)
        {
        }
    }
}