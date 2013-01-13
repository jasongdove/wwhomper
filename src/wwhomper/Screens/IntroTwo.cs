using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class IntroTwo : ScreenBase
    {
        private readonly TemplateButton _forward;

        public IntroTwo()
            : base("IntroTwo.png")
        {
            _forward = new TemplateButton("ForwardButton.png");
        }

        public TemplateButton Forward
        {
            get { return _forward; }
        }
    }
}