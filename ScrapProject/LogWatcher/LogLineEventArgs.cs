// <copyright file="LogLineEventArgs.cs" company="GenericEventHandler">
//     Copyright (c) GenericEventHandler all rights reserved. Licensed under the Mit license.
// </copyright>

namespace DRJ.Utilities
{
    /// <summary>Log Line Event Args.</summary>
    public class LogLineEventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="LogLineEventArgs"/> class.</summary>
        /// <param name="line">The line.</param>
        public LogLineEventArgs(string line)
        {
            this.Line = line;
        }

        /// <summary>Gets or sets the line.</summary>
        public string Line { get; set; }
    }
}