using Emgu.CV;
using Emgu.CV.Structure;

namespace sharperbot.Assets
{
    public interface IAssetCatalog
    {
        void Dump(string targetDirectory);
        string GetEntryText(string fileName);
        Image<Bgra, byte> GetEntryImage(string fileName);
        Image<Bgra, byte> GetCompositeImage(string fileName);
    }
}