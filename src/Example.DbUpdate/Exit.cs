using System;
using System.Linq;

namespace Example.DbUpdate
{
    public static class Exit
    {
        public static void ExitError(Exception ex, string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex);
            Console.ResetColor();
            StopOnExit(args);
            Environment.Exit(-1);
        }

        public static void StopOnExit(string[] args)
        {
            if (args.Length > 0 && args.AsEnumerable().Any(a => a.ToLower() == "--stoponexit"))
            {
                var timer = new System.Timers.Timer(15000);
                timer.Elapsed += (sender, eventArgs) => { Environment.Exit(0); };
                timer.Start();
                Console.WriteLine(string.Empty);
                Console.WriteLine("The window will close in 15 seconds. Press any key to exit now.");
                Console.WriteLine(string.Empty);
                Console.ReadKey();
            }
        }
    }
}