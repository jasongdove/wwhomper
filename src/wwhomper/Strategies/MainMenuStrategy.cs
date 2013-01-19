using System;
using sharperbot.Screens;
using sharperbot.Strategies;
using wwhomper.Screens;

namespace wwhomper.Strategies
{
    public class MainMenuStrategy : ScreenStrategy, IScreenStrategy<MainMenu>
    {
        public void ExecuteStrategy(MainMenu screen)
        {
            screen.Play.Click();
            Wait(TimeSpan.FromSeconds(4));
        }
    }
}