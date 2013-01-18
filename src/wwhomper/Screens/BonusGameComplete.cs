using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class BonusGameComplete : TextScreen
    {
        private readonly CoordinateButton _ok;

        public BonusGameComplete()
            : base(255, 223, 288, 37, "BONUS GAME")
        {
            _ok = new CoordinateButton(329, 382, 154, 43);
        }

        public CoordinateButton Ok
        {
            get { return _ok; }
        }
    }
}