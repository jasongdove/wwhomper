using System.Drawing;
using wwhomper.Screens;

namespace wwhomper
{
    public class ScreenSearchResult
    {
        public bool Success { get; set; }
        public Point Point { get; set; }
        public ScreenBase Screen { get; set; }
    }
}