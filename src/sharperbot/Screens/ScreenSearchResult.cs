using System.Drawing;

namespace sharperbot.Screens
{
    public class ScreenSearchResult
    {
        public bool Success { get; set; }
        public Point Point { get; set; }
        public IGameScreen Screen { get; set; }
    }
}