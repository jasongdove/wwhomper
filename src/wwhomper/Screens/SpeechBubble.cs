using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;

namespace wwhomper.Screens
{
    public class SpeechBubble : TemplateScreen
    {
        public SpeechBubble(IAutoIt autoIt, IAssetCatalog assetCatalog)
            : base(autoIt, assetCatalog, @"Images\ALL\Game\Common\Dialog_SpeechBubble_GS_Sm.jpg", 18, 13, 242, 20)
        {
        }
    }
}