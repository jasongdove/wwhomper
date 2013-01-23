using System;
using System.Drawing;
using Ninject.Extensions.Logging;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Strategies;
using wwhomper.Data;
using wwhomper.Screens;

namespace wwhomper.Strategies
{
    public class MapStrategy : ScreenStrategy, IScreenStrategy<Map>
    {
        private readonly IAutoIt _autoIt;
        private readonly ILogger _logger;
        private readonly PuzzleGameState _puzzleGameState;

        public MapStrategy(IAutoIt autoIt, ILogger logger, PuzzleGameState puzzleGameState)
        {
            _autoIt = autoIt;
            _logger = logger;
            _puzzleGameState = puzzleGameState;
        }

        public void ExecuteStrategy(Map screen)
        {
            // If we need a gear type, let's make sure we're in the right zone
            var gearWeNeed = _puzzleGameState.GearWeNeed;
            if (gearWeNeed != null)
            {
                int currentZone = -1;
                var inWrongZone = false;
                var windowContents = _autoIt.GetWindowImage();
                var searchResult = _autoIt.IsTemplateInWindow(windowContents, screen.CurrentLocation);
                if (searchResult.Success)
                {
                    for (int i = 0; i < screen.Zones.Count; i++)
                    {
                        windowContents.ROI = screen.Zones[i];
                        var zoneResult = _autoIt.IsTemplateInWindow(windowContents, screen.CurrentLocation);
                        if (zoneResult.Success)
                        {
                            currentZone = i;
                            inWrongZone = gearWeNeed.Index != i;
                            break;
                        }
                    }
                }

                windowContents.ROI = Rectangle.Empty;

                if (inWrongZone)
                {
                    _logger.Debug(
                        "Wrong zone for target gear - zone={0}, targetZone={1}",
                        currentZone,
                        gearWeNeed.Index);

                    // Start by moving somewhere in the first zone. This avoids
                    // issues caused by holes that are too close in later zones
                    foreach (int index in new[] { 0, gearWeNeed.Index })
                    {
                        var targetZone = screen.Zones[index];
                        var searchArea = windowContents.Copy(targetZone);
                        var openHoleSearchResult = _autoIt.IsTemplateInWindow(searchArea, screen.OpenHole);
                        if (openHoleSearchResult.Success)
                        {
                            // Convert to coordinates back to screen coordinates
                            var target = new Rectangle(
                                targetZone.X + openHoleSearchResult.Point.X,
                                targetZone.Y + openHoleSearchResult.Point.Y,
                                screen.OpenHole.Width,
                                screen.OpenHole.Height);

                            // Click the open hole
                            _autoIt.Click(target);

                            Wait(TimeSpan.FromSeconds(1));
                        }
                    }

                    // We can cycle back to determine if we're in the correct zone
                    return;
                }
            }

            _autoIt.MoveMouseOffscreen();

            screen.Accept.Click();
        }
    }
}