using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class IntroThree : ScreenBase
    {
        private readonly TemplateButton _ok;

        public IntroThree()
            : base("IntroThree.png")
        {
            _ok = new TemplateButton("OkButton.png");
        }

        public TemplateButton Ok
        {
            get { return _ok; }
        }
    }
}