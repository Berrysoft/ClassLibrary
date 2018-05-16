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
        public Log(string fileName)
        {
            Writer = new StreamWriter(fileName, true, Encoding.Unicode);
        }
        public Log(StreamWriter stream)
        {
            Writer = stream;
        }
        protected StreamWriter Writer { get; }
        protected virtual string ExceptionHeader => "Exception";
        protected virtual string EventHeader => "Event";
        protected virtual string DebugHeader => "Debug";
        protected virtual string MessageFormatString => "{0}|{1}";
        protected virtual string SpecialMessageFormatString => "{0}|{1}: {2}";
        public virtual void WriteLog(string message) 
            => Writer.WriteLine(MessageFormatString, DateTime.Now, message);
        public virtual void WriteException(Exception exception)
            => Writer.WriteLine(SpecialMessageFormatString, DateTime.Now, ExceptionHeader, exception.Message);
        public virtual void WriteEvent(string eventName) 
            => Writer.WriteLine(SpecialMessageFormatString, DateTime.Now, EventHeader, eventName);
        public virtual void WriteDebug(string message) 
            => Writer.WriteLine(SpecialMessageFormatString, DateTime.Now, DebugHeader, message);
        public Task WriteLogAsync(string message)
            => Writer.WriteLineAsync(string.Format(MessageFormatString, DateTime.Now, message));
        public Task WriteExceptionAsync(Exception exception)
            => Writer.WriteLineAsync(string.Format(SpecialMessageFormatString, DateTime.Now, ExceptionHeader, exception.Message));
        public Task WriteEventAsync(string eventName)
            => Writer.WriteLineAsync(string.Format(SpecialMessageFormatString, DateTime.Now, EventHeader, eventName));
        public Task WriteDebugAsync(string message)
            => Writer.WriteLineAsync(string.Format(SpecialMessageFormatString, DateTime.Now, DebugHeader, message));
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
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
