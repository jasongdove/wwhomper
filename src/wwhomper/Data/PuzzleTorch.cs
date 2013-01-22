using System.Drawing;

namespace wwhomper.Data
{
    public class PuzzleTorch : PuzzleTool
    {
        public PuzzleTorch(Rectangle pickupArea)
            : base(pickupArea)
        {
        }

        public override string ToString()
        {
            return "Torch";
        }
    }
}