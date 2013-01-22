using System.Collections.Generic;
using System.Linq;
using Ninject;
using Ninject.Extensions.Logging;
using sharperbot.AutoIt;
using sharperbot.Screens;

namespace sharperbot.Strategies
{
    public class ScreenSearchStrategy : ScreenStrategy
    {
        private readonly IKernel _kernel;
        private readonly IAutoIt _autoIt;
        private readonly ILogger _logger;

        private readonly Dictionary<IGameScreen, IScreenStrategy> _screens;

        protected ScreenSearchStrategy(IKernel kernel, IAutoIt autoIt, ILogger logger)
        {
            _kernel = kernel;
            _autoIt = autoIt;
            _logger = logger;

            _screens = new Dictionary<IGameScreen, IScreenStrategy>();
        }

        protected void RegisterNestedScreen<TScreen, TStrategy>()
            where TScreen : IGameScreen
            where TStrategy : IScreenStrategy<TScreen>
        {
            _screens.Add(_kernel.Get<TScreen>(), _kernel.Get<TStrategy>());
        }

        protected void PerformSearch()
        {
            if (!_screens.Any())
            {
                _logger.Warn("No nested screens registered");
                return;
            }

            _logger.Debug("Searching for nested screens - count={0}", _screens.Count);
            var screenSearchResult = _autoIt.WaitForScreen(_screens.Keys.ToArray());
            if (screenSearchResult.Success)
            {
                var screen = screenSearchResult.Screen;

                var strategy = _screens[screen];
                var type = strategy.GetType();

                _logger.Debug(
                    "Detected nested screen - type={0}, strategyType={1}",
                    screen.GetType().Name,
                    type.Name);

                var methodInfo = type.GetMethod("ExecuteStrategy");
                methodInfo.Invoke(strategy, new object[] { screen });
            }
        }
    }
}