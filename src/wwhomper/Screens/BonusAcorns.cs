using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class BonusAcorns : ScreenBase
    {
        private readonly CoordinateButton _ok;

        public BonusAcorns()
            : base("BonusAcorns.png")
        {
            _ok = new CoordinateButton(560, 256, 90, 20);
        }

        public CoordinateButton Ok
        {
            get { return _ok; }
        }
    }
}