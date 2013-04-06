using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Extensions.Logging;
using wwhomper.Data;
using wwhomper.Dictionary;

namespace wwhomper.Strategies
{
    public class TrashGearStrategy : ITrashGearStrategy
    {
        private readonly IPakDictionary _pakDictionary;
        private readonly ILogger _logger;

        public TrashGearStrategy(IPakDictionary pakDictionary, ILogger logger)
        {
            _pakDictionary = pakDictionary;
            _logger = logger;
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
                var worstGearWrongType = FindGearWithWorstLetter(wrongTypeGears);

                _logger.Debug(
                    "Determined worst letter - letters={0}, worst={1}, type=wrong",
                    String.Join(String.Empty, wrongTypeGears.Select(x => x.Letter)),
                    worstGearWrongType);

                return worstGearWrongType;
            }

            ////// Make sure we don't throw away a gear we need
            ////var gearWeNeed = _puzzleGameState.GearWeNeed;
            ////if (gearWeNeed != null)
            ////{
            ////    availableGears = availableGears.Where(x => !x.Color.HasFlag(gearWeNeed.Color) && !x.Size.HasFlag(gearWeNeed.Size)).ToList();
            ////}

            var result = FindGearWithWorstLetter(availableGears);

            _logger.Debug(
                "Determined worst letter - letters={0}, worst={1}, type=all",
                String.Join(String.Empty, availableGears.Select(x => x.Letter)),
                result);

            return result;
        }

        private PuzzleGear FindGearWithWorstLetter(List<PuzzleGear> gears)
        {
            var letters = gears.Select(x => x.Letter).ToArray();
            var targetLetter = _pakDictionary.WorstLetterOverall(letters);
            return gears.First(x => x.Letter == targetLetter);
        }
    }
}