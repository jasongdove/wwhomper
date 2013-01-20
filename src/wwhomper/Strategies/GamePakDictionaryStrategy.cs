using System;
using System.Collections.Generic;
using System.Linq;
using Combinatorics.Collections;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Strategies;
using wwhomper.Data;
using wwhomper.Dictionary;
using wwhomper.Screens;

namespace wwhomper.Strategies
{
    public class GamePakDictionaryStrategy : ScreenStrategy, IScreenStrategy<InGame>
    {
        private readonly IAutoIt _autoIt;
        private readonly IPakDictionary _pakDictionary;
        private readonly PuzzleGameState _puzzleGameState;

        private readonly Random _random;

        public GamePakDictionaryStrategy(IAutoIt autoIt, IPakDictionary pakDictionary, PuzzleGameState puzzleGameState)
        {
            _autoIt = autoIt;
            _pakDictionary = pakDictionary;
            _puzzleGameState = puzzleGameState;

            _random = new Random();
        }

        public void ExecuteStrategy(InGame screen)
        {
            if (_puzzleGameState.GearWeNeed != null)
            {
                screen.Menu.Click();
                Wait(TimeSpan.FromSeconds(1));

                screen.Map.Click();
                _autoIt.WaitAfterInput();

                screen.ExitConfirm.Click();
                return;
            }

            // Get available letters
            List<char> letters = screen.GetLetters().ToList();

            // If we didn't correctly detect all letters, just give up
            if (letters.Count != 6)
            {
                return;
            }

            // Preload our guesses
            var guesses = new HashSet<string>();
            for (int len = 3; len <= 6; len++)
            {
                var variations = new Variations<char>(letters, len);
                foreach (var variation in variations)
                {
                    var guess = new String(variation.ToArray());
                    if (_pakDictionary.ContainsWord(guess))
                    {
                        guesses.Add(guess);
                    }
                }
            }

            // Not really needed, but looks neat to do them in order
            var sorted = guesses.OrderBy(x => x.Length).ThenBy(x => x);

            // Type each guess
            foreach (var guess in sorted)
            {
                // Initial enter to dismiss any tip dialog that may be up
                _autoIt.Type("{ENTER}" + guess + "{ENTER}");
                Wait(TimeSpan.FromMilliseconds(_random.Next(20, 100)));
            }
        }
    }
}