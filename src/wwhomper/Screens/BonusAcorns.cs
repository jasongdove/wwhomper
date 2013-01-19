using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class BonusAcorns : TextScreen
    {
        private readonly Button _ok;

        public BonusAcorns(IAutoIt autoIt, IAssetCatalog assetCatalog)
            : base(autoIt, assetCatalog, 497, 192, 221, 21, "LET'S TAKE THEM TO THE BONUS")
        {
            AdditionalCharacters = "'";
            RequiresZoom = true;

            _ok = CreateCoordinateButton(550, 249, 99, 23);
        }

        public Button Ok
        {
            get { return _ok; }
        }
    }
}