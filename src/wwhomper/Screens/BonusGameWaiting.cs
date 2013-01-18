using System.Drawing;
using wwhomper.Pak;

namespace wwhomper.Screens
{
    public class BonusGameWaiting : TemplateScreen
    {
        public BonusGameWaiting(PakCatalog pakCatalog)
            : base(pakCatalog, @"Images\ALL\Game\bonus_game\BG_Background.jpg", new Rectangle(98, 386, 96, 23))
        {
        }
    }
}