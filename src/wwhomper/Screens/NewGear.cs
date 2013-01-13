using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class NewGear : ScreenBase
    {
        private readonly TemplateButton _no;

        public NewGear()
            : base("NewGear.png")
        {
            _no = new TemplateButton("NoButton.png");
        }

        public TemplateButton No
        {
            get { return _no; }
        }
    }
}