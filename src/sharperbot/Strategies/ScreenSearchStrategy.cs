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
                return;
            }

            var screenSearchResult = _autoIt.WaitForScreen(_screens.Keys.ToArray());
            if (screenSearchResult.Success)
            {
                var screen = screenSearchResult.Screen;

                _logger.Debug("Detected nested screen: {0}", screen.GetType().Name);

                var strategy = _screens[screen];
                var type = strategy.GetType();
                var methodInfo = type.GetMethod("ExecuteStrategy");
                methodInfo.Invoke(strategy, new object[] { screen });
            }
        }
    }
}