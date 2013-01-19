using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Combinatorics.Collections;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Screens;
using wwhomper.Screens;

namespace wwhomper
{
    public class WordWhomper
    {
        private readonly IAutoIt _autoIt;

        private readonly WordList _wordList = new WordList();

        private readonly MainMenu _mainMenu;
        private readonly LocIntro _locIntro;
        private readonly LocIntroComplete _locIntroComplete;
        private readonly Farm _farm;
        private readonly InGame _inGame;
        private readonly GameSummary _gameSummary;
        private readonly NewGear _newGear;
        private readonly BonusAcorns _bonusAcorns;
        private readonly InBonusGame _inBonusGame;
        private readonly Paused _paused;
        private readonly BonusGameWaiting _bonusGameWaiting;
        private readonly BonusGameComplete _bonusGameComplete;
        private readonly SpeechBubble _speechBubble;
        private readonly InPuzzleGame _inPuzzleGame;
        private readonly PuzzleGameComplete _puzzleGameComplete;
        private readonly BlowTorch _blowTorch;

        public WordWhomper(IAutoIt autoIt, IAssetCatalog assetCatalog)
        {
            _autoIt = autoIt;

            if (!autoIt.WindowExists())
            {
                throw new InvalidOperationException("Unable to find Word Whomp Underground!");
            }

            ////var fontLoader = new FontLoader(gameRoot);
            ////var speechBubbleFont = fontLoader.LoadFont("DomCasualStd105WItaALLCAPS13");

            ////var pakCatalog = new PakCatalog(Path.Combine(gameRoot, "images.pak"));
            ////pakCatalog.Load();

            _wordList.LoadFromDictionary(assetCatalog.GetEntryText(@"Dictionary\us-uk-fr\dictionary.txt"));

            _mainMenu = new MainMenu(autoIt, assetCatalog);
            _locIntro = new LocIntro(autoIt, assetCatalog);
            _locIntroComplete = new LocIntroComplete(autoIt, assetCatalog);
            _farm = new Farm(autoIt, assetCatalog);
            _inGame = new InGame(autoIt, assetCatalog);
            _gameSummary = new GameSummary(autoIt, assetCatalog);
            _paused = new Paused(autoIt, assetCatalog);
            _speechBubble = new SpeechBubble(autoIt, assetCatalog);
            _bonusGameWaiting = new BonusGameWaiting(autoIt, assetCatalog);
            _inBonusGame = new InBonusGame(autoIt, assetCatalog, _bonusGameWaiting);
            _inPuzzleGame = new InPuzzleGame(autoIt, assetCatalog);
            _puzzleGameComplete = new PuzzleGameComplete(autoIt, assetCatalog);
            _newGear = new NewGear(autoIt, assetCatalog);
            _bonusAcorns = new BonusAcorns(autoIt, assetCatalog);
            _bonusGameComplete = new BonusGameComplete(autoIt, assetCatalog);
            _blowTorch = new BlowTorch(autoIt, assetCatalog);
        }

        public void Run()
        {
            var allScreens = new GameScreen[]
            {
                _paused,
                _speechBubble, // This needs to be before "_inGame"
                _inGame,
                _gameSummary,
                _farm,
                _bonusGameWaiting,
                _inBonusGame,
                _inPuzzleGame,
                _locIntro,
                _locIntroComplete,
                _mainMenu,
                _bonusGameComplete
            };

            do
            {
                ScreenSearchResult state = _autoIt.WaitForScreen(allScreens);
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
                        _autoIt.MoveMouseOffscreen();
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
                        _autoIt.Type("{ENTER}");
                        Thread.Sleep(200);
                        _autoIt.Type("{ENTER}");
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
                        state = _autoIt.WaitForScreen(_newGear, _bonusAcorns, _puzzleGameComplete, _blowTorch);
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
                            else if (state.Screen == _blowTorch)
                            {
                                _blowTorch.Ok.Click();
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
                    if (!_autoIt.IsWindowActive())
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
                _autoIt.Type("{ENTER}" + guess + "{ENTER}");
                Thread.Sleep(random.Next(20, 100));
            }

            // Give the summary time to appear
            Thread.Sleep(2800);
        }

        private void PlayBonusGame()
        {
            var random = new Random();

            // Clear out any bad data
            for (int i = 0; i < 6; i++)
            {
                _autoIt.Type("{BACKSPACE}");
            }

            // Mix up the current word to give tesseract some variety
            _autoIt.Type("{SPACE}");

            string scrambled = _inBonusGame.GetNextScrambledWord();
            if (String.IsNullOrEmpty(scrambled))
            {
                Console.WriteLine("Nothing to do in the bonus game");
                return;
            }

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
                _autoIt.Type(guesses.ToList()[random.Next(guesses.Count)]);
                Thread.Sleep(random.Next(20, 100));
            }
        }

        private void PlayPuzzleGame()
        {
            _inPuzzleGame.ClearAllGears();

            var windowContents = _autoIt.GetWindowImage();

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