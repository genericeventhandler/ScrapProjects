using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRJ.Utilities
{
    public static class LogConsoleWatcher
    {
        /// <summary>The main.</summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            var lw = new LogWatcher(args[0]);
            lw.LogLine += OnLogLine;
            lw.Start();
            Console.ReadKey();
            lw.Stop();
        }

        /// <summary>The on log line.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public static void OnLogLine(object sender, LogLineEventArgs e)
        {
            if (e.Line.Contains("Error") || e.Line.Contains("Exception"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (e.Line.Contains("Information"))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            else if (e.Line.Contains("Debug"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (e.Line.Contains("Extended Properties"))
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            Console.WriteLine(e.Line);
        }
    }
}