using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Extensions.Logging;
using wwhomper.Dictionary;

namespace wwhomper.Data
{
    public class PuzzleGameState
    {
        private readonly IPakDictionary _pakDictionary;
        private readonly ILogger _logger;
        private readonly List<PuzzleGearSpot> _gearSpots;
        private readonly List<PuzzleGear> _gears;

        public PuzzleGameState(IPakDictionary pakDictionary, ILogger logger)
        {
            _pakDictionary = pakDictionary;
            _logger = logger;
            _gearSpots = new List<PuzzleGearSpot>();
            _gears = new List<PuzzleGear>();
        }

        public List<PuzzleGearSpot> GearSpots
        {
            get { return _gearSpots; }
        }

        public List<PuzzleGear> Gears
        {
            get { return _gears; }
        }

        public PuzzleGearSpot GearWeNeed
        {
            get
            {
                var gearType = GearSpots.GroupBy(x => new { x.Color, x.Size }).ToList();
                
                // look for missing gears
                foreach (var spot in gearType)
                {
                    if (spot.Count() > Gears.Count(x => x.Color.HasFlag(spot.Key.Color) && x.Size.HasFlag(spot.Key.Size)))
                    {
                        _logger.Debug(
                            "We need a gear {0}/{1}",
                            spot.Key.Size,
                            spot.Key.Color);

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
                    if (Gears.Where(x => x.Color.HasFlag(spot.Key.Color) && x.Size.HasFlag(spot.Key.Size)).All(x => !vowels.Contains(x.Letter)))
                    {
                        _logger.Debug(
                            "We need a vowel gear {0}/{1}",
                            spot.Key.Size,
                            spot.Key.Color);

                        return new PuzzleGearSpot(
                            spot.Key.Size,
                            spot.Key.Color,
                            GetZoneIndex(spot.Key.Color, spot.Key.Size));
                    }
                }

                // look for a gear spot where we don't have a top 5 letter
                foreach (var spot in gearType)
                {
                    var gears = Gears.Where(x => x.Color.HasFlag(spot.Key.Color) && x.Size.HasFlag(spot.Key.Size)).ToList();
                    foreach (var s in spot)
                    {
                        var gearChoice = gears.FirstOrDefault(x => _pakDictionary.GetLetterRankingForIndex(x.Letter[0], s.Index) < 5);
                        if (gearChoice == null)
                        {
                            _logger.Debug(
                                "We need a top 5 gear {0}/{1}",
                                spot.Key.Size,
                                spot.Key.Color);

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
                    var gears = Gears.Where(x => x.Color.HasFlag(spot.Key.Color) && x.Size.HasFlag(spot.Key.Size)).ToList();
                    if (gears.All(x => vowels.Contains(x.Letter[0])))
                    {
                        _logger.Debug(
                            "We need a consonant gear {0}/{1}",
                            spot.Key.Size,
                            spot.Key.Color);

                        return new PuzzleGearSpot(
                            spot.Key.Size,
                            spot.Key.Color,
                            GetZoneIndex(spot.Key.Color, spot.Key.Size));
                    }
                }

                return null;
            }
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