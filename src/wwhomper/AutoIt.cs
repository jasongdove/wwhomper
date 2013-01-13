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

        public static bool WindowExists(string title, string text = "")
        {
            return AutoItNative.AU3_WinExists(title, text) != 0;
        }

        public static Rectangle GetWindowRectangle(string title, string text = "")
        {
            var x = AutoItNative.AU3_WinGetPosX(title, text);
            var y = AutoItNative.AU3_WinGetPosY(title, text);
            var width = AutoItNative.AU3_WinGetPosWidth(title, text);
            var height = AutoItNative.AU3_WinGetPosHeight(title, text);

            return new Rectangle(x, y, width, height);
        }

        public static void ActivateWindow(string title, string text = "")
        {
            AutoItNative.AU3_WinActivate(title, text);
        }

        public static Bitmap GetWindowContents(string title, string text = "")
        {
            ActivateWindow(title);

            var rect = GetWindowRectangle(title);
            var bmp = new Bitmap(rect.Width, rect.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(new Point(rect.Left, rect.Top), Point.Empty, rect.Size);
            }
            return bmp;
        }

        public static TemplateSearchResult IsTemplateInWindow(
            string title,
            string iconName,
            float tolerance = 0.95f)
        {
            Image<Gray, byte> template = TemplateLoader.LoadTemplate(iconName);
            return IsTemplateInWindow(title, template, tolerance);
        }

        public static TemplateSearchResult IsTemplateInWindow(
            string title,
            Image<Gray, byte> template,
            float tolerance = 0.95f)
        {
            Image<Gray, Byte> source;
            using (Bitmap bmp = GetWindowContents(title))
            {
                source = new Image<Gray, Byte>(bmp);
            }

            var match = source.MatchTemplate(template, Emgu.CV.CvEnum.TM_TYPE.CV_TM_CCOEFF_NORMED);
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
            return IsTemplateInWindow(title, screen.Icon, tolerance).Success;
        }

        public static void Click(string title, int x, int y, int speed)
        {
            ActivateWindow(title);
            AutoItNative.AU3_MouseClick("left", x, y, 1, speed);
        }

        public static TemplateSearchResult WaitForTemplate(string title, Image<Gray, byte> template)
        {
            var endTime = DateTime.Now.Add(WordWhomper.ControlTimeout);

            TemplateSearchResult search;
            do
            {
                search = IsTemplateInWindow(title, template);
                Thread.Sleep(Random.Next(300, 700));
            } while (DateTime.Now < endTime && !search.Success);

            return search;
        }
    }
}