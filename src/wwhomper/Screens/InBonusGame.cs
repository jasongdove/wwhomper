using System;
using System.Collections.Generic;
using System.Linq;
using wwhomper.Screens.Controls;

namespace wwhomper.Screens
{
    public class InBonusGame : ScreenBase
    {
        private readonly List<List<TemplateCoordinate>> _letterGroups = new List<List<TemplateCoordinate>>();

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

            _letterGroups.Add(one);
            _letterGroups.Add(two);
            _letterGroups.Add(three);
            _letterGroups.Add(four);
            _letterGroups.Add(five);
        }

        public List<string> GetScrambledWords()
        {
            var result = new List<string>();

            var windowContents = AutoIt.GetWindowImage(WordWhomper.WindowTitle);

            foreach (var group in _letterGroups)
            {
                var letters = group.Select(letter => letter.Grab(windowContents));
                var combined = Combine(letters);
                var text = FixTesseract(GetText(combined));
                
                // If we've already solved any of the bonus words,
                // they will return white space
                if (!String.IsNullOrWhiteSpace(text))
                {
                    result.Add(text);
                }
            }

            return result;
        }

        private string FixTesseract(string input)
        {
            return input.Trim().Replace(" ", String.Empty);
        }
    }
}