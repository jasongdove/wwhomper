using System.Linq;
using Ninject.Extensions.Logging;
using wwhomper.Data;
using wwhomper.Dictionary;

namespace wwhomper.Strategies
{
    public class AcquireGearStrategy : IAcquireGearStrategy
    {
        private readonly IPakDictionary _pakDictionary;
        private readonly ILogger _logger;
        private readonly PuzzleGameState _puzzleGameState;

        public AcquireGearStrategy(
            IPakDictionary pakDictionary,
            ILogger logger,
            PuzzleGameState puzzleGameState)
        {
            _pakDictionary = pakDictionary;
            _logger = logger;
            _puzzleGameState = puzzleGameState;
        }

        public PuzzleGearSpot FindGearWeNeed()
        {
            var gearType = _puzzleGameState.GearSpots.GroupBy(x => new { x.Color, x.Size }).ToList();

            // look for missing gears
            foreach (var spot in gearType)
            {
                if (spot.Count() > _puzzleGameState.Gears.Count(x => x.Color.HasFlag(spot.Key.Color) && x.Size.HasFlag(spot.Key.Size)))
                {
                    _logger.Debug("Target gear - gearSpot={0}, type=any", new PuzzleGearSpot(spot.Key.Size, spot.Key.Color, -1));

                    return new PuzzleGearSpot(
                        spot.Key.Size,
                        spot.Key.Color,
                        GetZoneIndex(spot.Key.Color, spot.Key.Size));
                }
            }

            // look for missing vowels
            const string vowels = "AEIOU";
            foreach (var spot in gearType)
            {
                if (_puzzleGameState.Gears.Where(x => x.Color.HasFlag(spot.Key.Color) && x.Size.HasFlag(spot.Key.Size)).All(x => !vowels.Contains(x.Letter)))
                {
                    _logger.Debug("Target gear - gearSpot={0}, type=vowel", new PuzzleGearSpot(spot.Key.Size, spot.Key.Color, -1));

                    return new PuzzleGearSpot(
                        spot.Key.Size,
                        spot.Key.Color,
                        GetZoneIndex(spot.Key.Color, spot.Key.Size));
                }
            }

            // look for a gear spot where we don't have a top 5 letter
            foreach (var spot in gearType)
            {
                var gears = _puzzleGameState.Gears.Where(x => x.Color.HasFlag(spot.Key.Color) && x.Size.HasFlag(spot.Key.Size)).ToList();
                foreach (var s in spot)
                {
                    var gearChoice = gears.FirstOrDefault(x => _pakDictionary.GetLetterRankingForIndex(x.Letter, s.Index) < 5);
                    if (gearChoice == null)
                    {
                        _logger.Debug("Target gear - gearSpot={0}, type=top5", new PuzzleGearSpot(spot.Key.Size, spot.Key.Color, -1));

                        return new PuzzleGearSpot(
                            s.Size,
                            s.Color,
                            GetZoneIndex(s.Color, s.Size));
                    }

                    gears.Remove(gearChoice);
                }
            }

            // we don't want only vowels either
            foreach (var spot in gearType)
            {
                var gears = _puzzleGameState.Gears.Where(x => x.Color.HasFlag(spot.Key.Color) && x.Size.HasFlag(spot.Key.Size)).ToList();
                if (gears.All(x => vowels.Contains(x.Letter)))
                {
                    _logger.Debug("Target gear - gearSpot={0}, type=consonant", new PuzzleGearSpot(spot.Key.Size, spot.Key.Color, -1));

                    return new PuzzleGearSpot(
                        spot.Key.Size,
                        spot.Key.Color,
                        GetZoneIndex(spot.Key.Color, spot.Key.Size));
                }
            }

            return null;
        }

        private int GetZoneIndex(PuzzleGearColor color, PuzzleGearSize size)
        {
            int index = -1;

            if (color.HasFlag(PuzzleGearColor.Copper) &&
                size.HasFlag(PuzzleGearSize.Large))
            {
                index = 0;
            }
            else if (color.HasFlag(PuzzleGearColor.Copper) &&
                     size.HasFlag(PuzzleGearSize.Small))
            {
                index = 1;
            }
            else if (color.HasFlag(PuzzleGearColor.Silver) &&
                     size.HasFlag(PuzzleGearSize.Small))
            {
                index = 2;
            }
            else if (color.HasFlag(PuzzleGearColor.Silver) &&
                     size.HasFlag(PuzzleGearSize.Large))
            {
                index = 3;
            }
            else if (color.HasFlag(PuzzleGearColor.Gold) &&
                     size.HasFlag(PuzzleGearSize.Small))
            {
                index = 4;
            }
            else if (color.HasFlag(PuzzleGearColor.Gold) &&
                     size.HasFlag(PuzzleGearSize.Large))
            {
                index = 5;
            }

            return index;
        }
    }
}