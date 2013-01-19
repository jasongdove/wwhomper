using Ninject;
using Ninject.Extensions.Logging;
using sharperbot.AutoIt;
using sharperbot.Screens;
using sharperbot.Strategies;
using wwhomper.Screens;

namespace wwhomper.Strategies
{
    public class SpeechBubbleStrategy : ScreenSearchStrategy, IScreenStrategy<SpeechBubble>
    {
        public SpeechBubbleStrategy(IKernel kernel, IAutoIt autoIt, ILogger logger)
            : base(kernel, autoIt, logger)
        {
            RegisterNestedScreen<PuzzleGameComplete, AcceptDialogWithTransitionStrategy>();
            RegisterNestedScreen<NewGear, AcceptDialogWithTransitionStrategy>();
            RegisterNestedScreen<BonusAcorns, AcceptDialogWithTransitionStrategy>();
            RegisterNestedScreen<BlowTorch, AcceptDialogWithTransitionStrategy>();
            RegisterNestedScreen<PaintBrush, AcceptDialogWithTransitionStrategy>();
        }

        public void ExecuteStrategy(SpeechBubble screen)
        {
            PerformSearch();
        }
    }
}