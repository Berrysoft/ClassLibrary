using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Berrysoft.Console
{
    /// <summary>
    /// Represents a writer of a simple log.
    /// </summary>
    public class Log : IDisposable
    {
        /// <summary>
        /// Initialize a new instance of <see cref="Log"/> class.
        /// </summary>
        /// <param name="fileName">Path of the log file.</param>
        public Log(string fileName)
            : this(fileName, false)
        { }
        /// <summary>
        /// Initialize a new instance of <see cref="Log"/> class.
        /// </summary>
        /// <param name="fileName">Path of the log file.</param>
        /// <param name="append">Whether append the log or overwrite it.</param>
        public Log(string fileName, bool append)
            : this(new StreamWriter(fileName, append, Encoding.Unicode))
        { }
        /// <summary>
        /// Initialize a new instance of <see cref="Log"/> class.
        /// </summary>
        /// <param name="stream">The writer of the log file.</param>
        public Log(StreamWriter stream)
        {
            Writer = stream;
        }
        /// <summary>
        /// The writer of the log file.
        /// </summary>
        protected StreamWriter Writer { get; }
        /// <summary>
        /// The header of exception lines.
        /// </summary>
        protected virtual string ExceptionHeader => "Exception";
        /// <summary>
        /// The header of event lines.
        /// </summary>
        protected virtual string EventHeader => "Event";
        /// <summary>
        /// The header of debug lines.
        /// </summary>
        protected virtual string DebugHeader => "Debug";
        /// <summary>
        /// The format string of time and message. {0} for time and {1} for message.
        /// </summary>
        protected virtual string MessageFormatString => "{0}\t{1}";
        /// <summary>
        /// The format string of special message. {0} for header and {1} for message.
        /// </summary>
        protected virtual string SpecialMessageFormatString => "{0}: {1}";
        /// <summary>
        /// Write a piece of message in a single line.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public virtual void WriteLog(string message)
            => Writer.WriteLine(MessageFormatString, DateTime.Now, message);
        /// <summary>
        /// Write an exception.
        /// </summary>
        /// <param name="exception">Exception to write.</param>
        public virtual void WriteException(Exception exception)
            => WriteLog(string.Format(SpecialMessageFormatString, ExceptionHeader, exception.Message));
        /// <summary>
        /// Write an event.
        /// </summary>
        /// <param name="eventName">Event to write.</param>
        public virtual void WriteEvent(string eventName)
            => WriteLog(string.Format(SpecialMessageFormatString, EventHeader, eventName));
        /// <summary>
        /// Write a debug message.
        /// </summary>
        /// <param name="message">Debug message to write.</param>
        public virtual void WriteDebug(string message)
            => WriteLog(string.Format(SpecialMessageFormatString, DebugHeader, message));
        /// <summary>
        /// Flush the writer.
        /// </summary>
        public void Flush()
            => Writer.Flush();
        /// <summary>
        /// Write a piece of message in a single line asynchronously.
        /// </summary>
        /// <param name="message">Message to write.</param>
        /// <returns>The write <see cref="Task"/>.</returns>
        public Task WriteLogAsync(string message)
            => Writer.WriteLineAsync(string.Format(MessageFormatString, DateTime.Now, message));
        /// <summary>
        /// Write an exception asynchronously.
        /// </summary>
        /// <param name="exception">Exception to write.</param>
        /// <returns>The write <see cref="Task"/>.</returns>
        public Task WriteExceptionAsync(Exception exception)
            => WriteLogAsync(string.Format(SpecialMessageFormatString, ExceptionHeader, exception.Message));
        /// <summary>
        /// Write an event asynchronously.
        /// </summary>
        /// <param name="eventName">Event to write.</param>
        /// <returns>The write <see cref="Task"/>.</returns>
        public Task WriteEventAsync(string eventName)
            => WriteLogAsync(string.Format(SpecialMessageFormatString, EventHeader, eventName));
        /// <summary>
        /// Write a debug message asynchronously.
        /// </summary>
        /// <param name="message">Debug message to write.</param>
        /// <returns>The write <see cref="Task"/>.</returns>
        public Task WriteDebugAsync(string message)
            => WriteLogAsync(string.Format(SpecialMessageFormatString, DebugHeader, message));
        /// <summary>
        /// Flush the writer asynchronously.
        /// </summary>
        /// <returns>The flush <see cref="Task"/>.</returns>
        public Task FlushAsync()
            => Writer.FlushAsync();
        #region IDisposable Support
        private bool disposedValue = false;
        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="StreamWriter"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Writer.Close();
                }
                disposedValue = true;
            }
        }
        /// <summary>
        /// Closes the current <see cref="StreamWriter"/> object and the underlying stream.
        /// </summary>
        public void Dispose() => Dispose(true);
        #endregion
    }
}
