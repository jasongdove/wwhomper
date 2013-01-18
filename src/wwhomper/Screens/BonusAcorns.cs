using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class BonusAcorns : TextScreen
    {
        private readonly CoordinateButton _ok;

        public BonusAcorns()
            : base(497, 192, 221, 21, "LET'S TAKE THEM TO THE BONUS")
        {
            _ok = new CoordinateButton(550, 249, 99, 23);

            AdditionalCharacters = "'";
            RequiresZoom = true;
        }

        public CoordinateButton Ok
        {
            get { return _ok; }
        }
    }
}