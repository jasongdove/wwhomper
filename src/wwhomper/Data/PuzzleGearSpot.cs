namespace wwhomper.Data
{
    public class PuzzleGearSpot
    {
        private readonly PuzzleGearSize _size;
        private readonly PuzzleGearColor _color;
        private readonly int _index;

        public PuzzleGearSpot(PuzzleGearSize size, PuzzleGearColor color, int index)
        {
            _size = size;
            _color = color;
            _index = index;
        }

        public PuzzleGearSize Size
        {
            get { return _size; }
        }

        public PuzzleGearColor Color
        {
            get { return _color; }
        }

        public int Index
        {
            get { return _index; }
        }
    }
}