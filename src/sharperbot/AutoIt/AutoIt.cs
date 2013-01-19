using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using sharperbot.Screens;

namespace sharperbot.AutoIt
{
    internal sealed class AutoIt : IAutoIt
    {
        private readonly Random _random = new Random();
        private readonly TimeSpan _controlTimeout = TimeSpan.FromSeconds(15);
        private readonly string _windowTitle;

        static AutoIt()
        {
            AutoItNative.AU3_AutoItSetOption("MouseCoordMode", 0);
        }

        public AutoIt()
        {
            _windowTitle = ConfigurationManager.AppSettings["sharperbot.WindowTitle"];
        }

        public bool WindowExists()
        {
            return AutoItNative.AU3_WinExists(_windowTitle, String.Empty) != 0;
        }

        public Rectangle GetWindowRectangle()
        {
            var x = AutoItNative.AU3_WinGetPosX(_windowTitle, String.Empty);
            var y = AutoItNative.AU3_WinGetPosY(_windowTitle, String.Empty);
            var width = AutoItNative.AU3_WinGetPosWidth(_windowTitle, String.Empty);
            var height = AutoItNative.AU3_WinGetPosHeight(_windowTitle, String.Empty);

            return new Rectangle(x, y, width, height);
        }

        public void ActivateWindow()
        {
            AutoItNative.AU3_WinActivate(_windowTitle, String.Empty);
        }

        public ScreenSearchResult IsTemplateInWindow(
            Image<Bgra, byte> windowContents,
            Image<Bgra, byte> template,
            float tolerance = 0.95f)
        {
            var match = windowContents.MatchTemplate(template, Emgu.CV.CvEnum.TM_TYPE.CV_TM_CCOEFF_NORMED);
            float[, ,] matches = match.Data;
            for (int y = 0; y < match.Height; y++)
            {
                for (int x = 0; x < match.Width; x++)
                {
                    double matchScore = matches[y, x, 0];
                    if (matchScore > tolerance)
                    {
                        return new ScreenSearchResult { Success = true, Point = new Point(x, y) };
                    }
                }
            }

            return new ScreenSearchResult { Success = false };
        }

        public bool IsScreenActive(GameScreen screen, float tolerance = 0.95f)
        {
            var windowContents = GetWindowImage();
            return screen.IsActive(windowContents).Success;
        }

        public void Click(int x, int y, int speed)
        {
            if (AutoItNative.AU3_WinActive(_windowTitle, String.Empty) != 0)
            {
                AutoItNative.AU3_MouseClick("left", x, y, 1, speed);
            }
        }

        public ScreenSearchResult WaitForScreen(params GameScreen[] screens)
        {
            if (AutoItNative.AU3_WinActive(_windowTitle, String.Empty) == 0)
            {
                return new ScreenSearchResult();
            }

            // Don't want the mouse to be in any of the screenshots we use
            MoveMouseOffscreen();

            var endTime = DateTime.Now.Add(_controlTimeout);

            var search = new ScreenSearchResult();
            do
            {
                Thread.Sleep(100);

                if (AutoItNative.AU3_WinActive(_windowTitle, String.Empty) == 0)
                {
                    return search;
                }

                var windowContents = GetWindowImage();
                foreach (var screen in screens)
                {
                    search = screen.IsActive(windowContents);
                    if (search.Success)
                    {
                        search.Screen = screen;
                        Console.WriteLine("Detected screen: {0}", screen.GetType().Name);
                        break;
                    }
                }
            } while (DateTime.Now < endTime && !search.Success);

            return search;
        }

        public ScreenSearchResult WaitForTemplate(params Image<Bgra, byte>[] templates)
        {
            var endTime = DateTime.Now.Add(_controlTimeout);

            var search = new ScreenSearchResult();
            do
            {
                Thread.Sleep(100);

                if (AutoItNative.AU3_WinActive(_windowTitle, String.Empty) == 0)
                {
                    return search;
                }

                var windowContents = GetWindowImage();
                foreach (var template in templates)
                {
                    search = IsTemplateInWindow(windowContents, template);
                    if (search.Success)
                    {
                        break;
                    }
                }
            } while (DateTime.Now < endTime && !search.Success);

            return search;
        }

        public Image<Bgra, byte> GetWindowImage()
        {
            ActivateWindow();

            var rect = GetWindowRectangle();
            using (var bmp = new Bitmap(rect.Width, rect.Height))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(new Point(rect.Left, rect.Top), Point.Empty, rect.Size);
                }

                return new Image<Bgra, Byte>(bmp);
            }
        }

        public void MoveMouseOffscreen()
        {
            if (AutoItNative.AU3_MouseGetPosY() > 20)
            {
                int x = _random.Next(0, 500);
                int y = _random.Next(0, 20);
                int speed = _random.Next(2, 6);

                AutoItNative.AU3_MouseMove(x, y, speed);
            }
        }

        public void Type(string text)
        {
            if (AutoItNative.AU3_WinActive(_windowTitle, String.Empty) != 0)
            {
                AutoItNative.AU3_Send(text, 0);
            }
        }

        public void Click(Rectangle rectangle)
        {
#if DEBUG
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "clicks");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = Path.Combine(path, DateTime.Now.Ticks + ".png");
            var windowContents = GetWindowImage();
            windowContents.Draw(rectangle, new Bgra(255, 255, 255, 255), 1);
            windowContents.Save(fileName);
#endif

            var x = _random.Next(rectangle.Left, rectangle.Right);
            var y = _random.Next(rectangle.Top, rectangle.Bottom);
            var speed = _random.Next(2, 10);

            Click(x, y, speed);
        }

        public bool IsWindowActive()
        {
            return AutoItNative.AU3_WinActive(_windowTitle, String.Empty) != 0;
        }
    }
}