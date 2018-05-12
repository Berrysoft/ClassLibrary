using System;
using System.Collections.Generic;
using System.Text;

namespace Berrysoft.Data
{
    static class ExceptionHelper
    {
        public static Exception ArgumentNull(string paramName)
        {
            return new ArgumentNullException(paramName);
        }
        public static Exception RootHasParent()
        {
            return new ArgumentException("The root can't have a parent.");
        }
        public static Exception PairExisted()
        {
            return new ArgumentException("The key pair is existed.");
        }
        public static Exception KeyNotFound()
        {
            return new KeyNotFoundException();
        }
        public static Exception KeyExisted(string keyMessage)
        {
            return new ArgumentException($"{keyMessage} is existed.");
        }
        public static Exception KeysExisted()
        {
            return new ArgumentException("Both keys are existed.");
        }
    }
}
