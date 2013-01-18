using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using wwhomper.Pak;
using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class InBonusGame : TemplateScreen
    {
        private readonly Dictionary<TemplateSearchArea, List<TemplateCoordinate>> _letterGroups =
            new Dictionary<TemplateSearchArea, List<TemplateCoordinate>>();

        public InBonusGame(PakCatalog pakCatalog, BonusGameWaiting waiting)
            : base(pakCatalog, @"Images\ALL\Game\bonus_game\BG_Doors_Upper_Idle.jpg", new Rectangle(18, 8, 133, 8))
        {
            var one = new List<TemplateCoordinate>
            {
                new TemplateCoordinate(522, 41, 24, 25),
                new TemplateCoordinate(563, 47, 24, 25),
                new TemplateCoordinate(604, 53, 24, 25)
            };

            var two = new List<TemplateCoordinate>
            {
                new TemplateCoordinate(327, 195, 24, 25),
                new TemplateCoordinate(370, 189, 24, 25),
                new TemplateCoordinate(413, 183, 24, 25),
                new TemplateCoordinate(456, 177, 24, 25)
            };

            var three = new List<TemplateCoordinate>
            {
                new TemplateCoordinate(504, 301, 24, 25),
                new TemplateCoordinate(545, 307, 24, 25),
                new TemplateCoordinate(586, 312, 24, 25),
                new TemplateCoordinate(627, 318, 24, 25)
            };

            var four = new List<TemplateCoordinate>
            {
                new TemplateCoordinate(290, 465, 24, 25),
                new TemplateCoordinate(331, 460, 24, 25),
                new TemplateCoordinate(373, 453, 24, 25),
                new TemplateCoordinate(415, 447, 24, 25),
                new TemplateCoordinate(457, 441, 24, 25)
            };

            var five = new List<TemplateCoordinate>
            {
                new TemplateCoordinate(26, 454, 24, 25),
                new TemplateCoordinate(66, 455, 24, 25),
                new TemplateCoordinate(106, 454, 24, 25),
                new TemplateCoordinate(146, 454, 24, 25),
                new TemplateCoordinate(186, 455, 24, 25),
                new TemplateCoordinate(228, 455, 24, 25)
            };

            var rightTemplate = pakCatalog
                .GetCompositeImage(@"Images\ALL\Game\bonus_game\BG_LetterTile_Angle_Right_Up.jpg")
                .GetSubRect(new Rectangle(6, 10, 34, 32));

            var leftTemplate = pakCatalog
                .GetCompositeImage(@"Images\ALL\Game\bonus_game\BG_LetterTile_Angle_Left_Up.jpg")
                .GetSubRect(new Rectangle(10, 8, 34, 32));

            var bonusOne = new TemplateSearchArea(leftTemplate, new Rectangle(502, 33, 153, 61));
            var bonusTwo = new TemplateSearchArea(rightTemplate, new Rectangle(314, 169, 190, 69));
            var bonusThree = new TemplateSearchArea(leftTemplate, new Rectangle(481, 290, 198, 77));
            var bonusFour = new TemplateSearchArea(rightTemplate, new Rectangle(276, 437, 230, 81));
            var bonusFive = new TemplateSearchArea(waiting.Template, new Rectangle(25, 403, 240, 49));

            _letterGroups.Add(bonusOne, one);
            _letterGroups.Add(bonusTwo, two);
            _letterGroups.Add(bonusThree, three);
            _letterGroups.Add(bonusFour, four);
            _letterGroups.Add(bonusFive, five);
        }

        public string GetNextScrambledWord()
        {
            var windowContents = AutoIt.GetWindowImage(WordWhomper.WindowTitle);

            foreach (var group in _letterGroups)
            {
                // Make sure we haven't already completed this word
                var search = AutoIt.IsTemplateInWindow(windowContents.GetSubRect(group.Key.SearchArea), group.Key.Template);
                if (!search.Success)
                {
                    var letters = group.Value.Select(letter => letter.Grab(windowContents));
                    var combined = Combine(letters);
                    var text = GetText(combined).Trim().Replace(" ", String.Empty);
                    if (text.Length > group.Value.Count)
                    {
                        // TODO: Figure out a better way to match characters
                        text = text.Replace("II", "U");
                    }

                    // If we've already solved any of the bonus words,
                    // they will return white space
                    if (!String.IsNullOrWhiteSpace(text))
                    {
                        return text;
                    }
                }
            }

            return String.Empty;
        }

        private class TemplateSearchArea
        {
            private readonly Image<Bgra, byte> _template;
            private readonly Rectangle _searchArea;

            public TemplateSearchArea(Image<Bgra, byte> template, Rectangle searchArea)
            {
                _template = template;
                _searchArea = searchArea;
            }

            public Image<Bgra, byte> Template
            {
                get { return _template; }
            }

            public Rectangle SearchArea
            {
                get { return _searchArea; }
            }
        }
    }
}