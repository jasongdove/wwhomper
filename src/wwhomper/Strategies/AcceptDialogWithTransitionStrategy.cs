using System;
using sharperbot.Screens;
using sharperbot.Strategies;

namespace wwhomper.Strategies
{
    public class AcceptDialogWithTransitionStrategy : ScreenStrategy, IScreenStrategy<IDialogScreen>
    {
        public void ExecuteStrategy(IDialogScreen screen)
        {
            screen.Accept.Click();
            Wait(TimeSpan.FromSeconds(5));
        }
    }
}