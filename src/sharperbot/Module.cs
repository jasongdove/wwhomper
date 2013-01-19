using System.Configuration;
using Ninject.Activation;
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
            Bind<IAssetCatalog>().ToMethod(CreatePakCatalog).InSingletonScope();
        }

        private PakCatalog CreatePakCatalog(IContext context)
        {
            var catalog = new PakCatalog(ConfigurationManager.AppSettings["sharperbot.pakcatalog"]);
            catalog.Load();
            return catalog;
        }
    }
}
