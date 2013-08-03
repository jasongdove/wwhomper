using System.Collections.Generic;
using wwhomper.Data;

namespace wwhomper.Strategies
{
    public interface ITrashGearStrategy
    {
        PuzzleGear FindGearToTrash(List<PuzzleGearSpot> gearSpots, List<PuzzleGear> gears);
    }
}