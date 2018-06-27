using System;
using System.Collections.Generic;

namespace Berrysoft.Data
{
    internal static class ExceptionHelper
    {
        public static Exception ArgumentNull(string paramName)
            => new ArgumentNullException(paramName);
        public static Exception PairExisted()
            => new PairExistedException();
        public static Exception KeyNotFound()
            => new KeyNotFoundException();
        public static Exception ArgumentOutOfRange(string paramName)
            => new ArgumentOutOfRangeException(paramName);
        public static Exception ArrayTooSmall()
            => new ArgumentException("The array is too small.");
        public static Exception NotSupported()
            => new NotSupportedException();
    }
    /// <summary>
    /// Represents errors that a key pair is existed.
    /// </summary>
    public class PairExistedException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PairExistedException"/> class.
        /// </summary>
        public PairExistedException()
            : base("The key pair is existed.")
        { }
    }
}
