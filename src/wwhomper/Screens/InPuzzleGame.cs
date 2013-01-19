using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using Ninject.Extensions.Logging;
using sharperbot;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Screens.Controls;
using wwhomper.Data;

namespace wwhomper.Screens
{
    public class InPuzzleGame : TemplateScreen
    {
        private readonly Button _submit;
        private readonly Button _back;
        private readonly Image<Bgra, byte> _inactiveGear;
        private readonly Image<Bgra, byte> _largeCopperGear;
        private readonly Image<Bgra, byte> _largeWildcardGear;
        private readonly Image<Bgra, byte> _largeCopperGearSpot;
        private readonly Image<Bgra, byte> _smallCopperGear;
        private readonly Image<Bgra, byte> _smallWildcardGear;
        private readonly Dictionary<Rectangle, Rectangle> _letterSearchAreaTesseractArea;
        private readonly Button _gearOne;
        private readonly Button _gearTwo;
        private readonly Button _gearThree;
        private readonly Button _gearFour;
        private readonly Rectangle _gearSpotOne;
        private readonly Rectangle _gearSpotTwo;
        private readonly Rectangle _gearSpotThree;
        private readonly Rectangle _gearSpotFour;

        public InPuzzleGame(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\ALL\Game\puzzle_game\PuzzleGame_Background.jpg",
                11, 556, 277, 35,
                0, 569, 304, 60)
        {
            _submit = CreateCoordinateButton(271, 330, 93, 41);
            _back = CreateCoordinateButton(78, 66, 72, 22);

            _inactiveGear = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\Gear_NonInteractive.jpg")
                .Copy(new Rectangle(25, 26, 38, 38));

            _largeCopperGear = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\Gear_Large_Copper.jpg")
                .Copy(new Rectangle(25, 67, 39, 3));

            _largeWildcardGear = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\Gear_Large_Wildcard.jpg")
                .Copy(new Rectangle(30, 33, 28, 25));

            _largeCopperGearSpot = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\GearSpot_Large_Copper.jpg")
                .Copy(new Rectangle(30, 32, 29, 29));

            _smallCopperGear = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\Gear_Small_Copper.jpg")
                .Copy(new Rectangle(27, 57, 34, 8));

            _smallWildcardGear = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\Gear_Small_Wildcard.jpg")
                .Copy(new Rectangle(28, 29, 32, 32));

            _letterSearchAreaTesseractArea = new Dictionary<Rectangle, Rectangle>
            {
                // top row
                { new Rectangle(32, 395, 76, 82), new Rectangle(58, 423, 25, 27) },
                { new Rectangle(122, 393, 79, 86), new Rectangle(149, 423, 25, 27) },
                { new Rectangle(217, 398, 73, 78), new Rectangle(241, 423, 25, 27) },
                { new Rectangle(304, 398, 75, 77), new Rectangle(329, 423, 25, 27) },
                { new Rectangle(395, 397, 76, 79), new Rectangle(420, 423, 25, 27) },
                { new Rectangle(486, 396, 76, 83), new Rectangle(512, 423, 25, 27) },
                { new Rectangle(580, 400, 73, 76), new Rectangle(603, 423, 25, 27) },
                { new Rectangle(673, 400, 72, 74), new Rectangle(696, 423, 25, 27) },

                // bottom row
                { new Rectangle(36, 490, 72, 73), new Rectangle(59, 511, 25, 27) },
                { new Rectangle(126, 486, 71, 78), new Rectangle(149, 511, 25, 27) },
                { new Rectangle(218, 488, 71, 74), new Rectangle(240, 511, 25, 27) },
                { new Rectangle(303, 488, 74, 73), new Rectangle(328, 511, 25, 27) },
                { new Rectangle(397, 490, 70, 72), new Rectangle(419, 511, 25, 27) },
                { new Rectangle(488, 486, 73, 77), new Rectangle(512, 511, 25, 27) },
                { new Rectangle(579, 487, 73, 75), new Rectangle(603, 511, 25, 27) },
            };

            _gearOne = CreateCoordinateButton(231, 144, 23, 21);
            _gearTwo = CreateCoordinateButton(288, 175, 23, 21);
            _gearThree = CreateCoordinateButton(351, 165, 23, 21);
            _gearFour = CreateCoordinateButton(407, 192, 23, 21);

            _gearSpotOne = new Rectangle(206, 119, 71, 73);
            _gearSpotTwo = new Rectangle(265, 150, 69, 69);
            _gearSpotThree = new Rectangle(327, 138, 71, 73);
            _gearSpotFour = new Rectangle(380, 164, 74, 74);
        }

        public Button Submit
        {
            get { return _submit; }
        }

        public Button Back
        {
            get { return _back; }
        }

        public void ClearAllGears()
        {
            _gearOne.Click();
            AutoIt.Type("{BACKSPACE}");

            _gearTwo.Click();
            AutoIt.Type("{BACKSPACE}");

            _gearThree.Click();
            AutoIt.Type("{BACKSPACE}");

            _gearFour.Click();
            AutoIt.Type("{BACKSPACE}");
        }

