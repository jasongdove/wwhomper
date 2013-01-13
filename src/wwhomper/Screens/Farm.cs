using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class Farm : ScreenBase
    {
        private readonly TemplateButton _gopherHole;

        public Farm()
            : base("Farm.bmp")
        {
            _gopherHole = new TemplateButton("GopherHole.bmp");
        }

        public TemplateButton GopherHole
        {
            get { return _gopherHole; }
        }
    }
}