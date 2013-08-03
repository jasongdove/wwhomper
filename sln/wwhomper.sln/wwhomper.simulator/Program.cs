using Ninject;
using Ninject.Extensions.Logging;
using wwhomper.Data;
using wwhomper.Dictionary;
using wwhomper.Strategies;
using wwhomper.simulator.Strategies.Trash;

namespace wwhomper.simulator
{
    public class Program
    {
        public Program(
            ILogger logger,
            IPakDictionary pakDictionary,
            PuzzleGameState state,
            ITrashGearStrategy trashGearStrategy,
            PuzzleGameSimulator simulator)
        {
            var strategy = new PuzzleGameStrategy(
                logger,
                pakDictionary,
                state,
                trashGearStrategy);

            simulator.EvaluateStrategy(strategy);
        }

        public static void Main(string[] args)
        {
            var kernel = new StandardKernel(new sharperbot.Module(), new wwhomper.Module());
            
            kernel.Rebind<ITrashGearStrategy>().To<BasicTrashGearStrategy>();

            var p = kernel.Get<Program>();
        }
    }
}
