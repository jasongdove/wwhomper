using Ninject.Extensions.Logging;
using sharperbot.Assets;

namespace wwhomper.Dictionary
{
    public class PakDictionary : WordList, IPakDictionary
    {
        public PakDictionary(IAssetCatalog assetCatalog, ILogger logger)
            : base(logger)
        {
            LoadFromDictionary(assetCatalog.GetEntryText(@"Dictionary\us-uk-fr\dictionary.txt"));
        }
    }
}