using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class NewGear : TextScreen
    {
        private readonly CoordinateButton _no;
        private readonly CoordinateButton _yes;

        public NewGear()
            : base(492, 166, 168, 25, "YOU FOUND A NEW GEAR!")
        {
            AdditionalCharacters = "!";
            RequiresZoom = true;

            _no = new CoordinateButton(615, 254, 93, 24);
            _yes = new CoordinateButton(492, 250, 96, 22);
        }

        public CoordinateButton No
        {
            get { return _no; }
        }

        public CoordinateButton Yes
        {
            get { return _yes; }
        }
    }
}