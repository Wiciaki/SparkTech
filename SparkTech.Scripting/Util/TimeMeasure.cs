namespace SparkTech.Utils
{
    using System;
    using System.Diagnostics;

    using Entropy.ToolKit;
    using Entropy.ToolKit.Enumerations;

    public class TimeMeasure : IDisposable
    {
        #region Constructors and Destructors

        public TimeMeasure(string name, bool outputConsole = true, bool outputChat = true)
        {
            // Apply properties
            this.Name = name;
            this.OutputConsole = outputConsole;
            this.OutputChat = outputChat;

            // Create a new stopwatch and start it
            this.Timer = new Stopwatch();
            this.Timer.Start();
        }

        #endregion

        #region Public Properties

        public string Name { get; set; }

        public bool OutputChat { get; set; }

        public bool OutputConsole { get; set; }

        public Stopwatch Timer { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            // Stop the timer
            this.Timer.Stop();

            if (this.OutputChat)
            {
                GameConsole.Print($"{this.Name}: Action took {this.Timer.Elapsed}", true);
            }
            if (this.OutputConsole)
            {
                Logging.Log($"{this.Name}: Action took {this.Timer.Elapsed}", LogLevels.Debug);
            }
        }

        #endregion
    }
}