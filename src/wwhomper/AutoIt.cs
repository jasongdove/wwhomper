using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using wwhomper.Screens;

namespace wwhomper
{
    internal static class AutoIt
    {
        private static readonly Random Random = new Random();

        static AutoIt()
        {
            AutoItNative.AU3_AutoItSetOption("MouseCoordMode", 0);
        }

        public static bool WindowExists(string title)
        {
            return AutoItNative.AU3_WinExists(title, String.Empty) != 0;
        }

        public static Rectangle GetWindowRectangle(string title)
        {
            var x = AutoItNative.AU3_WinGetPosX(title, String.Empty);
            var y = AutoItNative.AU3_WinGetPosY(title, String.Empty);
            var width = AutoItNative.AU3_WinGetPosWidth(title, String.Empty);
            var height = AutoItNative.AU3_WinGetPosHeight(title, String.Empty);

            return new Rectangle(x, y, width, height);
        }

        public static void ActivateWindow(string title)
        {
            AutoItNative.AU3_WinActivate(title, String.Empty);
        }

        public static ScreenSearchResult IsTemplateInWindow(
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

        public static bool IsScreenActive(string title, ScreenBase screen, float tolerance = 0.95f)
        {
            var windowContents = GetWindowImage(title);
            return screen.IsActive(windowContents).Success;
        }

        public static void Click(string title, int x, int y, int speed)
        {
            if (AutoItNative.AU3_WinActive(title, String.Empty) != 0)
            {
                AutoItNative.AU3_MouseClick("left", x, y, 1, speed);
            }
        }

        public static ScreenSearchResult WaitForScreen(string title, params ScreenBase[] screens)
        {
            if (AutoItNative.AU3_WinActive(title, String.Empty) == 0)
            {
                return new ScreenSearchResult();
            }

            // Don't want the mouse to be in any of the screenshots we use
            MoveMouseOffscreen();

            var endTime = DateTime.Now.Add(WordWhomper.ControlTimeout);

            var search = new ScreenSearchResult();
            do
            {
                Thread.Sleep(100);

                if (AutoItNative.AU3_WinActive(title, String.Empty) == 0)
                {
                    return search;
                }

                var windowContents = GetWindowImage(title);
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

        public static ScreenSearchResult WaitForTemplate(string title, params Image<Bgra, byte>[] templates)
        {
            var endTime = DateTime.Now.Add(WordWhomper.ControlTimeout);

                var search = new ScreenSearchResult();
                do
                {
                    Thread.Sleep(100);

                    if (AutoItNative.AU3_WinActive(title, String.Empty) == 0)
                    {
                        return search;
                    }

                    var windowContents = GetWindowImage(title);
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

        public static Image<Bgra, byte> GetWindowImage(string title)
        {
            ActivateWindow(title);

            var rect = GetWindowRectangle(title);
            using (var bmp = new Bitmap(rect.Width, rect.Height))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(new Point(rect.Left, rect.Top), Point.Empty, rect.Size);
                }

                return new Image<Bgra, Byte>(bmp);
            }
        }

        public static void MoveMouseOffscreen()
        {
            if (AutoItNative.AU3_MouseGetPosY() > 20)
            {
                int x = Random.Next(0, 500);
                int y = Random.Next(0, 20);
                int speed = Random.Next(2, 6);

                AutoItNative.AU3_MouseMove(x, y, speed);
            }
        }

        public static void Type(string title, string text)
        {
            if (AutoItNative.AU3_WinActive(title, String.Empty) != 0)
            {
                AutoItNative.AU3_Send(text, 0);
            }
        }

        public static void Click(Rectangle rectangle)
        {
#if DEBUG
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "clicks");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = Path.Combine(path, DateTime.Now.Ticks + ".png");
            var windowContents = GetWindowImage(WordWhomper.WindowTitle);
            windowContents.Draw(rectangle, new Bgra(255, 255, 255, 255), 1);
            windowContents.Save(fileName);
#endif

            var x = Random.Next(rectangle.Left, rectangle.Right);
            var y = Random.Next(rectangle.Top, rectangle.Bottom);
            var speed = Random.Next(2, 10);

            Click(WordWhomper.WindowTitle, x, y, speed);
        }

        public static bool IsWindowActive(string title)
        {
            return AutoItNative.AU3_WinActive(title, String.Empty) != 0;
        }
    }
}