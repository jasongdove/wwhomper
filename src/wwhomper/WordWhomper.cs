using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Combinatorics.Collections;
using wwhomper.Screens;

namespace wwhomper
{
    public class WordWhomper
    {
        public const string WindowTitle = "[REGEXPTITLE:^Word Whomp Underground]";
        
        public static readonly TimeSpan ControlTimeout = TimeSpan.FromSeconds(15);

        private readonly WordList _wordList = new WordList();

        private readonly MainMenu _mainMenu = new MainMenu();
        private readonly IntroOne _introOne = new IntroOne();
        private readonly IntroTwo _introTwo = new IntroTwo();
        private readonly IntroThree _introThree = new IntroThree();
        private readonly Farm _farm = new Farm();
        private readonly Welcome _welcome = new Welcome();
        private readonly InGame _inGame = new InGame();
        private readonly GameSummary _gameSummary = new GameSummary();
        private readonly NewGear _newGear = new NewGear();
        private readonly BonusAcorns _bonusAcorns = new BonusAcorns();
        private readonly InBonusGame _inBonusGame = new InBonusGame();
        private readonly Paused _paused = new Paused();
        private readonly BonusGameWaiting _bonusGameWaiting = new BonusGameWaiting();
        private readonly BonusGameComplete _bonusGameComplete = new BonusGameComplete();

        public WordWhomper()
        {
            if (!AutoIt.WindowExists(WindowTitle))
            {
                throw new InvalidOperationException("Unable to find Word Whomp Underground!");
            }

            _wordList.Load("wordsEn.txt");
        }

        public void Run()
        {
            var allScreens = new ScreenBase[]
            {
                _paused,
                _mainMenu,
                _introOne,
                _introTwo,
                _introThree,
                _farm,
                _gameSummary,
                _inGame,
                _welcome,
                _newGear,
                _bonusAcorns,
                _inBonusGame,
                _bonusGameComplete, // This needs to be before "_bonusGameWaiting"
                _bonusGameWaiting
            };

            TemplateSearchResult state;
            do
            {
                state = AutoIt.WaitForScreen(WindowTitle, allScreens);
                if (state.Success)
                {
                    if (state.Template == _paused.Template)
                    {
                        _paused.Ok.Click();
                    }
                    else if (state.Template == _mainMenu.Template)
                    {
                        _mainMenu.Play.Click();
                    }
                    else if (state.Template == _introOne.Template)
                    {
                        _introOne.Forward.Click();
                        AutoIt.MoveMouseOffscreen();
                    }
                    else if (state.Template == _introTwo.Template)
                    {
                        _introTwo.Forward.Click();
                    }
                    else if (state.Template == _introThree.Template)
                    {
                        _introThree.Ok.Click();
                    }
                    else if (state.Template == _farm.Template)
                    {
                        _farm.GopherHole.Click();
                    }
                    else if (state.Template == _welcome.Template)
                    {
                        _welcome.Ok.Click();
                    }
                    else if (state.Template == _inGame.Template)
                    {
                        PlayRound();
                    }
                    else if (state.Template == _gameSummary.Template)
                    {
                        _gameSummary.OkeyDokey.Click();
                    }
                    else if (state.Template == _newGear.Template)
                    {
                        _newGear.No.Click();
                    }
                    else if (state.Template == _bonusAcorns.Template)
                    {
                        _bonusAcorns.Ok.Click();
                        // There is a transition here that takes a while
                        Thread.Sleep(3000);
                    }
                    else if (state.Template == _inBonusGame.Template)
                    {
                        PlayBonusRound();
                    }
                    else if (state.Template == _bonusGameWaiting.Template)
                    {
                        // Just wait for the acorns to roll down
                        Thread.Sleep(2000);
                    }
                    else if (state.Template == _bonusGameComplete.Template)
                    {
                        _bonusGameComplete.Ok.Click();
                    }
                    else
                    {
                        // Unknown screen
                        break;
                    }
                }

            } while (state.Success);
        }

        private void PlayRound()
        {
            var random = new Random();

            // Don't want the mouse to be in any of the screenshots we use
            AutoIt.MoveMouseOffscreen();

            // Get available letters
            List<char> letters = _inGame.GetLetters().ToList();

            // Preload our guesses
            var guesses = new HashSet<string>();
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
                AutoIt.Type(WindowTitle, guess + "{ENTER}")
            ;
                Thread.Sleep(random.Next(20, 100));
            }
        }

        private void PlayBonusRound()
        {
            var random = new Random();

            // Don't want the mouse to be in any of the screenshots we use
            AutoIt.MoveMouseOffscreen();

            // Clear out any bad data
            for (int i = 0; i < 22; i++)
            {
                AutoIt.Type(WindowTitle, "{BACKSPACE}");
            }

            // Mix up the current word to give tesseract some variety
            AutoIt.Type(WindowTitle, "{SPACE}");

            // Get scrambled words
            List<string> scrambled = _inBonusGame.GetScrambledWords();
            foreach (var word in scrambled)
            {
                var variations = new Variations<char>(word.ToArray(), word.Length, GenerateOption.WithoutRepetition);
                foreach (var variation in variations)
                {
                    var guess = new String(variation.ToArray());
                    if (_wordList.ContainsWord(guess))
                    {
                        AutoIt.Type(WindowTitle, guess);
                        Thread.Sleep(random.Next(20, 100));
                        break;
                    }
                }
            }
        }
    }
}