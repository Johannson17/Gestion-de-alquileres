using System;
using System.Diagnostics;

namespace Services.Domain
{
    /// <summary>
    /// Represents a log entry for system monitoring and debugging.
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Message of the log entry.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Severity level of the log entry.
        /// </summary>
        public TraceLevel TraceLevel { get; set; }

        /// <summary>
        /// Timestamp when the log entry was created.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Initializes a new instance of the Log class.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="traceLevel">The severity level of the log (default is TraceLevel.Info).</param>
        /// <param name="date">The timestamp of the log entry (default is current time).</param>
        public Log(string message, TraceLevel traceLevel = TraceLevel.Info, DateTime date = default)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message), "Log message cannot be null.");
            TraceLevel = traceLevel;
            Date = (date == default) ? DateTime.Now : date;
        }

        // Additional methods or properties can be added here to support more features such as logging to different targets, formatting, etc.

        /// <summary>
        /// Returns a formatted string representation of the log entry.
        /// </summary>
        /// <returns>A string that represents the current log object.</returns>
        public override string ToString()
        {
            return $"{Date:yyyy-MM-dd HH:mm:ss} [{TraceLevel}]: {Message}";
        }
    }
}