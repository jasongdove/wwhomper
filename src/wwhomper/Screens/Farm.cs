using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class Farm : ScreenBase
    {
        private readonly IconButton _gopherHole;

        public Farm()
            : base("Farm.bmp")
        {
            _gopherHole = new IconButton("GopherHole.bmp");
        }

        public IconButton GopherHole
        {
            get { return _gopherHole; }
        }
    }
}