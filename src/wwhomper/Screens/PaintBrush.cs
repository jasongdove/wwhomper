using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class PaintBrush : TextScreen, IDialogScreen
    {
        private readonly Button _ok;

        public PaintBrush(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                493, 160, 211, 22,
                "WANT TO SEE IF WE CAN FIX THE")
        {
            RequiresZoom = true;

            _ok = CreateCoordinateButton(547, 247, 40, 23);
        }

        public Button Accept
        {
            get { return _ok; }
        }
    }
}