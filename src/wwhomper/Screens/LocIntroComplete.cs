using System.Drawing;
using wwhomper.Pak;
using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class LocIntroComplete : ScreenBase
    {
        private readonly CoordinateButton _ok;

        public LocIntroComplete(PakCatalog pakCatalog)
            : base(pakCatalog, @"Images\ALL\Dialog\Dialog_Loc\Dialog_Loc_BG.jpg", new Rectangle(641, 464, 150, 127))
        {
            _ok = new CoordinateButton(319, 522, 159, 38);
        }

        public CoordinateButton Ok
        {
            get { return _ok; }
        }
    }
}