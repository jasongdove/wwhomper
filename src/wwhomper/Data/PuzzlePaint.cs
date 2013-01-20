using System.Drawing;

namespace wwhomper.Data
{
    public class PuzzlePaint : PuzzleTool
    {
        private readonly PuzzleGearColor _color;

        public PuzzlePaint(PuzzleGearColor color, Rectangle pickupArea)
            : base(pickupArea)
        {
            _color = color;
        }

        public PuzzleGearColor Color
        {
            get { return _color; }
        }
    }
}