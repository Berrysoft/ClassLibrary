using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Berrysoft.Console
{
    public interface ILog
    {
        void WriteLog(string message);
        void WriteException(Exception exception);
        void WriteEvent(string eventName);
        void WriteDebug(string message);
        Task WriteLogAsync(string message);
        Task WriteExceptionAsync(Exception exception);
        Task WriteEventAsync(string eventName);
        Task WriteDebugAsync(string message);
    }
    public class Log : ILog, IDisposable
    {
        private readonly object syncLock = new object();
        public Log(string fileName)
            : this(fileName, false)
        { }
        public Log(string fileName, bool append)
            : this(new StreamWriter(fileName, append, Encoding.Unicode))
        { }
        public Log(StreamWriter stream)
        {
            Writer = stream;
        }
        protected StreamWriter Writer { get; }
        protected virtual string ExceptionHeader => "Exception";
        protected virtual string EventHeader => "Event";
        protected virtual string DebugHeader => "Debug";
        protected virtual string MessageFormatString => "{0}\t{1}";
        protected virtual string SpecialMessageFormatString => "{0}: {1}";
        public virtual void WriteLog(string message)
            => Writer.WriteLine(MessageFormatString, DateTime.Now, message);
        public virtual void WriteException(Exception exception)
            => WriteLog(string.Format(SpecialMessageFormatString, ExceptionHeader, exception.Message));
        public virtual void WriteEvent(string eventName)
            => WriteLog(string.Format(SpecialMessageFormatString, EventHeader, eventName));
        public virtual void WriteDebug(string message)
            => WriteLog(string.Format(SpecialMessageFormatString, DebugHeader, message));
        public Task WriteLogAsync(string message)
        {
            lock (syncLock)
            {
                return Writer.WriteLineAsync(string.Format(MessageFormatString, DateTime.Now, message));
            }
        }
        public Task WriteExceptionAsync(Exception exception)
            => WriteLogAsync(string.Format(SpecialMessageFormatString, ExceptionHeader, exception.Message));
        public Task WriteEventAsync(string eventName)
            => WriteLogAsync(string.Format(SpecialMessageFormatString, EventHeader, eventName));
        public Task WriteDebugAsync(string message)
            => WriteLogAsync(string.Format(SpecialMessageFormatString, DebugHeader, message));
        #region IDisposable Support
        private bool disposedValue = false;
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
        public void Dispose() => Dispose(true);
        #endregion
    }
}
