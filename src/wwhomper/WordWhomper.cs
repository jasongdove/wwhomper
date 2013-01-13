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
        
        public static readonly TimeSpan ControlTimeout = TimeSpan.FromSeconds(15);

        private readonly WordList _wordList;

        private readonly MainMenu _mainMenu;
        private readonly IntroOne _introOne;
        private readonly IntroTwo _introTwo;
        private readonly IntroThree _introThree;
        private readonly Farm _farm;
        private readonly Welcome _welcome;
        private readonly InGame _inGame;
        private readonly GameSummary _gameSummary;
        private readonly NewGear _newGear;

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
            _welcome = new Welcome();
            _inGame = new InGame();
            _gameSummary = new GameSummary();
            _newGear = new NewGear();
        }

        public void Run()
        {
            var allStates = new[]
            {
                _mainMenu.Template,
                _introOne.Template,
                _introTwo.Template,
                _introThree.Template,
                _farm.Template,
                _inGame.Template,
                _welcome.Template,
                _gameSummary.Template,
                _newGear.Template
            };

            while (true)
            {
                var stateSearch = AutoIt.WaitForTemplate(WindowTitle, allStates);
                if (stateSearch.Success)
                {
                    if (stateSearch.Template == _mainMenu.Template)
                    {
                        _mainMenu.Play.Click();
                    }
                    else if (stateSearch.Template == _introOne.Template)
                    {
                        _introOne.Forward.Click();
                        AutoIt.MoveMouseOffscreen();
                    }
                    else if (stateSearch.Template == _introTwo.Template)
                    {
                        _introTwo.Forward.Click();
                    }
                    else if (stateSearch.Template == _introThree.Template)
                    {
                        _introThree.Ok.Click();
                    }
                    else if (stateSearch.Template == _farm.Template)
                    {
                        _farm.GopherHole.Click();
                    }
                    else if (stateSearch.Template == _welcome.Template)
                    {
                        _welcome.Ok.Click();
                    }
                    else if (stateSearch.Template == _inGame.Template)
                    {
                        PlayRound();
                    }
                    else if (stateSearch.Template == _gameSummary.Template)
                    {
                        _gameSummary.OkeyDokey.Click();
                    }
                    else if (stateSearch.Template == _newGear.Template)
                    {
                        _newGear.No.Click();
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
        }
    }
}