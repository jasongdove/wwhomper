using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using sharperbot.Screens;

namespace sharperbot.AutoIt
{
    public interface IAutoIt
    {
        bool WindowExists();
        Rectangle GetWindowRectangle();
        void ActivateWindow();
        ScreenSearchResult IsTemplateInWindow(Image<Bgra, byte> windowContents, Image<Bgra, byte> template, float tolerance = 0.95f);
        bool IsScreenActive(IGameScreen screen, float tolerance = 0.95f);
        void Click(int x, int y, int speed);
        ScreenSearchResult WaitForScreen(params IGameScreen[] screens);
        ScreenSearchResult WaitForTemplate(params Image<Bgra, byte>[] templates);
        Image<Bgra, byte> GetWindowImage();
        void MoveMouseOffscreen();
        void Type(string text);
        void Click(Rectangle rectangle);
        bool IsWindowActive();
    }
}