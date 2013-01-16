using System.Drawing;
using wwhomper.Pak;
using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class Farm : ScreenBase
    {
        private readonly TemplateButton _gopherHole;

        public Farm(PakCatalog pakCatalog)
            : base(pakCatalog, @"Images\ALL\Game\Map\MapScreen_Hole_Idle.jpg", new Rectangle(94, 61, 33, 43))
        {
            _gopherHole = new TemplateButton(
                pakCatalog,
                @"Images\ALL\Game\Map\MapScreen_Gopher_Idle.jpg",
                new Rectangle(105, 54, 11, 45));
        }

        public TemplateButton GopherHole
        {
            get { return _gopherHole; }
        }
    }
}