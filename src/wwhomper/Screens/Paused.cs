using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class Paused : ScreenBase
    {
        private readonly CoordinateButton _ok;

        public Paused()
            : base("Paused.png")
        {
            _ok = new CoordinateButton(341, 500, 125, 39);
        }

        public CoordinateButton Ok
        {
            get { return _ok; }
        }
    }
}