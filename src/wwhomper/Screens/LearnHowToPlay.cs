using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class LearnHowToPlay : ScreenBase
    {
        private readonly CoordinateButton _no;

        public LearnHowToPlay()
            : base("LearnHowToPlay.bmp")
        {
            _no = new CoordinateButton(621, 253, 95, 25);
        }

        public CoordinateButton No
        {
            get { return _no; }
        }
    }
}