using Emgu.CV;
using Emgu.CV.Structure;

namespace sharperbot.Screens
{
    public interface IGameScreen
    {
        ScreenSearchResult IsActive(Image<Bgra, byte> windowContents);
    }
}