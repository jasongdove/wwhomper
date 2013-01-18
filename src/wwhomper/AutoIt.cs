using System;
using System.Drawing;
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

        public static Bitmap GetWindowContents(string title)
        {
            return GetWindowContents(title, Rectangle.Empty);
        }

        public static Bitmap GetWindowContents(string title, Rectangle rect)
        {
            ActivateWindow(title);

            rect = rect == Rectangle.Empty ? GetWindowRectangle(title) : rect;
            var bmp = new Bitmap(rect.Width, rect.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(new Point(rect.Left, rect.Top), Point.Empty, rect.Size);
            }
            return bmp;
        }

        public static ScreenSearchResult IsTemplateInWindow(
            Image<Bgra, byte> windowContents,
            Image<Bgra, byte> template,
            float tolerance = 0.95f)
        {
            var match = windowContents.MatchTemplate(template, Emgu.CV.CvEnum.TM_TYPE.CV_TM_CCOEFF_NORMED);
            float[, ,] matches = match.Data;
            for (int y = 0; y < matches.GetLength(0); y++)
            {
                for (int x = 0; x < matches.GetLength(1); x++)
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
            using (Bitmap bmp = GetWindowContents(title))
            {
                return new Image<Bgra, Byte>(bmp);
            }
        }

        public static void MoveMouseOffscreen()
        {
            int x = Random.Next(0, 500);
            int y = Random.Next(0, 20);
            int speed = Random.Next(2, 6);

            AutoItNative.AU3_MouseMove(x, y, speed);
        }

        public static void Type(string title, string text)
        {
            if (AutoItNative.AU3_WinActive(title, String.Empty) != 0)
            {
                AutoItNative.AU3_Send(text, 0);
            }
        }
    }
}