        public List<PuzzleGearSize> GetRequiredGears(Image<Bgra, byte> windowContents)
        {
            var result = new List<PuzzleGearSize>();

            var spotOne = windowContents.Copy(_gearSpotOne);
            if (AutoIt.IsTemplateInWindow(spotOne, _largeCopperGearSpot).Success)
            {
                result.Add(PuzzleGearSize.Large);
            }
            else
            {
                result.Add(PuzzleGearSize.Small);
            }

            var spotTwo = windowContents.Copy(_gearSpotTwo);
            if (AutoIt.IsTemplateInWindow(spotTwo, _largeCopperGearSpot).Success)
            {
                result.Add(PuzzleGearSize.Large);
            }
            else
            {
                result.Add(PuzzleGearSize.Small);
            }

            var spotThree = windowContents.Copy(_gearSpotThree);
            if (AutoIt.IsTemplateInWindow(spotThree, _largeCopperGearSpot).Success)
            {
                result.Add(PuzzleGearSize.Large);
            }
            else
            {
                result.Add(PuzzleGearSize.Small);
            }

            var spotFour = windowContents.Copy(_gearSpotFour);
            if (AutoIt.IsTemplateInWindow(spotFour, _largeCopperGearSpot).Success)
            {
                result.Add(PuzzleGearSize.Large);
            }
            else
            {
                result.Add(PuzzleGearSize.Small);
            }

            // TODO: Support 5 gears
            
            return result;
        }

        public List<PuzzleLetter> GetAvailableLetters(Image<Bgra, byte> windowContents)
        {
            var result = new List<PuzzleLetter>();
            var letters = new List<Image<Gray, byte>>();

            foreach (var entry in _letterSearchAreaTesseractArea)
            {
                PuzzleLetter letter = null;

                var searchArea = windowContents.Copy(entry.Key);
                if (AutoIt.IsTemplateInWindow(searchArea, _largeWildcardGear).Success)
                {
                    letter = new PuzzleLetter("*", PuzzleGearSize.Large);
                }
                else if (AutoIt.IsTemplateInWindow(searchArea, _smallWildcardGear).Success)
                {
                    letter = new PuzzleLetter("*", PuzzleGearSize.Small);
                }
                else if (AutoIt.IsTemplateInWindow(searchArea, _largeCopperGear).Success)
                {
                    var letterImage = windowContents.Copy(entry.Value).Convert<Gray, byte>();
                    letterImage.Floor(245);
                    letters.Add(letterImage);

                    var text = GetZoomedOutText(letterImage, 2).Trim();
                    letter = new PuzzleLetter(text, PuzzleGearSize.Large);
                }
                else if (AutoIt.IsTemplateInWindow(searchArea, _smallCopperGear).Success)
                {
                    var letterImage = windowContents.Copy(entry.Value).Convert<Gray, byte>();
                    letterImage.Floor(245);
                    letters.Add(letterImage);

                    var text = GetZoomedOutText(letterImage, 2).Trim();
                    letter = new PuzzleLetter(text, PuzzleGearSize.Small);
                }

                if (letter != null)
                {
                    result.Add(letter);
                }
            }

            if (result.Any())
            {
                var combined = Combine(letters);
                var allText = GetZoomedOutText(combined, 2, "*").Trim().Replace(" ", String.Empty);
                var withoutWildcards = result.Where(x => x.Letter != "*").ToList();
                if (allText.Length == withoutWildcards.Count)
                {
                    foreach (var r in withoutWildcards.Where(x => String.IsNullOrEmpty(x.Letter)).ToList())
                    {
                        r.Letter = allText[withoutWildcards.IndexOf(r)].ToString(CultureInfo.InvariantCulture);
                    }
                }
            }

            return result;
        }

        public void SubmitWord(List<PuzzleLetter> guess)
        {
            if (guess.Count != 4)
            {
                return;
            }

            TypeLetter(guess[0], _gearOne);
            TypeLetter(guess[1], _gearTwo);
            TypeLetter(guess[2], _gearThree);
            TypeLetter(guess[3], _gearFour);

            _submit.Click();
        }

        private void TypeLetter(PuzzleLetter letter, Button gear)
        {
            if (letter.Letter == "*")
            {
                var windowContents = AutoIt.GetWindowImage();
                foreach (var entry in _letterSearchAreaTesseractArea)
                {
                    var searchArea = windowContents.Copy(entry.Key);

                    if (AutoIt.IsTemplateInWindow(searchArea, _largeWildcardGear).Success)
                    {
                        AutoIt.Click(entry.Key);
                        break;
                    }

                    if (AutoIt.IsTemplateInWindow(searchArea, _smallWildcardGear).Success)
                    {
                        AutoIt.Click(entry.Key);
                        break;
                    }
                }
            }
            else
            {
                AutoIt.Type(letter.Letter);
            }

            gear.Click();
        }
    }
}