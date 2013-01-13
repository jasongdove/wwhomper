using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class IntroOne : ScreenBase
    {
        private readonly TemplateButton _forward;

        public IntroOne()
            : base("IntroOne.png")
        {
            _forward = new TemplateButton("ForwardButton.png");
        }

        public TemplateButton Forward
        {
            get { return _forward; }
        }
    }
}