using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;

namespace wwhomper.Screens
{
    public class SpeechBubble : TemplateScreen
    {
        public SpeechBubble(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\ALL\Game\Common\Dialog_SpeechBubble_GS_Sm.jpg",
                18, 13, 242, 20,
                377, 79, 423, 303)
        {
        }
    }
}