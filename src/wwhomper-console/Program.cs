using System;
using Ninject;
using wwhomper;

namespace wwhomper_console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var kernel = new StandardKernel(new sharperbot.Module(), new wwhomper.Module());
            var whomper = kernel.Get<WordWhomper>();
            whomper.Run();

            Console.ReadKey();
        }
    }
}
