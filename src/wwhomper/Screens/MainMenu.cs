using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class MainMenu : ScreenBase
    {
        private readonly CoordinateButton _play;

        public MainMenu()
            : base("MainMenu.bmp")
        {
            _play = new CoordinateButton(22, 327, 142, 90);
        }

        public CoordinateButton Play
        {
            get { return _play; }
        }
    }
}