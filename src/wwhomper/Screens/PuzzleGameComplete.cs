using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class PuzzleGameComplete : TemplateScreen, IDialogScreen
    {
        private readonly Button _ok;

        public PuzzleGameComplete(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\ALL\Game\puzzle_game\PuzzleGame_Background.jpg",
                11, 556, 277, 35,
                0, 547, 389, 82)
        {
            _ok = CreateCoordinateButton(603, 326, 97, 24);
        }

        public Button Accept
        {
            get { return _ok; }
        }
    }
}