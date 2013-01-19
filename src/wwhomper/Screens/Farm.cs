using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class Farm : TemplateScreen
    {
        private readonly Button _gopherHole;

        public Farm(IAutoIt autoIt, IAssetCatalog assetCatalog)
            : base(
            autoIt,
            assetCatalog,
            @"Images\ALL\Game\Map\MapScreen_Hole_Idle.jpg",
            94, 61, 33, 43,
            0, 0, 800, 600)
        {
            _gopherHole = CreateTemplateButton(
                @"Images\ALL\Game\Map\MapScreen_Gopher_Idle.jpg",
                105, 54, 11, 45);
        }

        public Button GopherHole
        {
            get { return _gopherHole; }
        }
    }
}