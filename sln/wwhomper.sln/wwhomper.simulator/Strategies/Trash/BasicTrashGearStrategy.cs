using System.Collections.Generic;
using System.Linq;
using wwhomper.Data;
using wwhomper.Strategies;

namespace wwhomper.simulator.Strategies.Trash
{
    public class BasicTrashGearStrategy : ITrashGearStrategy
    {
        public PuzzleGear FindGearToTrash(List<PuzzleGearSpot> gearSpots, List<PuzzleGear> gears)
        {
            return gears.First();
        }
    }
}
