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
        private readonly PuzzleGameState _puzzleGameState;

        public PuzzleGameStrategy(IAutoIt autoIt, ILogger logger, IPakDictionary pakDictionary, PuzzleGameState puzzleGameState)
        {
            _autoIt = autoIt;
            _logger = logger;
            _pakDictionary = pakDictionary;
            _puzzleGameState = puzzleGameState;
        }

        public void ExecuteStrategy(InPuzzleGame screen)
        {
            screen.ClearAllGears();

            _autoIt.MoveMouseOffscreen();

            // Identify all gear spots (size, color, index)
            var gearSpots = screen.GetGearSpots();

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

            _puzzleGameState.Gears.Clear();
            _puzzleGameState.Gears.AddRange(gears);
            _puzzleGameState.GearSpots.Clear();
            _puzzleGameState.GearSpots.AddRange(gearSpots);

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

                bool performedOptimizations = false;
                if (tools.Any())
                {
                    var optimizations = OptimizeTools(tools, gearSpots, gears);
                    foreach (var optimization in optimizations)
                    {
                        performedOptimizations = true;
                        screen.ApplyTool(optimization.Tool, optimization.Gear);
                    }
                }

                // If we haven't changed anything, and our gear collection is full, delete one
                bool trashedGear = false;
                if (!performedOptimizations && gears.Count == 15)
                {
                    // TODO: Maybe base this on color/size rather than a simple overall frequency
                    trashedGear = true;

                    var availableGears = gears.AsEnumerable();

                    // Make sure we don't throw away a gear we need
                    var gearWeNeed = _puzzleGameState.GearWeNeed;
                    if (gearWeNeed != null)
                    {
                        availableGears = availableGears.Where(x => !x.Color.HasFlag(gearWeNeed.Color) && !x.Size.HasFlag(gearWeNeed.Size));
                    }

                    var letters = availableGears.Select(x => x.Letter[0]).ToArray();
                    var targetLetter = _pakDictionary.WorstLetterOverall(letters);
                    var gear = gears.First(x => x.Letter[0] == targetLetter);
                    screen.Trash(gear);
                }

                // Refresh the gears if we changed anything
                if (performedOptimizations || trashedGear)
                {
                    gears = screen.GetGears();
                    _puzzleGameState.Gears.Clear();
                    _puzzleGameState.Gears.AddRange(gears);
                }

                screen.Back.Click();
            }
        }

        private List<PuzzleStep> OptimizeTools(List<PuzzleTool> tools, List<PuzzleGearSpot> gearSpots, List<PuzzleGear> gears)
        {
            var availableGears = gears.Where(x => !x.IsWildcard).ToList();
            var optimizations = new List<PuzzleStep>();

            // If we have no letters of the right size, use a torch on our most common letter
            var torch = tools.FirstOrDefault(x => x is PuzzleTorch);
            if (torch != null)
            {
                _logger.Debug("Attempting to optimize torch usage");

                if (gearSpots.Count(x => x.Size.HasFlag(PuzzleGearSize.Large)) >
                    availableGears.Count(x => x.Size.HasFlag(PuzzleGearSize.Large)))
                {
                    ////_logger.Debug("We have large gear spots, but no large gears");

                    var targetSpot = gearSpots.First(x => x.Size.HasFlag(PuzzleGearSize.Large));
                    var targetGears = availableGears.Where(x => !x.Size.HasFlag(PuzzleGearSize.Large) && x.Color.HasFlag(targetSpot.Color)).ToList();
                    if (targetGears.Any())
                    {
                        ////_logger.Debug("We have gears with the correct color");

                        var targetGearLetter = _pakDictionary.BestLetterForIndex(targetGears.Select(x => x.Letter[0]).ToArray(), targetSpot.Index);
                        var targetGear = targetGears.First(x => x.Letter[0] == targetGearLetter);
                        if (targetGear != null)
                        {
                            _logger.Debug("Changing size of gear with letter {0} and color {1}", targetGear.Letter, targetGear.Color);

                            availableGears.Remove(targetGear);
                            optimizations.Add(new PuzzleStep(targetGear, torch));
                        }
                    }
                }

                if (gearSpots.Count(x => x.Size.HasFlag(PuzzleGearSize.Small)) >
                    availableGears.Count(x => x.Size.HasFlag(PuzzleGearSize.Small)))
                {
                    ////_logger.Debug("We have small gear spots, but no small gears");

                    var targetSpot = gearSpots.First(x => x.Size.HasFlag(PuzzleGearSize.Small));
                    var targetGears = availableGears.Where(x => !x.Size.HasFlag(PuzzleGearSize.Small) && x.Color.HasFlag(targetSpot.Color)).ToList();
                    if (targetGears.Any())
                    {
                        ////_logger.Debug("We have gears with the correct color");

                        var targetGearLetter = _pakDictionary.BestLetterForIndex(targetGears.Select(x => x.Letter[0]).ToArray(), targetSpot.Index);
                        var targetGear = targetGears.First(x => x.Letter[0] == targetGearLetter);
                        if (targetGear != null)
                        {
                            _logger.Debug("Changing size of gear with letter {0} and color {1}", targetGear.Letter, targetGear.Color);

                            availableGears.Remove(targetGear);
                            optimizations.Add(new PuzzleStep(targetGear, torch));
                        }
                    }
                }

                // If we haven't used the torch yet, let's loosen our color requirements
                if (optimizations.All(x => !(x.Tool is PuzzleTorch)))
                {
                    if (gearSpots.Count(x => x.Size.HasFlag(PuzzleGearSize.Large)) >
                        availableGears.Count(x => x.Size.HasFlag(PuzzleGearSize.Large)))
                    {
                        ////_logger.Debug("We have large gear spots, but no large gears");

                        var targetSpot = gearSpots.First(x => x.Size.HasFlag(PuzzleGearSize.Large));
                        var targetGears = availableGears.Where(x => !x.Size.HasFlag(PuzzleGearSize.Large)).ToList();
                        if (targetGears.Any())
                        {
                            var targetGearLetter = _pakDictionary.BestLetterForIndex(targetGears.Select(x => x.Letter[0]).ToArray(), targetSpot.Index);
                            var targetGear = targetGears.First(x => x.Letter[0] == targetGearLetter);
                            if (targetGear != null)
                            {
                                _logger.Debug("Changing size of gear with letter {0} and color {1}", targetGear.Letter, targetGear.Color);

                                availableGears.Remove(targetGear);
                                optimizations.Add(new PuzzleStep(targetGear, torch));
                            }
                        }
                    }

                    if (gearSpots.Count(x => x.Size.HasFlag(PuzzleGearSize.Small)) >
                        availableGears.Count(x => x.Size.HasFlag(PuzzleGearSize.Small)))
                    {
                        ////_logger.Debug("We have small gear spots, but no small gears");

                        var targetSpot = gearSpots.First(x => x.Size.HasFlag(PuzzleGearSize.Small));
                        var targetGears = availableGears.Where(x => !x.Size.HasFlag(PuzzleGearSize.Small)).ToList();
                        if (targetGears.Any())
                        {
                            var targetGearLetter = _pakDictionary.BestLetterForIndex(targetGears.Select(x => x.Letter[0]).ToArray(), targetSpot.Index);
                            var targetGear = targetGears.First(x => x.Letter[0] == targetGearLetter);
                            if (targetGear != null)
                            {
                                _logger.Debug("Changing size of gear with letter {0} and color {1}", targetGear.Letter, targetGear.Color);

                                availableGears.Remove(targetGear);
                                optimizations.Add(new PuzzleStep(targetGear, torch));
                            }
                        }
                    }
                }
            }

            var copperPaint = tools.FirstOrDefault(x => x is PuzzlePaint && ((PuzzlePaint)x).Color.HasFlag(PuzzleGearColor.Copper));
            if (copperPaint != null)
            {
                _logger.Debug("Attempting to optimize copper paint usage");

                if (gearSpots.Count(x => x.Color.HasFlag(PuzzleGearColor.Copper)) >
                    availableGears.Count(x => x.Color.HasFlag(PuzzleGearColor.Copper)))
                {
                    ////_logger.Debug("We have copper gear spots, but no copper gears");

                    var targetSpot = gearSpots.First(x => x.Color.HasFlag(PuzzleGearColor.Copper));
                    var targetGears = availableGears.Where(x => !x.Color.HasFlag(PuzzleGearColor.Copper) && x.Size.HasFlag(targetSpot.Size)).ToList();
                    
                    // If we don't have any the size we want, let's paint one anyway
                    if (!targetGears.Any())
                    {
                        targetGears = availableGears.Where(x => !x.Color.HasFlag(PuzzleGearColor.Copper)).ToList();
                    }
                    
                    if (targetGears.Any())
                    {
                        ////_logger.Debug("We have gears with the correct size");

                        var targetGearLetter = _pakDictionary.BestLetterForIndex(targetGears.Select(x => x.Letter[0]).ToArray(), targetSpot.Index);
                        var targetGear = targetGears.First(x => x.Letter[0] == targetGearLetter);
                        if (targetGear != null)
                        {
                            _logger.Debug("Changing color of gear with letter {0} and size {1}", targetGear.Letter, targetGear.Size);

                            availableGears.Remove(targetGear);
                            optimizations.Add(new PuzzleStep(targetGear, copperPaint));
                        }
                    }
                }
            }

            var silverPaint = tools.FirstOrDefault(x => x is PuzzlePaint && ((PuzzlePaint)x).Color.HasFlag(PuzzleGearColor.Silver));
            if (silverPaint != null)
            {
                _logger.Debug("Attempting to optimize silver paint usage");

                if (gearSpots.Count(x => x.Color.HasFlag(PuzzleGearColor.Silver)) >
                    availableGears.Count(x => x.Color.HasFlag(PuzzleGearColor.Silver)))
                {
                    ////_logger.Debug("We have silver gear spots, but no silver gears");

                    var targetSpot = gearSpots.First(x => x.Color.HasFlag(PuzzleGearColor.Silver));
                    var targetGears = availableGears.Where(x => !x.Color.HasFlag(PuzzleGearColor.Silver) && x.Size.HasFlag(targetSpot.Size)).ToList();

                    // If we don't have any the size we want, let's paint one anyway
                    if (!targetGears.Any())
                    {
                        targetGears = availableGears.Where(x => !x.Color.HasFlag(PuzzleGearColor.Silver)).ToList();
                    }

                    if (targetGears.Any())
                    {
                        ////_logger.Debug("We have gears with the correct size");

                        var targetGearLetter = _pakDictionary.BestLetterForIndex(targetGears.Select(x => x.Letter[0]).ToArray(), targetSpot.Index);
                        var targetGear = targetGears.First(x => x.Letter[0] == targetGearLetter);
                        if (targetGear != null)
                        {
                            _logger.Debug("Changing color of gear with letter {0} and size {1}", targetGear.Letter, targetGear.Size);

                            availableGears.Remove(targetGear);
                            optimizations.Add(new PuzzleStep(targetGear, silverPaint));
                        }
                    }
                }
            }

            var goldPaint = tools.FirstOrDefault(x => x is PuzzlePaint && ((PuzzlePaint)x).Color.HasFlag(PuzzleGearColor.Gold));
            if (goldPaint != null)
            {
                _logger.Debug("Attempting to optimize gold paint usage");

                if (gearSpots.Count(x => x.Color.HasFlag(PuzzleGearColor.Gold)) >
                    availableGears.Count(x => x.Color.HasFlag(PuzzleGearColor.Gold)))
                {
                    ////_logger.Debug("We have gold gear spots, but no gold gears");

                    var targetSpot = gearSpots.First(x => x.Color.HasFlag(PuzzleGearColor.Gold));
                    var targetGears = availableGears.Where(x => !x.Color.HasFlag(PuzzleGearColor.Gold) && x.Size.HasFlag(targetSpot.Size)).ToList();

                    // If we don't have any the size we want, let's paint one anyway
                    if (!targetGears.Any())
                    {
                        targetGears = availableGears.Where(x => !x.Color.HasFlag(PuzzleGearColor.Gold)).ToList();
                    }

                    if (targetGears.Any())
                    {
                        ////_logger.Debug("We have gears with the correct size");

                        var targetGearLetter = _pakDictionary.BestLetterForIndex(targetGears.Select(x => x.Letter[0]).ToArray(), targetSpot.Index);
                        var targetGear = targetGears.First(x => x.Letter[0] == targetGearLetter);
                        if (targetGear != null)
                        {
                            _logger.Debug("Changing color of gear with letter {0} and size {1}", targetGear.Letter, targetGear.Size);

                            availableGears.Remove(targetGear);
                            optimizations.Add(new PuzzleStep(targetGear, goldPaint));
                        }
                    }
                }
            }

            // TODO: Loosen size requirements on paint

            return optimizations;
        }
    }
}