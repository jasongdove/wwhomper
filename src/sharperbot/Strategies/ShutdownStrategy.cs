using System;
using Ninject.Extensions.Logging;
using sharperbot.Screens;

namespace sharperbot.Strategies
{
    public class ShutdownStrategy : ScreenStrategy, IScreenStrategy<IGameScreen>
    {
        private readonly ILogger _logger;

        public ShutdownStrategy(ILogger logger)
        {
            _logger = logger;
        }

        public void ExecuteStrategy(IGameScreen screen)
        {
            _logger.Info("Shutting down!");
            Environment.Exit(0);
        }
    }
}