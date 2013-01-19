using Ninject.Modules;
using sharperbot.Assets;
using sharperbot.AutoIt;

namespace sharperbot
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Bind<IAutoIt>().To<AutoIt.AutoIt>().InSingletonScope();
            Bind<IAssetCatalog>().To<PakCatalog>().InSingletonScope();
        }
    }
}
