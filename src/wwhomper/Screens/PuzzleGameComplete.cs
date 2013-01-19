using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class PuzzleGameComplete : TemplateScreen
    {
        private readonly Button _ok;

        public PuzzleGameComplete(IAutoIt autoIt, IAssetCatalog assetCatalog)
            : base(autoIt, assetCatalog, @"Images\ALL\Game\puzzle_game\PuzzleGame_Background.jpg", 11, 556, 277, 35)
        {
            _ok = CreateCoordinateButton(609, 332, 95, 23);
        }

        public Button Ok
        {
            get { return _ok; }
        }
    }
}