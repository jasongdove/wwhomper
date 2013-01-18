using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using wwhomper.Pak;
using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class InPuzzleGame : TemplateScreen
    {
        private readonly CoordinateButton _submit;
        private readonly CoordinateButton _back;
        private readonly Image<Bgra, byte> _inactiveGear;
        private readonly Image<Bgra, byte> _largeCopperGear;
        private readonly Image<Bgra, byte> _largeWildcardGear;
        private readonly Dictionary<Rectangle, Rectangle> _letterSearchAreaTesseractArea;
        private readonly CoordinateButton _gearOne;
        private readonly CoordinateButton _gearTwo;
        private readonly CoordinateButton _gearThree;
        private readonly CoordinateButton _gearFour;

        public InPuzzleGame(PakCatalog pakCatalog)
            : base(pakCatalog, @"Images\ALL\Game\puzzle_game\PuzzleGame_Background.jpg", new Rectangle(11, 556, 277, 35))
        {
            _submit = new CoordinateButton(271, 330, 93, 41);
            _back = new CoordinateButton(78, 66, 72, 22);
            
            _inactiveGear = pakCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\Gear_NonInteractive.jpg")
                .GetSubRect(new Rectangle(25, 26, 38, 38));

            _largeCopperGear = pakCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\Gear_Large_Copper.jpg")
                .GetSubRect(new Rectangle(25, 67, 39, 3));

            _largeWildcardGear = pakCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\Gear_Large_Wildcard.jpg")
                .GetSubRect(new Rectangle(30, 33, 28, 25));

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

            _gearOne = new CoordinateButton(231, 144, 23, 21);
            _gearTwo = new CoordinateButton(288, 175, 23, 21);
            _gearThree = new CoordinateButton(351, 165, 23, 21);
            _gearFour = new CoordinateButton(407, 192, 23, 21);
        }

        public CoordinateButton Submit
        {
            get { return _submit; }
        }

        public CoordinateButton Back
        {
            get { return _back; }
        }

        public void ClearAllGears()
        {
            _gearOne.Click();
            AutoIt.Type(WordWhomper.WindowTitle, "{BACKSPACE}");

            _gearTwo.Click();
            AutoIt.Type(WordWhomper.WindowTitle, "{BACKSPACE}");

            _gearThree.Click();
            AutoIt.Type(WordWhomper.WindowTitle, "{BACKSPACE}");

            _gearFour.Click();
            AutoIt.Type(WordWhomper.WindowTitle, "{BACKSPACE}");
        }

        public int GetRequiredLetterCount(Image<Bgra, byte> windowContents)
        {
            // look for the inactive gear in position 5
            var searchArea = windowContents.GetSubRect(new Rectangle(444, 152, 74, 84));
            return AutoIt.IsTemplateInWindow(searchArea, _inactiveGear).Success ? 4 : 5;
        }

        public string GetAvailableLetters(Image<Bgra, byte> windowContents)
        {
            var letters = new List<Image<Bgra, byte>>();
            var wildcards = new List<string>();

            foreach (var entry in _letterSearchAreaTesseractArea)
            {
                var searchArea = windowContents.GetSubRect(entry.Key);
                if (AutoIt.IsTemplateInWindow(searchArea, _largeWildcardGear).Success)
                {
                    wildcards.Add("*");
                }
                else if (AutoIt.IsTemplateInWindow(searchArea, _largeCopperGear).Success)
                {
                    var letterImage = windowContents.GetSubRect(entry.Value);
                    letters.Add(letterImage);
                }
            }

            if (!letters.Any())
            {
                return null;
            }

            var combined = Combine(letters);
            var text = GetZoomedOutText(combined, 2, "*", true)
                .Trim()
                .Replace(" ", String.Empty);

            return text + String.Join(String.Empty, wildcards);
        }

        public void SubmitWord(Image<Bgra, byte> windowContents, string guess)
        {
            if (guess.Length != 4)
            {
                return;
            }

            TypeLetter(windowContents, guess[0], _gearOne);
            TypeLetter(windowContents, guess[1], _gearTwo);
            TypeLetter(windowContents, guess[2], _gearThree);
            TypeLetter(windowContents, guess[3], _gearFour);

            _submit.Click();
        }

        private void TypeLetter(Image<Bgra, byte> windowContents, char letter, CoordinateButton gear)
        {
            if (letter == '*')
            {
                foreach (var entry in _letterSearchAreaTesseractArea)
                {
                    var searchArea = windowContents.GetSubRect(entry.Key);
                    if (AutoIt.IsTemplateInWindow(searchArea, _largeWildcardGear).Success)
                    {
                        AutoIt.Click(entry.Key);
                        break;
                    }
                }
            }
            else
            {
                AutoIt.Type(WordWhomper.WindowTitle, letter.ToString(CultureInfo.InvariantCulture));
            }

            gear.Click();
        }
    }
}