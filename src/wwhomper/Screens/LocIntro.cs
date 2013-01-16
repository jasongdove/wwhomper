using System.Drawing;
using wwhomper.Pak;
using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class LocIntro : ScreenBase
    {
        private readonly TemplateButton _forward;

        public LocIntro(PakCatalog pakCatalog)
            : base(pakCatalog, @"Images\ALL\Dialog\Dialog_Loc\Dialog_Loc_BG.jpg", new Rectangle(274, 469, 198, 106))
        {
            _forward = new TemplateButton(pakCatalog, @"Images\ALL\Dialog\Dialog_Arrow_Right_Idle.jpg", new Rectangle(10, 14, 74, 35));
        }

        public TemplateButton Forward
        {
            get { return _forward; }
        }
    }
}