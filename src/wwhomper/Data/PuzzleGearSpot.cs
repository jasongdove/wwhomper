using System;

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

        public override string ToString()
        {
            var result = String.Empty;
            
            if (Size.HasFlag(PuzzleGearSize.Small))
            {
                result += "S";
            }

            if (Size.HasFlag(PuzzleGearSize.Large))
            {
                result += "L";
            }

            result += "/";

            if (Color.HasFlag(PuzzleGearColor.Copper))
            {
                result += "C";
            }

            if (Color.HasFlag(PuzzleGearColor.Silver))
            {
                result += "S";
            }

            if (Color.HasFlag(PuzzleGearColor.Gold))
            {
                result += "G";
            }

            return result;
        }
    }
}