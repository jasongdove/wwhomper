using System;
using System.Threading;
using sharperbot.Screens;

namespace sharperbot.Strategies
{
    public abstract class ScreenStrategy : IScreenStrategy
    {
        protected void Wait(TimeSpan duration)
        {
            Thread.Sleep(duration);
        }
    }
}