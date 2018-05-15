using System;
using System.IO;
using System.Text;

namespace Berrysoft.Console
{
    public interface ILog
    {
        void WriteLog(string message);
        void WriteException(Exception exception);
        void WriteEvent(string eventName);
        void WriteDebug(string message);
    }
    public class Log : ILog, IDisposable
    {
        public Log(string fileName)
        {
            FileName = fileName;
            Writer = new StreamWriter(fileName, true, Encoding.Unicode);
        }
        protected StreamWriter Writer { get; }
        public string FileName { get; }
        protected virtual string ExceptionHeader => "Exception";
        protected virtual string EventHeader => "Event";
        protected virtual string DebugHeader => "Debug";
        protected virtual string MessageFormatString => "{0}|{1}";
        protected virtual string SpecialMessageFormatString => "{0}|{1}: {2}";
        public virtual void WriteLog(string message)
        {
            Writer.WriteLine(MessageFormatString, DateTime.Now, message);
        }
        public virtual void WriteException(Exception exception)
        {
            Writer.WriteLine(SpecialMessageFormatString, DateTime.Now, ExceptionHeader, exception.Message);
        }
        public virtual void WriteEvent(string eventName)
        {
            Writer.WriteLine(SpecialMessageFormatString, DateTime.Now, EventHeader, eventName);
        }
        public virtual void WriteDebug(string message)
        {
            Writer.WriteLine(SpecialMessageFormatString, DateTime.Now, DebugHeader, message);
        }
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
