using System;
using wwhomper;
using wwhomper_console.Properties;

namespace wwhomper_console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var whomper = new WordWhomper(Settings.Default.GameRoot);
            whomper.Run();

            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
