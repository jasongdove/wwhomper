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
        private readonly InBonusGame _inBonusGame;
        private readonly Paused _paused;
        private readonly BonusGameWaiting _bonusGameWaiting;
        private readonly BonusGameComplete _bonusGameComplete = new BonusGameComplete();
        private readonly SpeechBubble _speechBubble;
        private readonly InPuzzleGame _inPuzzleGame;
        private readonly PuzzleGameComplete _puzzleGameComplete;

        public WordWhomper(string gameRoot)
        {
            if (!AutoIt.WindowExists(WindowTitle))
            {
                throw new InvalidOperationException("Unable to find Word Whomp Underground!");
            }

            ////var fontLoader = new FontLoader(gameRoot);
            ////var speechBubbleFont = fontLoader.LoadFont("DomCasualStd105WItaALLCAPS13");

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
            _bonusGameWaiting = new BonusGameWaiting(pakCatalog);
            _inBonusGame = new InBonusGame(pakCatalog, _bonusGameWaiting);
            _inPuzzleGame = new InPuzzleGame(pakCatalog);
            _puzzleGameComplete = new PuzzleGameComplete(pakCatalog);
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
                _bonusGameWaiting,
                _inPuzzleGame,
                _locIntro,
                _locIntroComplete,
                _mainMenu,
                _bonusGameComplete
            };

            do
            {
                ScreenSearchResult state = AutoIt.WaitForScreen(WindowTitle, allScreens);
                if (state.Success)
                {
                    if (state.Screen == _paused)
                    {
                        _paused.Ok.Click();
                    }
                    else if (state.Screen == _mainMenu)
                    {
                        _mainMenu.Play.Click();
                        WaitForTransition();
                    }
                    else if (state.Screen == _locIntro)
                    {
                        _locIntro.Forward.Click();
                        AutoIt.MoveMouseOffscreen();
                    }
                    else if (state.Screen == _locIntroComplete)
                    {
                        _locIntroComplete.Ok.Click();
                    }
                    else if (state.Screen == _farm)
                    {
                        _farm.GopherHole.Click();
                    }
                    else if (state.Screen == _inGame)
                    {
                        PlayGame();
                    }
                    else if (state.Screen == _gameSummary)
                    {
                        AutoIt.Type(WindowTitle, "{ENTER}");
                        Thread.Sleep(200);
                        AutoIt.Type(WindowTitle, "{ENTER}");
                    }
                    else if (state.Screen == _inBonusGame)
                    {
                        PlayBonusGame();
                    }
                    else if (state.Screen == _bonusGameWaiting)
                    {
                        // Just wait for the acorns to roll down
                        Thread.Sleep(2000);
                    }
                    else if (state.Screen == _bonusGameComplete)
                    {
                        _bonusGameComplete.Ok.Click();
                        WaitForTransition();
                    }
                    else if (state.Screen == _speechBubble)
                    {
                        // Detail check for different types of speech bubbles
                        state = AutoIt.WaitForScreen(WindowTitle, _newGear, _bonusAcorns, _puzzleGameComplete);
                        if (state.Success)
                        {
                            if (state.Screen == _newGear)
                            {
                                _newGear.Yes.Click();
                                WaitForTransition();
                            }
                            else if (state.Screen == _bonusAcorns)
                            {
                                _bonusAcorns.Ok.Click();
                                WaitForTransition();
                            }
                            else if (state.Screen == _puzzleGameComplete)
                            {
                                _puzzleGameComplete.Ok.Click();
                                WaitForTransition();
                            }
                        }
                    }
                    else if (state.Screen == _inPuzzleGame)
                    {
                        PlayPuzzleGame();
                    }
                }
                else
                {
                    Console.WriteLine("No screen detected.");
                    if (!AutoIt.IsWindowActive(WindowTitle))
                    {
                        Thread.Sleep(3000);
                    }
                }

            } while (true);
        }

        private void PlayGame()
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
            Thread.Sleep(1500);
        }

        private void PlayBonusGame()
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

        private void PlayPuzzleGame()
        {
            _inPuzzleGame.ClearAllGears();

            var windowContents = AutoIt.GetWindowImage(WindowTitle);

            // Determine how many letters we need
            int requiredLetterCount = _inPuzzleGame.GetRequiredLetterCount(windowContents);

            Console.WriteLine("We need {0} letters", requiredLetterCount);

            // Determine which letters we have
            string letters = _inPuzzleGame.GetAvailableLetters(windowContents);
            if (letters == null)
            {
                Console.WriteLine("Unable to create a word with these letters, will try again later...");
                _inPuzzleGame.Back.Click();
                WaitForTransition();
                return;
            }

            Console.WriteLine("Letters we have: {0}", letters);

            string guess = null;
            if (!letters.Contains('*'))
            {
                var variations = new Variations<char>(letters.ToArray(), requiredLetterCount);
                var words = variations.Select(x => new String(x.ToArray()));
                guess = words.FirstOrDefault(x => _wordList.ContainsWord(x));
            }
            else
            {
                const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                for (int i = 0; i < 26; i++)
                {
                    var wildcardLetters = letters.ToArray().ToList();
                    wildcardLetters.Add(alphabet[i]);
                    var variations = new Variations<char>(wildcardLetters, requiredLetterCount);
                    var words = variations.Select(x => new String(x.ToArray()));
                    var wildcardGuess = words.FirstOrDefault(x => _wordList.ContainsWord(x));
                    if (wildcardGuess != null)
                    {
                        guess = wildcardGuess;
                        break;
                    }
                }
            }

            if (guess == null)
            {
                Console.WriteLine("Unable to create a word with these letters, will try again later...");
                _inPuzzleGame.Back.Click();
                WaitForTransition();
                return;
            }

            Console.WriteLine("We should guess: {0}", guess);

            // Replace wildcards
            for (int i=0; i<guess.Length; i++)
            {
                if (!letters.Contains(guess[i]))
                {
                    guess = guess.Remove(i, 1);
                    guess = guess.Insert(i, "*");
                }
            }

            _inPuzzleGame.SubmitWord(windowContents, guess);

            // Give it a chance to work or not
            Thread.Sleep(7000);
        }

        private void WaitForTransition()
        {
            Thread.Sleep(5000);
        }
    }
}