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
        /// Unique identifier for the log entry.
        /// </summary>
        public Guid Id { get; set; }

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
        /// Optional details of the exception if applicable.
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// Initializes a new instance of the Log class.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="traceLevel">The severity level of the log (default is TraceLevel.Info).</param>
        /// <param name="exception">The exception details, if any.</param>
        /// <param name="date">The timestamp of the log entry (default is current time).</param>
        public Log(string message, TraceLevel traceLevel = TraceLevel.Info, string exception = null, DateTime date = default)
        {
            Id = Guid.NewGuid();
            Message = message ?? throw new ArgumentNullException(nameof(message), "Log message cannot be null.");
            TraceLevel = traceLevel;
            Exception = exception;
            Date = (date == default) ? DateTime.Now : date;
        }

        /// <summary>
        /// Returns a formatted string representation of the log entry.
        /// </summary>
        /// <returns>A string that represents the current log object.</returns>
        public override string ToString()
        {
            return $"{Date:yyyy-MM-dd HH:mm:ss} [{TraceLevel}]: {Message} {(Exception != null ? $"\nException: {Exception}" : string.Empty)}";
        }
    }
}