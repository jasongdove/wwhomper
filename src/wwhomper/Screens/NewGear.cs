using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class NewGear : ScreenBase
    {
        private readonly CoordinateButton _no;

        public NewGear()
            : base("NewGear.png")
        {
            _no = new CoordinateButton(615, 254, 93, 24);
        }

        public CoordinateButton No
        {
            get { return _no; }
        }
    }
}