using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Ninject.Extensions.Logging;
using wwhomper.Data;
using wwhomper.Dictionary;
using wwhomper.Strategies;

namespace wwhomper.tests.Strategies
{
    [TestFixture]
    public class TrashGearStrategyTests
    {
        private Mock<IPakDictionary> _pakDictionary;
        private Mock<ILogger> _logger;
        private TrashGearStrategy _strategy;

        [SetUp]
        public void Setup()
        {
            _pakDictionary = new Mock<IPakDictionary>();
            _logger = new Mock<ILogger>();
            
            _strategy = new TrashGearStrategy(
                _pakDictionary.Object,
                _logger.Object);
        }

        [Test]
        public void FindGearToTrash_Should_Return_Size_We_Dont_Need()
        {
            // Arrange
            var gearSpots = new List<PuzzleGearSpot>
            {
                new PuzzleGearSpot(PuzzleGearSize.Large, PuzzleGearColor.Copper, 0),
                new PuzzleGearSpot(PuzzleGearSize.Large, PuzzleGearColor.Silver, 1),
                new PuzzleGearSpot(PuzzleGearSize.Large, PuzzleGearColor.Gold, 2),
                new PuzzleGearSpot(PuzzleGearSize.Large, PuzzleGearColor.Copper, 3)
            };

            var gears = new List<PuzzleGear>
            {
                new PuzzleGear('A', PuzzleGearSize.Large, PuzzleGearColor.Copper, Rectangle.Empty),
                new PuzzleGear('A', PuzzleGearSize.Large, PuzzleGearColor.Silver, Rectangle.Empty),
                new PuzzleGear('A', PuzzleGearSize.Large, PuzzleGearColor.Gold, Rectangle.Empty),
                new PuzzleGear('A', PuzzleGearSize.Small, PuzzleGearColor.Copper, Rectangle.Empty)
            };

            _pakDictionary.Setup(x => x.WorstLetterOverall(It.IsAny<char[]>())).Returns<char[]>(x => x.First());

            // Act
            var gearToTrash = _strategy.FindGearToTrash(gearSpots, gears);

            // Assert
            gearToTrash.Size.Should().Be(PuzzleGearSize.Small);
        }
    }
}
