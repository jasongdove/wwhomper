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
        public static Image<Bgra, byte> LoadTemplate(string templateName)
        {
            var assembly = Assembly.GetAssembly(typeof(ScreenBase));
            using (var stream = assembly.GetManifestResourceStream(String.Format("wwhomper.Templates.{0}", templateName)))
            {
                using (var bmp = new Bitmap(stream))
                {
                    return new Image<Bgra, byte>(bmp);
                }
            }
        }
    }
}