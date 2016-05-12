// <copyright file="LogWatcher.cs" company="GenericEventHandler">
//     Copyright (c) GenericEventHandler all rights reserved. Licensed under the Mit license.
// </copyright>

namespace DRJ.Utilities
{
    using System;
    using System.IO;
    using System.Threading;

    /// <summary>The log watcher.</summary>
    public class LogWatcher
    {
        /// <summary>The log filename.</summary>
        private readonly string logFilename;

        /// <summary>The last offset.</summary>
        private long lastOffset;

        /// <summary>The running.</summary>
        private bool running;

        /// <summary>Initializes a new instance of the <see cref="LogWatcher"/> class.</summary>
        /// <param name="logFilename">The log filename.</param>
        public LogWatcher(string logFilename)
        {
            this.logFilename = logFilename;
        }

        /// <summary>The finished log.</summary>
        public event EventHandler<EventArgs> FinishedLog;

        /// <summary>The log line.</summary>
        public event EventHandler<LogLineEventArgs> LogLine;

        /// <summary>The reset last read.</summary>
        public void ResetLastRead()
        {
            this.lastOffset = 0;
        }

        /// <summary>The start.</summary>
        public void Start()
        {
            Console.WriteLine("Running");
            this.running = true;
            while (this.running)
            {
                this.Read();
                Thread.Sleep(5000);
            }

            Console.WriteLine("Stopped");
        }

        /// <summary>Stops the log watcher</summary>
        public void Stop()
        {
            Console.WriteLine("Stop called");
            this.running = false;
        }

        /// <summary>The on finished log.</summary>
        /// <param name="e">The e.</param>
        private void OnFinishedLog(EventArgs e)
        {
            var local = this.FinishedLog;
            if (local != null)
            {
                local(this, e);
            }
        }

        /// <summary>The on log line.</summary>
        /// <param name="args">The args.</param>
        private void OnLogLine(LogLineEventArgs args)
        {
            var local = this.LogLine;
            if (local != null)
            {
                local(this, args);
            }
        }

        /// <summary>The read.</summary>
        private void Read()
        {
            if (File.Exists(this.logFilename))
            {
                using (FileStream logReader = new FileStream(this.logFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    logReader.Seek(this.lastOffset, SeekOrigin.Begin);
                    var sr = new StreamReader(logReader);
                    logReader.Seek(this.lastOffset, SeekOrigin.Begin);
                    var line = sr.ReadLine();
                    while (!string.IsNullOrEmpty(line))
                    {
                        this.OnLogLine(new LogLineEventArgs(line));
                        line = sr.ReadLine();
                    }

                    this.lastOffset = logReader.Position;
                    sr.Close();
                    if (sr != null)
                    {
                        sr.Dispose();
                        sr = null;
                    }
                }

                this.OnFinishedLog(new EventArgs());
            }
        }
    }
}