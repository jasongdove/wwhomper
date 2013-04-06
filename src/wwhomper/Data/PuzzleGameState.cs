using System.Collections.Generic;

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
    }
}