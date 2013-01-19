﻿using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class NewGear : TextScreen
    {
        private readonly Button _no;
        private readonly Button _yes;

        public NewGear(IAutoIt autoIt, IAssetCatalog assetCatalog)
            : base(autoIt, assetCatalog, 492, 166, 168, 25, "YOU FOUND A NEW GEAR!")
        {
            AdditionalCharacters = "!";
            RequiresZoom = true;

            _no = CreateCoordinateButton(615, 254, 93, 24);
            _yes = CreateCoordinateButton(492, 250, 96, 22);
        }

        public Button No
        {
            get { return _no; }
        }

        public Button Yes
        {
            get { return _yes; }
        }
    }
}