using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Combinatorics.Collections;
using wwhomper.Screens;

namespace wwhomper
{
    public class WordWhomper
    {
        public const string WindowTitle = "[REGEXPTITLE:^Word Whomp Underground]";
        
        public static readonly TimeSpan ControlTimeout = TimeSpan.FromSeconds(10);

        private readonly WordList _wordList;

        private readonly MainMenu _mainMenu;
        private readonly IntroOne _introOne;
        private readonly IntroTwo _introTwo;
        private readonly IntroThree _introThree;
        private readonly Farm _farm;
        private readonly InGame _inGame;
        private readonly GameSummary _gameSummary;

        public WordWhomper()
        {
            if (!AutoIt.WindowExists(WindowTitle))
            {
                throw new InvalidOperationException("Unable to find Word Whomp Underground!");
            }

            _wordList = new WordList();
            _wordList.Load("wordsEn.txt");

            _mainMenu = new MainMenu();
            _introOne = new IntroOne();
            _introTwo = new IntroTwo();
            _introThree = new IntroThree();
            _farm = new Farm();
            _inGame = new InGame();
            _gameSummary = new GameSummary();
        }

        public void Run()
        {
            if (AutoIt.IsScreenActive(WindowTitle, _mainMenu))
            {
                PlayGame();
            }
        }

        private void PlayGame()
        {
            _mainMenu.Play.Click();

            var search = AutoIt.WaitForTemplate(WindowTitle, _farm.Template, _introOne.Template);
            if (!search.Success)
            {
                // Unexpected screen
                return;
            }

            if (search.Template == _introOne.Template)
            {
                _introOne.Forward.Click();
                
                // Move the mouse offscreen so it doesn't affect our
                // ability to find the button again (hover highlight)
                AutoIt.MoveMouseOffscreen();

                _introTwo.WaitUntilLoaded();
                _introTwo.Forward.Click();

                AutoIt.MoveMouseOffscreen();

                _introThree.WaitUntilLoaded();
                _introThree.Ok.Click();

                _farm.WaitUntilLoaded();
            }

            // Click whichever level we're on
            _farm.GopherHole.Click();

            while (true)
            {
                // TODO: Check for bonus game or new gear or summary
                var gameScreenSearch = AutoIt.WaitForTemplate(WindowTitle, _inGame.Template);
                if (gameScreenSearch.Success)
                {
                    // Play the game
                    if (gameScreenSearch.Template == _inGame.Template)
                    {
                        PlayRound();
                    }
                    else
                    {
                        // Unknown screen
                        break;
                    }
                }
                else
                {
                    // Unknown screen
                    break;
                }
            }
        }

        private void PlayRound()
        {
            var random = new Random();

            // Don't want the mouse to be in any of the screenshots we use
            AutoIt.MoveMouseOffscreen();

            // Get available letters
            List<char> letters = _inGame.GetLetters().ToList();

            // Preload our guesses
            var guesses = new List<string>();
            for (int len = 3; len <= 6; len++)
            {
                var variations = new Variations<char>(letters, len, GenerateOption.WithoutRepetition);
                foreach (var variation in variations)
                {
                    var guess = new String(variation.ToArray());
                    if (_wordList.ContainsWord(guess))
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
                AutoIt.Type(WindowTitle, guess + Environment.NewLine);
                Thread.Sleep(random.Next(20, 100));
            }

            // Dismiss the scoreboard
            var wait = _gameSummary.WaitUntilLoaded();
            if (wait.Success)
            {
                _gameSummary.OkeyDokey.Click();
            }
        }
    }
}