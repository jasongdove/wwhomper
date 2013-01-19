using System;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Strategies;
using wwhomper.Screens;

namespace wwhomper.Strategies
{
    public class GameSummaryStrategy : ScreenStrategy, IScreenStrategy<GameSummary>
    {
        private readonly IAutoIt _autoIt;

        public GameSummaryStrategy(IAutoIt autoIt)
        {
            _autoIt = autoIt;
        }

        public void ExecuteStrategy(GameSummary screen)
        {
            // Speed up the summary display
            _autoIt.Type("{ENTER}");

            Wait(TimeSpan.FromMilliseconds(200));

            // Dismiss the summary
            _autoIt.Type("{ENTER}");
        }
    }
}