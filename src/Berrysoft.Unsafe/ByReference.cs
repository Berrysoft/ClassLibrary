using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.Unsafe;

namespace Berrysoft.Unsafe
{
    /// <summary>
    /// Represents a .NET CLR reference.
    /// </summary>
    /// <typeparam name="T">Type of the reference pointed to.</typeparam>
    [DebuggerDisplay("{Value}")]
    [DebuggerTypeProxy(typeof(PointerDebugView<>))]
    public readonly unsafe struct ByReference<T>
    {
        private readonly void* _ptr;
        /// <summary>
        /// Initialize a <see cref="ByReference{T}"/> with a <typeparamref name="T"/> reference.
        /// </summary>
        /// <param name="value">The reference.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByReference(ref T value)
        {
            _ptr = AsPointer(ref value);
        }
        /// <summary>
        /// The target reference of <see cref="ByReference{T}"/>.
        /// </summary>
        public ref T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref AsRef<T>(_ptr);
        }
    }
    [DebuggerDisplay("{Value}")]
    [DebuggerTypeProxy(typeof(PointerDebugView<>))]
    public readonly unsafe struct ReadOnlyRefrence<T>
    {
        private readonly void* _ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyRefrence(in T value)
        {
            _ptr = AsPointer(ref AsRef(in value));
        }
        public ref readonly T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref AsRef<T>(_ptr);
        }
    }
}
