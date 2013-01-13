using System;
using System.Drawing;
using System.Reflection;
using Emgu.CV;
using Emgu.CV.Structure;
using wwhomper.Screens;

namespace wwhomper
{
    public static class TemplateLoader
    {
        public static Image<Gray, byte> LoadTemplate(string iconName)
        {
            var assembly = Assembly.GetAssembly(typeof(ScreenBase));
            using (var stream = assembly.GetManifestResourceStream(String.Format("wwhomper.Icons.{0}", iconName)))
            {
                using (var bmp = new Bitmap(stream))
                {
                    return new Image<Gray, byte>(bmp);
                }
            }
        }
    }
}