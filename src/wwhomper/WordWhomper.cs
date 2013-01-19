﻿using System;
using Ninject;
using Ninject.Extensions.Logging;
using sharperbot;
using sharperbot.AutoIt;
using sharperbot.Strategies;
using wwhomper.Screens;
using wwhomper.Strategies;

namespace wwhomper
{
    public class WordWhomper : Bot
    {
        public WordWhomper(IKernel kernel, ILogger logger, IAutoIt autoIt)
            : base(kernel, logger, autoIt)
        {
            if (!autoIt.WindowExists())
            {
                throw new InvalidOperationException("Unable to find Word Whomp Underground!");
            }

            RegisterScreen<MainMenu, AcceptDialogWithTransitionStrategy>();
            RegisterScreen<Intro, AcceptDialogWithoutWaitingStrategy>();
            RegisterScreen<IntroComplete, AcceptDialogWithoutWaitingStrategy>();
            RegisterScreen<Map, AcceptDialogWithoutWaitingStrategy>();
            RegisterScreen<Paused, AcceptDialogWithoutWaitingStrategy>();
            RegisterScreen<SpeechBubble, SpeechBubbleStrategy>();
            RegisterScreen<InGame, GamePakDictionaryStrategy>();
            RegisterScreen<GameSummary, GameSummaryStrategy>();
            RegisterScreen<BonusGameWaiting, BonusGameWaitingStrategy>();
            RegisterScreen<InBonusGame, BonusGameStrategy>();
            RegisterScreen<InPuzzleGame, PuzzleGameStrategy>();
            RegisterScreen<BonusGameComplete, AcceptDialogWithTransitionStrategy>();
        }
    }
}