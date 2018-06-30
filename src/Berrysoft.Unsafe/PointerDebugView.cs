using System;
using System.Diagnostics;

namespace Berrysoft.Unsafe
{
    /// <summary>
    /// Represents a debug view for <see cref="Pointer{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the pointer targets to.</typeparam>
    internal sealed class PointerDebugView<T>
    {
        private readonly T target;
        /// <summary>
        /// Initializes a new instance of <see cref="PointerDebugView{T}"/> class. This method is called by the debugger.
        /// </summary>
        /// <param name="ptr">The pointer.</param>
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
        /// <summary>
        /// Initializes a new instance of <see cref="PointerDebugView{T}"/> class. This method is called by the debugger.
        /// </summary>
        /// <param name="ptr">The reference.</param>
        public PointerDebugView(ByReference<T> ptr)
        {
            target = ptr.Value;
        }
        public PointerDebugView(ReadOnlyRefrence<T> ptr)
        {
            target = ptr.Value;
        }
        /// <summary>
        /// Target of the pointer.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public T Target => target;
    }
}
