using System.Drawing;
using wwhomper.Pak;

namespace wwhomper.Screens
{
    public class SpeechBubble : ScreenBase
    {
        public SpeechBubble(PakCatalog pakCatalog)
            : base(pakCatalog, @"Images\ALL\Game\Common\Dialog_SpeechBubble_GS_Sm.jpg", new Rectangle(18, 13, 242, 20))
        {
        }
    }
}