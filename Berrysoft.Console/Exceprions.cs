using System;

namespace Berrysoft.Console
{
    public class ArgNotValidException : Exception
    {
        public ArgNotValidException()
            : this(null, null, null)
        { }
        public ArgNotValidException(string argName)
            : this(argName, null, null)
        { }
        public ArgNotValidException(string argName, string message)
            : this(argName, message, null)
        { }
        public ArgNotValidException(string argName, string message, Exception innerException)
            : base(message, innerException)
        {
            ArgName = argName;
        }
        public string ArgName { get; }
    }
    public class ArgRepeatedException : ArgNotValidException
    {
        public ArgRepeatedException()
            : base()
        { }
        public ArgRepeatedException(string argName)
            : base(argName, "Argument repeated.")
        { }
        public ArgRepeatedException(string argName, Exception innerException)
            : base(argName, "Argument repeated.", innerException)
        { }
    }
    public class ArgRequiredException : ArgNotValidException
    {
        public ArgRequiredException()
            : base()
        { }
        public ArgRequiredException(string argName)
            : base(argName, "Argument required.")
        { }
        public ArgRequiredException(string argName, Exception innerException)
            : base(argName, "Argument requited.", innerException)
        { }
    }
}
