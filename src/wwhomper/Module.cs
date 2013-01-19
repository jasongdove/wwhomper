using Ninject.Modules;
using wwhomper.Dictionary;

namespace wwhomper
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Bind<IPakDictionary>().To<PakDictionary>().InSingletonScope();
        }
    }
}
