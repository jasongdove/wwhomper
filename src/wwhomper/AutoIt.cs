using System;
using System.Drawing;
using System.Linq;
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

        public static TemplateSearchResult IsTemplateInWindow(
            string title,
            string templateName,
            float tolerance = 0.95f)
        {
            var windowContents = GetWindowImage(title);
            Image<Gray, byte> template = TemplateLoader.LoadTemplate(templateName);
            return IsTemplateInWindow(windowContents, template, tolerance);
        }

        public static TemplateSearchResult IsTemplateInWindow(
            Image<Gray, byte> windowContents,
            string templateName,
            float tolerance = 0.95f)
        {
            Image<Gray, byte> template = TemplateLoader.LoadTemplate(templateName);
            return IsTemplateInWindow(windowContents, template, tolerance);
        }

        public static TemplateSearchResult IsTemplateInWindow(
            Image<Gray, byte> windowContents,
            Image<Gray, byte> template,
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
                        return new TemplateSearchResult { Success = true, Point = new Point(x, y) };
                    }
                }
            }

            return new TemplateSearchResult { Success = false };
        }

        public static bool IsScreenActive(string title, ScreenBase screen, float tolerance = 0.95f)
        {
            var windowContents = GetWindowImage(title);
            return IsTemplateInWindow(windowContents, screen.Template, tolerance).Success;
        }

        public static bool IsScreenActiveFast(ScreenBase screen)
        {
            foreach (var anchor in screen.Anchors)
            {
                var color = AutoItNative.AU3_PixelGetColor(anchor.Point.X, anchor.Point.Y);
                if (color != anchor.Color)
                {
                    return false;
                }
            }

            return true;
        }

        public static void Click(string title, int x, int y, int speed)
        {
            if (AutoItNative.AU3_WinActive(title, String.Empty) != 0)
            {
                AutoItNative.AU3_MouseClick("left", x, y, 1, speed);
            }
        }

        public static TemplateSearchResult WaitForScreen(string title, params ScreenBase[] screens)
        {
            var templates = screens.Select(x => x.Template);
            
            var result = WaitForTemplate(title, templates.ToArray());
            if (result.Success)
            {
                var screen = screens.Single(x => x.Template == result.Template);
                var screenName = screen.GetType().Name;
                Console.WriteLine("Detected screen: {0}", screenName);
            }

            return result;
        }

        public static TemplateSearchResult WaitForTemplate(string title, params Image<Gray, byte>[] templates)
        {
            var endTime = DateTime.Now.Add(WordWhomper.ControlTimeout);

            var search = new TemplateSearchResult();
            do
            {
                Thread.Sleep(100);

                var windowContents = GetWindowImage(title);
                foreach (var template in templates)
                {
                    search = IsTemplateInWindow(windowContents, template);
                    if (search.Success)
                    {
                        search.Template = template;
                        break;
                    }
                }
            } while (DateTime.Now < endTime && !search.Success);

            return search;
        }

        public static Image<Gray, byte> GetWindowImage(string title)
        {
            using (Bitmap bmp = GetWindowContents(title))
            {
                return new Image<Gray, Byte>(bmp);
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