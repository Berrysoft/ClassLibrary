using System;
using System.Collections.Generic;

namespace Berrysoft.Data
{
    internal static class ExceptionHelper
    {
        public static Exception ArgumentNull(string paramName)
            => new ArgumentNullException(paramName);
        public static Exception RootHasParent()
            => new ArgumentException("The root can't have a parent.");
        public static Exception PairExisted()
            => new ArgumentException("The key pair is existed.");
        public static Exception KeyNotFound()
            => new KeyNotFoundException();
        public static Exception KeyExisted(string keyMessage)
            => new ArgumentException($"{keyMessage} is existed.");
        public static Exception KeysExisted()
            => new ArgumentException("Both keys are existed.");
        public static Exception ArgumentOutOfRange(string paramName)
            => new ArgumentOutOfRangeException(paramName);
        public static Exception NotSupported()
            => new NotSupportedException();
        public static Exception ArrayTooSmall()
            => new ArgumentException("The array is too small.");
    }
}
