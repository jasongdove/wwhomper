using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class GameSummary : ScreenBase
    {
        private readonly CoordinateButton _okeyDokey;

        public GameSummary()
            : base("GameSummary.png")
        {
            _okeyDokey = new CoordinateButton(481, 550, 91, 37);
        }

        public CoordinateButton OkeyDokey
        {
            get { return _okeyDokey; }
        }
    }
}