﻿using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class LocIntroComplete : TemplateScreen
    {
        private readonly Button _ok;

        public LocIntroComplete(IAutoIt autoIt, IAssetCatalog assetCatalog)
            : base(
                autoIt,
                assetCatalog,
                @"Images\ALL\Dialog\Dialog_Loc\Dialog_Loc_BG.jpg",
                641, 464, 150, 127,
                593, 451, 213, 178)
        {
            _ok = CreateCoordinateButton(329, 545, 152, 38);
        }

        public Button Ok
        {
            get { return _ok; }
        }
    }
}