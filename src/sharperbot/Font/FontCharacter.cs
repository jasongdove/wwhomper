using System.Diagnostics;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace sharperbot.Font
{
    [DebuggerDisplay("{Character} - {Width}")]
    public class FontCharacter
    {
        public char Character { get; set; }
        public int Width { get; set; }
        public Rectangle Rectangle { get; set; }
        public Point Offset { get; set; }
        public Image<Bgra, byte> MatchImage { get; set; }
    }
}