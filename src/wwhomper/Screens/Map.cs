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
                20, 47, 740, 561)
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
                new Rectangle(110, 48, 155, 290),
                new Rectangle(55, 332, 237, 277),
                new Rectangle(305, 281, 227, 316),
                new Rectangle(310, 18, 237, 285),
                new Rectangle(540, 14, 209, 297),
                new Rectangle(513, 307, 227, 270)
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