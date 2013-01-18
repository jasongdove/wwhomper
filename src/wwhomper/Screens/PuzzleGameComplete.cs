using System.Drawing;
using wwhomper.Pak;
using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class PuzzleGameComplete : TemplateScreen
    {
        private readonly CoordinateButton _ok;

        public PuzzleGameComplete(PakCatalog pakCatalog)
            : base(pakCatalog, @"Images\ALL\Game\puzzle_game\PuzzleGame_Background.jpg", new Rectangle(11, 556, 277, 35))
        {
            _ok = new CoordinateButton(609, 332, 95, 23);
        }

        public CoordinateButton Ok
        {
            get { return _ok; }
        }
    }
}