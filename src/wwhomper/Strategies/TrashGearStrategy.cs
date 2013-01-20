using System.Collections.Generic;
using System.Linq;
using wwhomper.Data;
using wwhomper.Dictionary;

namespace wwhomper.Strategies
{
    public class TrashGearStrategy
    {
        private readonly IPakDictionary _pakDictionary;

        public TrashGearStrategy(IPakDictionary pakDictionary)
        {
            _pakDictionary = pakDictionary;
        }

        public PuzzleGear FindGearToTrash(List<PuzzleGearSpot> gearSpots, List<PuzzleGear> gears)
        {
            // TODO: Maybe base this on color/size rather than a simple overall frequency

            var availableGears = new List<PuzzleGear>(gears);

            var gearTypes = gearSpots.GroupBy(x => new { x.Color, x.Size }).ToList();

            // Try to throw away a color/size we don't need
            var allGears = new List<PuzzleGear>(availableGears);
            var wrongTypeGears = allGears.Where(g => gearTypes.All(x => !g.Color.HasFlag(x.Key.Color) || !g.Size.HasFlag(x.Key.Size))).ToList();
            if (wrongTypeGears.Any())
            {
                return FindGearWithWorstLetter(wrongTypeGears);
            }

            ////// Make sure we don't throw away a gear we need
            ////var gearWeNeed = _puzzleGameState.GearWeNeed;
            ////if (gearWeNeed != null)
            ////{
            ////    availableGears = availableGears.Where(x => !x.Color.HasFlag(gearWeNeed.Color) && !x.Size.HasFlag(gearWeNeed.Size)).ToList();
            ////}

            return FindGearWithWorstLetter(availableGears);
        }

        private PuzzleGear FindGearWithWorstLetter(List<PuzzleGear> gears)
        {
            var letters = gears.Select(x => x.Letter[0]).ToArray();
            var targetLetter = _pakDictionary.WorstLetterOverall(letters);
            return gears.First(x => x.Letter[0] == targetLetter);
        }
    }
}