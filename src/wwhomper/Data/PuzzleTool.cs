using System.Drawing;

namespace wwhomper.Data
{
    public abstract class PuzzleTool
    {
        private readonly Rectangle _pickupArea;

        protected PuzzleTool(Rectangle pickupArea)
        {
            _pickupArea = pickupArea;
        }

        public Rectangle PickupArea
        {
            get { return _pickupArea; }
        }
    }
}