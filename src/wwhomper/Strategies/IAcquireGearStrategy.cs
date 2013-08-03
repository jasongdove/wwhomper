using wwhomper.Data;

namespace wwhomper.Strategies
{
    public interface IAcquireGearStrategy
    {
        PuzzleGearSpot FindGearWeNeed();
    }
}