using Ninject.Modules;
using wwhomper.Data;
using wwhomper.Dictionary;

namespace wwhomper
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Bind<IPakDictionary>().To<PakDictionary>().InSingletonScope();
            Bind<PuzzleGameState>().ToSelf().InSingletonScope();
        }
    }
}
