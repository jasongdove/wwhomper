using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Combinatorics.Collections;
using wwhomper.Pak;
using wwhomper.Screens;

namespace wwhomper
{
    public class WordWhomper
    {
        public const string WindowTitle = "[REGEXPTITLE:^Word Whomp Underground -]";
        
        public static readonly TimeSpan ControlTimeout = TimeSpan.FromSeconds(15);

        private readonly WordList _wordList = new WordList();

        private readonly MainMenu _mainMenu;
        private readonly LocIntro _locIntro;
        private readonly LocIntroComplete _locIntroComplete;
        private readonly Farm _farm;
        private readonly InGame _inGame;
        private readonly GameSummary _gameSummary;
        private readonly NewGear _newGear = new NewGear();
        private readonly BonusAcorns _bonusAcorns = new BonusAcorns();
        private readonly InBonusGame _inBonusGame = new InBonusGame();
        private readonly Paused _paused;
        private readonly BonusGameWaiting _bonusGameWaiting = new BonusGameWaiting();
        private readonly BonusGameComplete _bonusGameComplete = new BonusGameComplete();
        private readonly SpeechBubble _speechBubble;

        public WordWhomper(string gameRoot)
        {
            if (!AutoIt.WindowExists(WindowTitle))
            {
                throw new InvalidOperationException("Unable to find Word Whomp Underground!");
            }

            var pakCatalog = new PakCatalog(Path.Combine(gameRoot, "images.pak"));
            pakCatalog.Load();

            _wordList.LoadFromDictionary(pakCatalog.GetEntryText(@"Dictionary\us-uk-fr\dictionary.txt"));

            _mainMenu = new MainMenu(pakCatalog);
            _locIntro = new LocIntro(pakCatalog);
            _locIntroComplete = new LocIntroComplete(pakCatalog);
            _farm = new Farm(pakCatalog);
            _inGame = new InGame(pakCatalog);
            _gameSummary = new GameSummary(pakCatalog);
            _paused = new Paused(pakCatalog);
            _speechBubble = new SpeechBubble(pakCatalog);
        }

        public void Run()
        {
            var allScreens = new ScreenBase[]
            {
                _paused,
                _speechBubble, // This needs to be before "_inGame"
                _inGame,
                _gameSummary,
                _farm,
                _inBonusGame,
                _bonusGameComplete, // This needs to be before "_bonusGameWaiting"
                _bonusGameWaiting,
                _locIntro,
                _locIntroComplete,
                _mainMenu
            };

            do
            {
                TemplateSearchResult state = AutoIt.WaitForScreen(WindowTitle, allScreens);
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
                    else if (state.Template == _locIntro.Template)
                    {
                        _locIntro.Forward.Click();
                        AutoIt.MoveMouseOffscreen();
                    }
                    else if (state.Template == _locIntroComplete.Template)
                    {
                        _locIntroComplete.Ok.Click();
                    }
                    else if (state.Template == _farm.Template)
                    {
                        _farm.GopherHole.Click();
                    }
                    else if (state.Template == _inGame.Template)
                    {
                        PlayRound();
                    }
                    else if (state.Template == _gameSummary.Template)
                    {
                        AutoIt.Type(WindowTitle, "{ENTER}");
                        Thread.Sleep(200);
                        AutoIt.Type(WindowTitle, "{ENTER}");
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
                    else if (state.Template == _speechBubble.Template)
                    {
                        // Detail check for different types of speech bubbles
                        state = AutoIt.WaitForScreen(WindowTitle, _newGear, _bonusAcorns);
                        if (state.Success)
                        {
                            if (state.Template == _newGear.Template)
                            {
                                _newGear.No.Click();
                            }
                            else if (state.Template == _bonusAcorns.Template)
                            {
                                _bonusAcorns.Ok.Click();
                                // There is a transition here that takes a while
                                Thread.Sleep(3000);
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No screen detected, sleeping...");
                    Thread.Sleep(3000);
                }

            } while (true);
        }

        private void PlayRound()
        {
            var random = new Random();

            // Get available letters
            List<char> letters = _inGame.GetLetters().ToList();

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
                // Initial enter to dismiss any tip dialog that may be up
                AutoIt.Type(WindowTitle, "{ENTER}" + guess + "{ENTER}");
                Thread.Sleep(random.Next(20, 100));
            }

            // Give the summary time to appear
            Thread.Sleep(3000);
        }

        private void PlayBonusRound()
        {
            var random = new Random();

            // Clear out any bad data
            for (int i = 0; i < 6; i++)
            {
                AutoIt.Type(WindowTitle, "{BACKSPACE}");
            }

            // Mix up the current word to give tesseract some variety
            AutoIt.Type(WindowTitle, "{SPACE}");

            string scrambled = _inBonusGame.GetNextScrambledWord();

            var guesses = new HashSet<string>();
            var variations = new Variations<char>(scrambled.ToArray(), scrambled.Length);
            foreach (var variation in variations)
            {
                var guess = new String(variation.ToArray());
                if (_wordList.ContainsWord(guess))
                {
                    guesses.Add(guess);
                }
            }

            // If tesseract didn't get the letters right, we may not have any words to try this cycle
            if (guesses.Any())
            {
                // Type a random guess from the list
                AutoIt.Type(WindowTitle, guesses.ToList()[random.Next(guesses.Count)]);
                Thread.Sleep(random.Next(20, 100));
            }
        }
    }
}