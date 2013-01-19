using System;
using sharperbot.Screens;
using sharperbot.Strategies;
using wwhomper.Screens;

namespace wwhomper.Strategies
{
    public class BonusGameWaitingStrategy : ScreenStrategy, IScreenStrategy<BonusGameWaiting>
    {
        public void ExecuteStrategy(BonusGameWaiting screen)
        {
            Wait(TimeSpan.FromSeconds(2));
        }
    }
}