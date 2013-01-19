using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Ninject.Extensions.Logging;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Strategies;
using wwhomper.Data;
using wwhomper.Dictionary;
using wwhomper.Screens;

namespace wwhomper.Strategies
{
    public class PuzzleGameStrategy : ScreenStrategy, IScreenStrategy<InPuzzleGame>
    {
        private readonly IAutoIt _autoIt;
        private readonly IPakDictionary _pakDictionary;
        private readonly ILogger _logger;

        public PuzzleGameStrategy(IAutoIt autoIt, IPakDictionary pakDictionary, ILogger logger)
        {
            _autoIt = autoIt;
            _pakDictionary = pakDictionary;
            _logger = logger;
        }

        public void ExecuteStrategy(InPuzzleGame screen)
        {
            screen.ClearAllGears();

            _autoIt.MoveMouseOffscreen();
            var windowContents = _autoIt.GetWindowImage();

            // Determine how many letters we need
            var gearSpaces = screen.GetRequiredGears(windowContents);

            _logger.Debug("We need {0} letters", gearSpaces.Count);

            // Determine which letters we have
            var letters = screen.GetAvailableLetters(windowContents);
            if (letters.Count < gearSpaces.Count)
            {
                _logger.Debug("Insufficient letters, will try again later...");
                screen.Back.Click();
                return;
            }

            _logger.Debug("Letters we have: {0}", String.Join(String.Empty, letters.Select(x => x.Letter).ToArray()));

            var words = _pakDictionary.OfLength(gearSpaces.Count);
            var guess = new List<PuzzleLetter>();
            foreach (var word in words)
            {
                guess.Clear();

                var guessLetters = new List<PuzzleLetter>(letters);
                var wildcards = letters.Where(x => x.Letter == "*").ToList();

                for (int i = 0; i < gearSpaces.Count; i++)
                {
                    var guessLetter = guessLetters.FirstOrDefault(x => x.Letter == word[i].ToString(CultureInfo.InvariantCulture) && x.Size == gearSpaces[i]);
                    if (guessLetter != null)
                    {
                        guessLetters.Remove(guessLetter);
                        guess.Add(guessLetter);
                    }
                    else
                    {
                        if (wildcards.Any())
                        {
                            guess.Add(wildcards[0]);
                            wildcards.RemoveAt(0);
                        }
                        else
                        {
                            guess.Clear();
                            break;
                        }
                    }
                }

                if (String.Join(String.Empty, guess.Select(x => x.Letter)) == word)
                {
                    _logger.Debug("Trying {0}", word);
                    break;
                }
            }

            if (!guess.Any())
            {
                _logger.Debug("Unable to create a word with these letters, will try again later...");
                screen.Back.Click();
                return;
            }

            _logger.Debug("We should guess: {0}", String.Join(String.Empty, guess.Select(x => x.Letter)));

            screen.SubmitWord(guess);

            // Give it a chance to work or not
            Wait(TimeSpan.FromSeconds(7));
        }
    }
}