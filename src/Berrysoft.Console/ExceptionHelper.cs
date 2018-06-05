using System;

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
    /// <summary>
    /// Represents errors that a command line arg is invalid.
    /// </summary>
    public class ArgInvalidException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ArgInvalidException"/> class.
        /// </summary>
        public ArgInvalidException()
            : this(null, null, null)
        { }
        /// <summary>
        /// Initializes a new instance of <see cref="ArgInvalidException"/> class.
        /// </summary>
        /// <param name="argName">Name of the arg.</param>
        public ArgInvalidException(string argName)
            : this(argName, null, null)
        { }
        /// <summary>
        /// Initializes a new instance of <see cref="ArgInvalidException"/> class.
        /// </summary>
        /// <param name="argName">Name of the arg.</param>
        /// <param name="message">Error message.</param>
        public ArgInvalidException(string argName, string message)
            : this(argName, message, null)
        { }
        /// <summary>
        /// Initializes a new instance of <see cref="ArgInvalidException"/> class.
        /// </summary>
        /// <param name="argName">Name of the arg.</param>
        /// <param name="message">Error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ArgInvalidException(string argName, string message, Exception innerException)
            : base(message, innerException)
        {
            ArgName = argName;
        }
        /// <summary>
        /// Name of the arg.
        /// </summary>
        public string ArgName { get; }
    }
    /// <summary>
    /// Represents errors that a command line arg is repeated.
    /// </summary>
    public class ArgRepeatedException : ArgInvalidException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ArgRepeatedException"/>.
        /// </summary>
        public ArgRepeatedException()
            : base()
        { }
        /// <summary>
        /// Initializes a new instance of <see cref="ArgRepeatedException"/>.
        /// </summary>
        /// <param name="argName">Name of the arg.</param>
        public ArgRepeatedException(string argName)
            : base(argName, "Argument repeated.")
        { }
    }
    /// <summary>
    /// Represents errors that a command line arg is required.
    /// </summary>
    public class ArgRequiredException : ArgInvalidException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ArgRequiredException"/> class.
        /// </summary>
        public ArgRequiredException()
            : base()
        { }
        /// <summary>
        /// Initializes a new instance of <see cref="ArgRequiredException"/> class.
        /// </summary>
        /// <param name="argName">Name of the arg.</param>
        public ArgRequiredException(string argName)
            : base(argName, "Argument required.")
        { }
    }
}
