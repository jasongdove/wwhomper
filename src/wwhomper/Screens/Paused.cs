using System.Drawing;
using wwhomper.Pak;
using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class Paused : TemplateScreen
    {
        private readonly CoordinateButton _ok;

        public Paused(PakCatalog pakCatalog)
            : base(pakCatalog, @"Images\ALL\Dialog\Dialog_565x540.jpg", new Rectangle(15, 20, 580, 63))
        {
            _ok = new CoordinateButton(341, 500, 125, 39);
        }

        public CoordinateButton Ok
        {
            get { return _ok; }
        }
    }
}