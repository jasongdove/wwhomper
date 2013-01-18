using System.Drawing;
using wwhomper.Pak;
using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class MainMenu : TemplateScreen
    {
        private readonly CoordinateButton _play;

        public MainMenu(PakCatalog pakCatalog)
            : base(pakCatalog, @"Images\EN_US\Menu\MainMenu_Button_PogoLogo.jpg", new Rectangle(0, 0, 72, 40))
        {
            _play = new CoordinateButton(22, 327, 142, 90);
        }

        public CoordinateButton Play
        {
            get { return _play; }
        }
    }
}