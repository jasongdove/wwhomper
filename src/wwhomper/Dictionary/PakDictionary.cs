using sharperbot.Assets;

namespace wwhomper.Dictionary
{
    public class PakDictionary : WordList, IPakDictionary
    {
        public PakDictionary(IAssetCatalog assetCatalog)
        {
            LoadFromDictionary(assetCatalog.GetEntryText(@"Dictionary\us-uk-fr\dictionary.txt"));
        }
    }
}