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

        public WordWhomper()
        {
            if (!AutoIt.WindowExists(WindowTitle))
            {
                throw new InvalidOperationException("Unable to find Word Whomp Underground!");
            }

            _mainMenu = new MainMenu();
            _farm = new Farm();
            _learnHowToPlay = new LearnHowToPlay();
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

            // Dismiss the "learn how to play" dialog if it shows up
            var learnHowToPlaySearch = _learnHowToPlay.WaitUntilLoaded();
            if (learnHowToPlaySearch.Success)
            {
                _learnHowToPlay.No.Click();
            }

            PlayRound();
        }

        private void PlayRound()
        {
        }
    }
}