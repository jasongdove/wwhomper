using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;

namespace wwhomper.Screens
{
    public class BonusGameWaiting : TemplateScreen
    {
        public BonusGameWaiting(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\ALL\Game\bonus_game\BG_Background.jpg",
                98, 386, 96, 23,
                93, 407, 111, 32)
        {
        }
    }
}