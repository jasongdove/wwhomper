using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Ninject.Extensions.Logging;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Strategies;
using wwhomper.Data;
using wwhomper.Dictionary;
using wwhomper.Screens;

namespace wwhomper.Strategies
{
    public class PuzzleGameStrategy : ScreenStrategy, IScreenStrategy<InPuzzleGame>
    {
        private readonly IAutoIt _autoIt;
        private readonly ILogger _logger;
        private readonly IPakDictionary _pakDictionary;

        public PuzzleGameStrategy(IAutoIt autoIt, ILogger logger, IPakDictionary pakDictionary)
        {
            _autoIt = autoIt;
            _logger = logger;
            _pakDictionary = pakDictionary;
        }

        public void ExecuteStrategy(InPuzzleGame screen)
        {
            screen.ClearAllGears();

            _autoIt.MoveMouseOffscreen();

            // Identify all gear spots (size, color, index)
            var gearSpots = screen.GetGearSpots().OrderBy(x => x.Index);

            // Identify all tools (torch, paint)
            var tools = screen.GetTools();
            foreach (var tool in tools)
            {
                if (tool is PuzzleTorch)
                {
                    _logger.Debug("Found torch");
                }
                else
                {
                    var paint = tool as PuzzlePaint;
                    if (paint != null)
                    {
                        _logger.Debug("Found paint: {0}", paint.Color);
                    }
                }
            }

            // Identify all gears (letter, size, color, clickable area)
            var gears = screen.GetGears();
            foreach (var gear in gears)
            {
                _logger.Debug("Found gear {0}/{1}/{2}", gear.Size, gear.Color, gear.Letter);
            }

            if (gears.Count < gearSpots.Count())
            {
                _logger.Debug("Not enough gears to solve the puzzle yet");
                screen.Back.Click();
                return;
            }

            var potentialAnswers = _pakDictionary.OfLength(gearSpots.Count());
            var answerSteps = new List<PuzzleStep>(5);
            foreach (var answer in potentialAnswers)
            {
                // Reset available gears and tools
                var availableGears = new List<PuzzleGear>(gears);
                var availableTools = new List<PuzzleTool>(tools);
                
                foreach (var gearSpot in gearSpots)
                {
                    PuzzleStep answerStep = null;

                    var letter = answer[gearSpot.Index].ToString(CultureInfo.InvariantCulture);
                    
                    // Prefer actual letters over wildcards
                    var availableGear = availableGears.FirstOrDefault(x => x.Letter == letter && x.Size.HasFlag(gearSpot.Size) && x.Color.HasFlag(gearSpot.Color))
                        ?? availableGears.FirstOrDefault(x => x.IsWildcard);
                    if (availableGear != null)
                    {
                        availableGears.Remove(availableGear);
                        answerStep = new PuzzleStep(availableGear);
                    }

                    // Try to use the torch (changes gear size)
                    if (answerStep == null)
                    {
                        var torch = availableTools.FirstOrDefault(x => x is PuzzleTorch);
                        if (torch != null)
                        {
                            var wrongSizedGear = availableGears.FirstOrDefault(x => x.Letter == letter && !x.Size.HasFlag(gearSpot.Size) && x.Color.HasFlag(gearSpot.Color));
                            if (wrongSizedGear != null)
                            {
                                availableGears.Remove(wrongSizedGear);
                                availableTools.Remove(torch);
                                answerStep = new PuzzleStep(wrongSizedGear, torch);
                            }
                        }
                    }

                    // Try to use paint (changes gear color)
                    if (answerStep == null)
                    {
                        var paint = availableTools.FirstOrDefault(x => x is PuzzlePaint && ((PuzzlePaint)x).Color.HasFlag(gearSpot.Color));
                        if (paint != null)
                        {
                            var wrongColorGear = availableGears.FirstOrDefault(x => x.Letter == letter && x.Size.HasFlag(gearSpot.Size) && !x.Color.HasFlag(gearSpot.Color));
                            if (wrongColorGear != null)
                            {
                                availableGears.Remove(wrongColorGear);
                                availableTools.Remove(paint);
                                answerStep = new PuzzleStep(wrongColorGear, paint);
                            }
                        }
                    }

                    if (answerStep != null)
                    {
                        answerSteps.Add(answerStep);
                    }
                    else
                    {
                        break;
                    }
                }

                if (answerSteps.Count == answer.Length)
                {
                    _logger.Debug("Found answer: {0}", answer);
                    break;
                }

                answerSteps.Clear();
            }

            if (answerSteps.Any())
            {
                screen.SubmitAnswer(answerSteps);
                foreach (var step in answerSteps)
                {
                    var gear = step.Gear;
                    _logger.Debug("Using gear {0}/{1}/{2}{3}", gear.Size, gear.Color, gear.Letter, step.Tool != null ? "/tool" : String.Empty);
                }

                Wait(TimeSpan.FromSeconds(7));
            }
            else
            {
                _logger.Debug("Unable to create word with available gears");
                screen.Back.Click();
            }
        }
    }
}