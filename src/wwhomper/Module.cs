using Ninject.Modules;
using wwhomper.Data;
using wwhomper.Dictionary;
using wwhomper.Screens;
using wwhomper.Strategies;

namespace wwhomper
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Bind<IPakDictionary>().To<PakDictionary>().InSingletonScope();
            Bind<PuzzleGameState>().ToSelf().InSingletonScope();

            Bind<ITrashGearStrategy>().To<TrashGearStrategy>();
            Bind<IAcquireGearStrategy>().To<AcquireGearStrategy>();
            Bind<IInPuzzleGame>().To<InPuzzleGame>();
        }
    }
}
