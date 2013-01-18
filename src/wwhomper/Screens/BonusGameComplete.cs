using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class BonusGameComplete : TemplateScreen
    {
        private readonly CoordinateButton _ok;

        public BonusGameComplete()
            : base("BonusGameComplete.png")
        {
            _ok = new CoordinateButton(336, 390, 150, 39);
        }

        public CoordinateButton Ok
        {
            get { return _ok; }
        }
    }
}