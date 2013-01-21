using System.Drawing;

namespace wwhomper.Data
{
    public class PuzzleGear
    {
        private readonly PuzzleGearSize _size;
        private readonly PuzzleGearColor _color;
        private readonly Rectangle _pickupArea;

        public PuzzleGear(char letter, PuzzleGearSize size, PuzzleGearColor color, Rectangle pickupArea)
        {
            Letter = letter;
            _size = size;
            _color = color;
            _pickupArea = pickupArea;
        }

        public char Letter { get; set; }

        public PuzzleGearSize Size
        {
            get { return _size; }
        }

        public PuzzleGearColor Color
        {
            get { return _color; }
        }

        public Rectangle PickupArea
        {
            get { return _pickupArea; }
        }

        public bool IsWildcard
        {
            get { return Letter == '*'; }
        }
    }
}