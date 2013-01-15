using System;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class InBonusGame : ScreenBase
    {
        private readonly Dictionary<Image<Gray, byte>, List<TemplateCoordinate>> _letterGroups =
            new Dictionary<Image<Gray, byte>, List<TemplateCoordinate>>();

        public InBonusGame()
            : base("InBonusGame.png")
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

            var bonusOne = TemplateLoader.LoadTemplate("BonusOne.png");
            var bonusTwo = TemplateLoader.LoadTemplate("BonusTwo.png");
            var bonusThree = TemplateLoader.LoadTemplate("BonusThree.png");
            var bonusFour = TemplateLoader.LoadTemplate("BonusFour.png");
            var bonusFive = TemplateLoader.LoadTemplate("BonusGameWaiting.png");

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
                var search = AutoIt.IsTemplateInWindow(windowContents, group.Key);
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
    }
}