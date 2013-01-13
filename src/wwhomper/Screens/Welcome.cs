using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class Welcome : ScreenBase
    {
        private readonly TemplateButton _ok;

        public Welcome()
            : base("Welcome.png")
        {
            _ok = new TemplateButton("OkButton.png");
        }

        public TemplateButton Ok
        {
            get { return _ok; }
        }
    }
}