using System;

namespace Berrysoft.Html
{
    static class ExceptionHelper
    {
        public static Exception NotSupported()
            => new NotSupportedException();

        public static Exception NodeNotFound(string name)
            => new NodeNotFoundException(name);
    }

    public class NodeNotFoundException : Exception
    {
        public NodeNotFoundException(string name)
            : base($"Node {name} can't be found.")
        { }
    }
}
