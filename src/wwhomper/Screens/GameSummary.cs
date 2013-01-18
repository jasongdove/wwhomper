using System.Drawing;
using wwhomper.Pak;

namespace wwhomper.Screens
{
    public class GameSummary : TemplateScreen
    {
        public GameSummary(PakCatalog pakCatalog)
            : base(pakCatalog, @"Images\ALL\Game\GameSummary\Dialog_GS_BG.jpg", new Rectangle(179, 360, 372, 71))
        {
        }
    }
}