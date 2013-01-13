using System;
using wwhomper.Screens;

namespace wwhomper
{
    public class WordWhomper
    {
        public const string WindowTitle = "[REGEXPTITLE:^Word Whomp Underground]";
        
        public static readonly TimeSpan ControlTimeout = TimeSpan.FromSeconds(10);

        private readonly MainMenu _mainMenu;
        private readonly Farm _farm;
        private readonly LearnHowToPlay _learnHowToPlay;
        private readonly InGame _inGame;

        public WordWhomper()
        {
            if (!AutoIt.WindowExists(WindowTitle))
            {
                throw new InvalidOperationException("Unable to find Word Whomp Underground!");
            }

            _mainMenu = new MainMenu();
            _farm = new Farm();
            _learnHowToPlay = new LearnHowToPlay();
            _inGame = new InGame();
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

            // Click whichever level we're on
            _farm.WaitUntilLoaded();
            _farm.GopherHole.Click();

            var gameScreenSearch = AutoIt.WaitForTemplate(WindowTitle, _learnHowToPlay.Template, _inGame.Template);
            if (gameScreenSearch.Success)
            {
                // Dismiss the "learn how to play" dialog if it shows up
                if (gameScreenSearch.Template == _learnHowToPlay.Template)
                {
                    _learnHowToPlay.No.Click();
                }
                // Play the game
                else if (gameScreenSearch.Template == _inGame.Template)
                {
                    PlayRound();
                }
            }
        }

        private void PlayRound()
        {
            // TODO: Win the round
            var letters = _inGame.GetLetters();
            Console.WriteLine(letters);
        }
    }
}