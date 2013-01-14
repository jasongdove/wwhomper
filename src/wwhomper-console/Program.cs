using System;
using wwhomper;

namespace wwhomper_console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var whomper = new WordWhomper();
            whomper.Run();

            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
