using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;

namespace wwhomper.Screens
{
    public class Map : TemplateScreen, IDialogScreen
    {
        private readonly Button _gopherHole;
        private readonly Image<Bgra, byte> _currentLocation;
        private readonly Image<Bgra, byte> _openHole;
        private readonly List<Rectangle> _zones;

        public Map(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\ALL\Game\Map\MapScreen_Hole_Idle.jpg",
                94, 61, 33, 43,
                0, 0, 800, 600)
        {
            _gopherHole = CreateTemplateButton(
                @"Images\ALL\Game\Map\MapScreen_Gopher_Idle.jpg",
                105, 54, 11, 45);

            _currentLocation = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\Map\MapScreen_Hole_Idle.jpg")
                .Copy(new Rectangle(94, 61, 33, 43));

            _openHole = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\Map\MapScreen_Hole_Small.jpg")
                .Copy(new Rectangle(96, 85, 27, 13));

            _zones = new List<Rectangle>
            {
                new Rectangle(108, 45, 147, 261),
                new Rectangle(50, 294, 235, 284),
                new Rectangle(300, 238, 204, 333),
                new Rectangle(298, 0, 240, 275),
                new Rectangle(540, 0, 249, 306),
                new Rectangle(513, 278, 287, 322)
            };
        }

        public Button Accept
        {
            get { return _gopherHole; }
        }

        public Image<Bgra, byte> CurrentLocation
        {
            get { return _currentLocation; }
        }

        public Image<Bgra, byte> OpenHole
        {
            get { return _openHole; }
        }

        public List<Rectangle> Zones
        {
            get { return _zones; }
        }
    }
}