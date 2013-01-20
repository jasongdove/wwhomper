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
                var gearType = GearSpots.GroupBy(x => new { x.Color, x.Size });
                foreach (var spot in gearType)
                {
                    if (spot.Count() > Gears.Count(x => x.Color.HasFlag(spot.Key.Color) && x.Size.HasFlag(spot.Key.Size)))
                    {
                        Console.WriteLine("We need a gear of size {0} with color {1}", spot.Key.Size, spot.Key.Color);
                        return new PuzzleGearSpot(spot.Key.Size, spot.Key.Color, -1);
                    }
                }

                return null;
            }
        }
    }
}