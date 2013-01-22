using System;
using System.Collections.Generic;
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
        private readonly TrashGearStrategy _trashGearStrategy;

        public PuzzleGameStrategy(
            IAutoIt autoIt,
            ILogger logger,
            IPakDictionary pakDictionary,
            PuzzleGameState puzzleGameState,
            TrashGearStrategy trashGearStrategy)
        {
            _autoIt = autoIt;
            _logger = logger;
            _pakDictionary = pakDictionary;
            _puzzleGameState = puzzleGameState;
            _trashGearStrategy = trashGearStrategy;
        }

        public void ExecuteStrategy(InPuzzleGame screen)
        {
            screen.ClearAllGears();

            _autoIt.MoveMouseOffscreen();

            // Identify all gear spots (size, color, index)
            var gearSpots = screen.GetGearSpots();

            // Identify all tools (torch, paint)
            var tools = screen.GetTools();

            // Identify all gears (letter, size, color, clickable area)
            var gears = screen.GetGears();

            if (gears.Count < gearSpots.Count())
            {
                _logger.Debug(
                    "Insufficient gears - gearSpots={0}, gears={1}",
                    gearSpots.Count,
                    gears.Count);

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

                    var letter = answer[gearSpot.Index];
                    
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

                // If we found an answer, let's get out
                if (answerSteps.Count == answer.Length)
                {
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

                // No idea what we need until we get the next puzzle
                _puzzleGameState.GearSpots.Clear();
                _puzzleGameState.Gears.Clear();

                Wait(TimeSpan.FromSeconds(7));
            }
            else
            {
                _logger.Debug(
                    "Unable to create word - gearSpots={0}, gears={1}, tools={2}",
                    String.Join(" ", gearSpots.Select(x => x.ToString())),
                    String.Join(" ", gears.Select(x => x.ToString())),
                    String.Join(" ", tools.Select(x => x.ToString())));

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
                    trashedGear = true;

                    var gear = _trashGearStrategy.FindGearToTrash(gearSpots, gears);
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
                if (gearSpots.Count(x => x.Size.HasFlag(PuzzleGearSize.Large)) >
                    availableGears.Count(x => x.Size.HasFlag(PuzzleGearSize.Large)))
                {
                    var targetSpot = gearSpots.First(x => x.Size.HasFlag(PuzzleGearSize.Large));
                    var targetGears = availableGears.Where(x => !x.Size.HasFlag(PuzzleGearSize.Large) && x.Color.HasFlag(targetSpot.Color)).ToList();
                    if (targetGears.Any())
                    {
                        var targetGearLetter = _pakDictionary.BestLetterForIndex(targetGears.Select(x => x.Letter).ToArray(), targetSpot.Index);
                        var targetGear = targetGears.First(x => x.Letter == targetGearLetter);
                        if (targetGear != null)
                        {
                            _logger.Debug("Optimizing tool - tool={0}, gear={1}", torch, targetGear);

                            availableGears.Remove(targetGear);
                            optimizations.Add(new PuzzleStep(targetGear, torch));
                        }
                    }
                }

                if (gearSpots.Count(x => x.Size.HasFlag(PuzzleGearSize.Small)) >
                    availableGears.Count(x => x.Size.HasFlag(PuzzleGearSize.Small)))
                {
                    var targetSpot = gearSpots.First(x => x.Size.HasFlag(PuzzleGearSize.Small));
                    var targetGears = availableGears.Where(x => !x.Size.HasFlag(PuzzleGearSize.Small) && x.Color.HasFlag(targetSpot.Color)).ToList();
                    if (targetGears.Any())
                    {
                        var targetGearLetter = _pakDictionary.BestLetterForIndex(targetGears.Select(x => x.Letter).ToArray(), targetSpot.Index);
                        var targetGear = targetGears.First(x => x.Letter == targetGearLetter);
                        if (targetGear != null)
                        {
                            _logger.Debug("Optimizing tool - tool={0}, gear={1}", torch, targetGear);

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
                        var targetSpot = gearSpots.First(x => x.Size.HasFlag(PuzzleGearSize.Large));
                        var targetGears = availableGears.Where(x => !x.Size.HasFlag(PuzzleGearSize.Large)).ToList();
                        if (targetGears.Any())
                        {
                            var targetGearLetter = _pakDictionary.BestLetterForIndex(targetGears.Select(x => x.Letter).ToArray(), targetSpot.Index);
                            var targetGear = targetGears.First(x => x.Letter == targetGearLetter);
                            if (targetGear != null)
                            {
                                _logger.Debug("Optimizing tool - tool={0}, gear={1}", torch, targetGear);

                                availableGears.Remove(targetGear);
                                optimizations.Add(new PuzzleStep(targetGear, torch));
                            }
                        }
                    }

                    if (gearSpots.Count(x => x.Size.HasFlag(PuzzleGearSize.Small)) >
                        availableGears.Count(x => x.Size.HasFlag(PuzzleGearSize.Small)))
                    {
                        var targetSpot = gearSpots.First(x => x.Size.HasFlag(PuzzleGearSize.Small));
                        var targetGears = availableGears.Where(x => !x.Size.HasFlag(PuzzleGearSize.Small)).ToList();
                        if (targetGears.Any())
                        {
                            var targetGearLetter = _pakDictionary.BestLetterForIndex(targetGears.Select(x => x.Letter).ToArray(), targetSpot.Index);
                            var targetGear = targetGears.First(x => x.Letter == targetGearLetter);
                            if (targetGear != null)
                            {
                                _logger.Debug("Optimizing tool - tool={0}, gear={1}", torch, targetGear);

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
                foreach (var size in new[] { PuzzleGearSize.Large, PuzzleGearSize.Small })
                {
                    if (gearSpots.Count(x => x.Color.HasFlag(PuzzleGearColor.Copper) && x.Size.HasFlag(size)) >
                        availableGears.Count(x => x.Color.HasFlag(PuzzleGearColor.Copper) && x.Size.HasFlag(size)))
                    {
                        var targetSpot = gearSpots.First(x => x.Color.HasFlag(PuzzleGearColor.Copper) && x.Size.HasFlag(size));
                        var targetGears = availableGears.Where(x => !x.Color.HasFlag(PuzzleGearColor.Copper) && x.Size.HasFlag(targetSpot.Size)).ToList();

                        // If we don't have any the size we want, let's paint one anyway
                        if (!targetGears.Any())
                        {
                            targetGears = availableGears.Where(x => !x.Color.HasFlag(PuzzleGearColor.Copper)).ToList();
                        }

                        if (targetGears.Any())
                        {
                            var targetGearLetter = _pakDictionary.BestLetterForIndex(targetGears.Select(x => x.Letter).ToArray(), targetSpot.Index);
                            var targetGear = targetGears.First(x => x.Letter == targetGearLetter);
                            if (targetGear != null)
                            {
                                _logger.Debug("Optimizing tool - tool={0}, gear={1}", copperPaint, targetGear);

                                availableGears.Remove(targetGear);
                                optimizations.Add(new PuzzleStep(targetGear, copperPaint));
                            }
                        }
                    }
                }
            }

            var silverPaint = tools.FirstOrDefault(x => x is PuzzlePaint && ((PuzzlePaint)x).Color.HasFlag(PuzzleGearColor.Silver));
            if (silverPaint != null)
            {
                foreach (var size in new[] { PuzzleGearSize.Large, PuzzleGearSize.Small })
                {
                    if (gearSpots.Count(x => x.Color.HasFlag(PuzzleGearColor.Silver) && x.Size.HasFlag(size)) >
                        availableGears.Count(x => x.Color.HasFlag(PuzzleGearColor.Silver) && x.Size.HasFlag(size)))
                    {
                        var targetSpot = gearSpots.First(x => x.Color.HasFlag(PuzzleGearColor.Silver) && x.Size.HasFlag(size));
                        var targetGears = availableGears.Where(x => !x.Color.HasFlag(PuzzleGearColor.Silver) && x.Size.HasFlag(targetSpot.Size)).ToList();

                        // If we don't have any the size we want, let's paint one anyway
                        if (!targetGears.Any())
                        {
                            targetGears = availableGears.Where(x => !x.Color.HasFlag(PuzzleGearColor.Silver)).ToList();
                        }

                        if (targetGears.Any())
                        {
                            var targetGearLetter = _pakDictionary.BestLetterForIndex(targetGears.Select(x => x.Letter).ToArray(), targetSpot.Index);
                            var targetGear = targetGears.First(x => x.Letter == targetGearLetter);
                            if (targetGear != null)
                            {
                                _logger.Debug("Optimizing tool - tool={0}, gear={1}", silverPaint, targetGear);

                                availableGears.Remove(targetGear);
                                optimizations.Add(new PuzzleStep(targetGear, silverPaint));
                            }
                        }
                    }
                }
            }

            var goldPaint = tools.FirstOrDefault(x => x is PuzzlePaint && ((PuzzlePaint)x).Color.HasFlag(PuzzleGearColor.Gold));
            if (goldPaint != null)
            {
                foreach (var size in new[] { PuzzleGearSize.Large, PuzzleGearSize.Small })
                {
                    if (gearSpots.Count(x => x.Color.HasFlag(PuzzleGearColor.Gold) && x.Size.HasFlag(size)) >
                        availableGears.Count(x => x.Color.HasFlag(PuzzleGearColor.Gold) && x.Size.HasFlag(size)))
                    {
                        ////_logger.Debug("We have gold gear spots, but no gold gears");

                        var targetSpot = gearSpots.First(x => x.Color.HasFlag(PuzzleGearColor.Gold) && x.Size.HasFlag(size));
                        var targetGears = availableGears.Where(x => !x.Color.HasFlag(PuzzleGearColor.Gold) && x.Size.HasFlag(targetSpot.Size)).ToList();

                        // If we don't have any the size we want, let's paint one anyway
                        if (!targetGears.Any())
                        {
                            targetGears = availableGears.Where(x => !x.Color.HasFlag(PuzzleGearColor.Gold)).ToList();
                        }

                        if (targetGears.Any())
                        {
                            ////_logger.Debug("We have gears with the correct size");

                            var targetGearLetter = _pakDictionary.BestLetterForIndex(targetGears.Select(x => x.Letter).ToArray(), targetSpot.Index);
                            var targetGear = targetGears.First(x => x.Letter == targetGearLetter);
                            if (targetGear != null)
                            {
                                _logger.Debug("Optimizing tool - tool={0}, gear={1}", goldPaint, targetGear);

                                availableGears.Remove(targetGear);
                                optimizations.Add(new PuzzleStep(targetGear, goldPaint));
                            }
                        }
                    }
                }
            }

            return optimizations;
        }

    }
}