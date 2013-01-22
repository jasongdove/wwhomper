using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ninject;
using Ninject.Extensions.Logging;
using sharperbot.AutoIt;
using sharperbot.Screens;

namespace sharperbot
{
    public abstract class Bot
    {
        private readonly IKernel _kernel;
        private readonly ILogger _logger;
        private readonly IAutoIt _autoIt;

        private readonly Dictionary<IGameScreen, IScreenStrategy> _screens;

        protected Bot(IKernel kernel, ILogger logger, IAutoIt autoIt)
        {
            _kernel = kernel;
            _logger = logger;
            _autoIt = autoIt;

            _screens = new Dictionary<IGameScreen, IScreenStrategy>();
        }

        public void Run()
        {
            do
            {
                _logger.Debug("Searching for screens - count={0}", _screens.Count);

                var screenSearchResult = _autoIt.WaitForScreen(_screens.Keys.ToArray());
                if (screenSearchResult.Success)
                {
                    var screen = screenSearchResult.Screen;

                    var strategy = _screens[screen];
                    var type = strategy.GetType();

                    _logger.Debug(
                        "Detected screen - type={0}, strategyType={1}",
                        screen.GetType().Name,
                        type.Name);

                    var methodInfo = type.GetMethod("ExecuteStrategy");
                    methodInfo.Invoke(strategy, new object[] { screen });
                }
                else
                {
                    if (!_autoIt.IsWindowActive())
                    {
                        Thread.Sleep(3000);
                    }
                }
            } while (true);
        }

        protected void RegisterScreen<TScreen, TStrategy>()
            where TScreen : IGameScreen
            where TStrategy : IScreenStrategy<TScreen>
        {
            _screens.Add(_kernel.Get<TScreen>(), _kernel.Get<TStrategy>());
        }
    }
}