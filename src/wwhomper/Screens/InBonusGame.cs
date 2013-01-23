using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using Ninject.Extensions.Logging;
using sharperbot;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class InBonusGame : TemplateScreen
    {
        private readonly List<KeyValuePair<TemplateSearchArea, List<TemplateCoordinate>>> _letterGroups =
            new List<KeyValuePair<TemplateSearchArea, List<TemplateCoordinate>>>();

        public InBonusGame(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\ALL\Game\bonus_game\BG_Background.jpg",
                266, 372, 74, 38,
                256, 363, 95, 56)
        {
            var one = new List<TemplateCoordinate>
            {
                new TemplateCoordinate(520, 17, 23, 24),
                new TemplateCoordinate(562, 23, 23, 24),
                new TemplateCoordinate(602, 29, 23, 24)
            };

            var two = new List<TemplateCoordinate>
            {
                new TemplateCoordinate(328, 170, 23, 24),
                new TemplateCoordinate(367, 164, 23, 24),
                new TemplateCoordinate(414, 158, 23, 24),
                new TemplateCoordinate(457, 152, 23, 24)
            };

            var three = new List<TemplateCoordinate>
            {
                new TemplateCoordinate(500, 277, 23, 24),
                new TemplateCoordinate(543, 283, 23, 24),
                new TemplateCoordinate(581, 288, 23, 24), // TODO: double check this area
                new TemplateCoordinate(623, 295, 23, 24)
            };

            var four = new List<TemplateCoordinate>
            {
                new TemplateCoordinate(288, 440, 23, 24),
                new TemplateCoordinate(330, 434, 23, 24),
                new TemplateCoordinate(372, 428, 23, 24),
                new TemplateCoordinate(414, 422, 23, 24),
                new TemplateCoordinate(457, 416, 23, 24),
            };

            var five = new List<TemplateCoordinate>
            {
                new TemplateCoordinate(24, 430, 23, 24),
                new TemplateCoordinate(65, 430, 23, 24),
                new TemplateCoordinate(106, 430, 23, 24),
                new TemplateCoordinate(145, 430, 23, 24),
                new TemplateCoordinate(187, 430, 23, 24),
                new TemplateCoordinate(227, 430, 23, 24)
            };

            var rightTemplate = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\bonus_game\BG_LetterTile_Angle_Right_Up.jpg")
                .Copy(new Rectangle(6, 10, 34, 32));

            var leftTemplate = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\bonus_game\BG_LetterTile_Angle_Left_Up.jpg")
                .Copy(new Rectangle(10, 8, 34, 32));

            var straightTemplate = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\bonus_game\BG_LetterTile_Angle_Straight_Up.jpg")
                .Copy(new Rectangle(7, 8, 34, 34));

            var bonusOne = new TemplateSearchArea(leftTemplate, new Rectangle(499, 8, 153, 61));
            var bonusTwo = new TemplateSearchArea(rightTemplate, new Rectangle(311, 144, 190, 69));
            var bonusThree = new TemplateSearchArea(leftTemplate, new Rectangle(478, 265, 198, 77));
            var bonusFour = new TemplateSearchArea(rightTemplate, new Rectangle(273, 412, 230, 81));
            var bonusFive = new TemplateSearchArea(straightTemplate, new Rectangle(10, 419, 252, 45));

            _letterGroups.Add(new KeyValuePair<TemplateSearchArea,List<TemplateCoordinate>>(bonusOne, one));
            _letterGroups.Add(new KeyValuePair<TemplateSearchArea,List<TemplateCoordinate>>(bonusTwo, two));
            _letterGroups.Add(new KeyValuePair<TemplateSearchArea,List<TemplateCoordinate>>(bonusThree, three));
            _letterGroups.Add(new KeyValuePair<TemplateSearchArea,List<TemplateCoordinate>>(bonusFour, four));
            _letterGroups.Add(new KeyValuePair<TemplateSearchArea,List<TemplateCoordinate>>(bonusFive, five));
        }

        public string GetNextScrambledWord()
        {
            var windowContents = AutoIt.GetWindowImage();

            for (int i = 0; i < _letterGroups.Count; i++)
            {
                var group = _letterGroups[i];

                // Make sure we haven't already completed this word
                windowContents.ROI = group.Key.SearchArea;
                var search = AutoIt.IsTemplateInWindow(windowContents, group.Key.Template);
                if (!search.Success)
                {
                    Logger.Debug("Bonus group is unsolved - group={0}", i);

                    windowContents.ROI = Rectangle.Empty;

                    var letters = group.Value.Select(letter => letter.Grab(windowContents));
                    var combined = Combine(letters);

                    // These letters are fairly bright, so let's raise the floor on intensity.
                    // Anything under 245 goes down to 0
                    var gray = combined.Convert<Gray, byte>();
                    gray.Floor(245);

                    var text = GetZoomedOutText(gray, 2).Trim().Replace(" ", String.Empty);
                    if (text.Length > group.Value.Count)
                    {
                        var input = text;

                        // TODO: Figure out a better way to match characters
                        text = text.Replace("II", "U");

                        Logger.Debug(
                            "Fixed tesseract errors - in={0}, out={1}",
                            input.Replace("\r\n", @"\r\n").Replace("\n", @"\n"),
                            text);
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