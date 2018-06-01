﻿using System;

namespace Berrysoft.Console
{
    internal static class ExceptionHelper
    {
        public static Exception ArgumentNull(string paramName)
            => new ArgumentNullException(paramName);
        public static Exception ArgInvalid(string argName)
            => new ArgInvalidException(argName);
        public static Exception ArgInvalid(string argName, string message, Exception innerException)
            => new ArgInvalidException(argName, message, innerException);
        public static Exception ArgRepeated(string argName)
            => new ArgRepeatedException(argName);
        public static Exception ArgRequired(string argName)
            => new ArgRequiredException(argName);
    }
    public class ArgInvalidException : Exception
    {
        public ArgInvalidException()
            : this(null, null, null)
        { }
        public ArgInvalidException(string argName)
            : this(argName, null, null)
        { }
        public ArgInvalidException(string argName, string message)
            : this(argName, message, null)
        { }
        public ArgInvalidException(string argName, string message, Exception innerException)
            : base(message, innerException)
        {
            ArgName = argName;
        }
        public string ArgName { get; }
    }
    public class ArgRepeatedException : ArgInvalidException
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
    public class ArgRequiredException : ArgInvalidException
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