// <copyright file="LogConsoleWatcher.cs" company="GenericEventHandler">
//     Copyright (c) GenericEventHandler all rights reserved. Licensed under the Mit license.
// </copyright>
namespace DRJ.Utilities
{
    using System;

    /// <summary>
    /// watches a log file in the console screen without having to refresh it all the time.
    /// </summary>
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
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender), "when calling events pass this as the first parameter");
            }

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