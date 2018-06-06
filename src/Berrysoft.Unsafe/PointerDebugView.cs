using System;
using System.Diagnostics;

namespace Berrysoft.Unsafe
{
    internal sealed class PointerDebugView<T>
    {
        private readonly T target;
        public unsafe PointerDebugView(Pointer<T> ptr)
        {
            if (ptr.Ptr != null)
            {
                target = ptr.Target;
            }
            else
            {
                target = default;
            }
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public T Target => target;
    }
}
