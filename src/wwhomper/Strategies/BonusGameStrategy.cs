using System;
using System.Collections.Generic;
using System.Linq;
using Combinatorics.Collections;
using Ninject.Extensions.Logging;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Strategies;
using wwhomper.Dictionary;
using wwhomper.Screens;

namespace wwhomper.Strategies
{
    public class BonusGameStrategy : ScreenStrategy, IScreenStrategy<InBonusGame>
    {
        private readonly IAutoIt _autoIt;
        private readonly IPakDictionary _pakDictionary;
        private readonly ILogger _logger;
        private Guess _lastGuess;

        private readonly Random _random;

        public BonusGameStrategy(IAutoIt autoIt, IPakDictionary pakDictionary, ILogger logger)
        {
            _autoIt = autoIt;
            _pakDictionary = pakDictionary;
            _logger = logger;

            _random = new Random();
        }

        public void ExecuteStrategy(InBonusGame screen)
        {
            // Clear out our last guess if it was a while ago
            if (_lastGuess != null && (DateTime.Now - _lastGuess.Time) > TimeSpan.FromSeconds(10))
            {
                _lastGuess = null;
            }

            // Clear out any bad data
            for (int i = 0; i < 6; i++)
            {
                _autoIt.Type("{BACKSPACE}");
            }

            // Mix up the current word to give tesseract some variety
            _autoIt.Type("{SPACE}");

            string scrambled = screen.GetNextScrambledWord();
            if (String.IsNullOrEmpty(scrambled))
            {
                _logger.Debug("Nothing to do in the bonus game");
                return;
            }

            // If our last guess had an R (but didn't work) let's try A instead
            var currentLetters = scrambled.ToArray().OrderBy(x => x);
            if (_lastGuess != null &&
                currentLetters.Contains('R') &&
                _lastGuess.Letters.SequenceEqual(currentLetters))
            {
                scrambled = scrambled.Remove(scrambled.IndexOf('R'));
                scrambled += 'A';
            }
            else
            {
                _lastGuess = new Guess { Time = DateTime.Now, Letters = scrambled.ToArray().OrderBy(x => x) };
            }

            var guesses = new HashSet<string>();
            var variations = new Variations<char>(scrambled.ToArray(), scrambled.Length);
            foreach (var variation in variations)
            {
                var guess = new String(variation.ToArray());
                if (_pakDictionary.ContainsWord(guess))
                {
                    guesses.Add(guess);
                }
            }

            // If tesseract didn't get the letters right, we may not have any words to try this cycle
            if (guesses.Any())
            {
                // Type a random guess from the list
                _autoIt.Type(guesses.ToList()[_random.Next(guesses.Count)]);
                Wait(TimeSpan.FromMilliseconds(_random.Next(20, 100)));
            }
        }

        private class Guess
        {
            public DateTime Time { get; set; }
            public IOrderedEnumerable<char> Letters { get; set; }
        }
    }
}