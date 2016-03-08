namespace DRJ.Utilities
{
    public class LogLineEventArgs
    {
        /// Initializes a new instance of the <see cref="LogLineEventArgs"/> class. </summary>
        /// <param name="line"> The line. </param>
        public LogLineEventArgs(string line)
        {
            this.Line = line;
        }

        /// <summary>
        /// Gets or sets the line.
        /// </summary>
        public string Line { get; set; }
    }
}