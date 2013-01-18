using System.Diagnostics;
using System.Drawing;

namespace wwhomper.Font
{
    [DebuggerDisplay("{Character} - {Width}")]
    public class FontCharacter
    {
        public char Character { get; set; }
        public int Width { get; set; }
        public Rectangle Rectangle { get; set; }
        public Point Offset { get; set; }
    }
}