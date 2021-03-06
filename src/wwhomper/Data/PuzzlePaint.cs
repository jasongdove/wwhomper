﻿using System.Drawing;

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

        public override string ToString()
        {
            string result = "Paint/";

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