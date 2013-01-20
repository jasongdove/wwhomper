﻿using Ninject.Extensions.Logging;
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
                497, 192, 221, 21,
                "WANT TO SEE IF WE CAN FIX THE")
        {
            RequiresZoom = true;

            _ok = CreateCoordinateButton(550, 249, 99, 23);
        }

        public Button Accept
        {
            get { return _ok; }
        }
    }
}