using System;
using System.Collections.Generic;
using System.Linq;

namespace wwhomper.Data
{
    public class PuzzleGameState
    {
        private readonly List<PuzzleGearSpot> _gearSpots;
        private readonly List<PuzzleGear> _gears;

        public PuzzleGameState()
        {
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
                        Console.WriteLine(
                            "We need a gear of size {0} with color {1}",
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
                        Console.WriteLine(
                            "We need a vowel gear of size {0} with color {1}",
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