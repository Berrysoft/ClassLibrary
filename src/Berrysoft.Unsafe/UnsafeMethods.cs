using System;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.Unsafe;

namespace Berrysoft.Unsafe
{
    /// <summary>
    /// Contains generic, low-level functionality for <see cref="Pointer{T}"/> and <see cref="Unsafe.ByReference{T}"/>.
    /// </summary>
    public unsafe static class UnsafeMethods
    {
        /// <summary>
        /// Get size of a <typeparamref name="T"/> instance.
        /// </summary>
        /// <typeparam name="T">The type to get size.</typeparam>
        /// <returns>The size of an instance if <typeparamref name="T"/> is value type; otherwise, the size of pointer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSize<T>() => SizeOf<T>();
        /// <summary>
        /// Get a <see cref="Pointer{T}"/> with a <typeparamref name="T"/> reference.
        /// Note: the storage of value should be pinned.
        /// </summary>
        /// <typeparam name="T">Type of the pointer pointed to.</typeparam>
        /// <param name="value">The reference.</param>
        /// <returns>An instance of <see cref="Pointer{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<T> PointerOf<T>(ref T value) => new Pointer<T>(ref value);
        /// <summary>
        /// Get a <see cref="Pointer{T}"/> of an array.
        /// </summary>
        /// <typeparam name="T">Type of the pointer pointed to.</typeparam>
        /// <param name="array">The array.</param>
        /// <returns>An instance of <see cref="Pointer{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<T> PointerOf<T>(T[] array) => new Pointer<T>(ref array[0]);
        /// <summary>
        /// Get a <see cref="Pointer{T}"/> of a <see cref="string"/>.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>An instance of <see cref="Pointer{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<char> PointerOf(string str)
        {
            fixed (char* p = str)
            {
                return new Pointer<char>(p);
            }
        }
        /// <summary>
        /// Get a <see cref="Unsafe.ByReference{T}"/> with a <typeparamref name="T"/> reference.
        /// Note: the storage of value should be pinned.
        /// </summary>
        /// <typeparam name="T">Type of the pointer pointed to.</typeparam>
        /// <param name="value">The reference.</param>
        /// <returns>An instance of <see cref="Unsafe.ByReference{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ByReference<T> ByReference<T>(ref T value) => new ByReference<T>(ref value);
        /// <summary>
        /// Get a <see cref="Unsafe.ByReference{T}"/> with a readonly <typeparamref name="T"/> reference.
        /// Note: the storage of value should be pinned.
        /// </summary>
        /// <typeparam name="T">Type of the pointer pointed to.</typeparam>
        /// <param name="value">The reference.</param>
        /// <returns>An instance of <see cref="Unsafe.ByReference{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ByReference<T> GetRef<T>(in T value) => new ByReference<T>(ref AsRef(in value));
        /// <summary>
        /// Get the target reference of <see cref="Pointer{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the pointer pointed to.</typeparam>
        /// <param name="ptr">The <see cref="Pointer{T}"/>.</param>
        /// <returns>The target reference of <see cref="Pointer{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T TargetOf<T>(Pointer<T> ptr) => ref ptr.Target;
        /// <summary>
        /// Alloc an array on stack and do <paramref name="action"/> with the pointer.
        /// </summary>
        /// <typeparam name="T">Type of the pointer pointed to.</typeparam>
        /// <param name="size">Size of the array.</param>
        /// <param name="action">An <see cref="Action{T}"/> delegate do with the pointer.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StackAlloc<T>(int size, Action<Pointer<T>> action)
        {
            byte* ptr = stackalloc byte[size * SizeOf<T>()];
            action(new Pointer<T>(ptr));
        }
        /// <summary>
        /// Get a <see cref="Span{T}"/> of a <see cref="Pointer{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the pointer pointed to.</typeparam>
        /// <param name="ptr">The <see cref="Pointer{T}"/>.</param>
        /// <param name="size">Size of the <see cref="Span{T}"/>.</param>
        /// <returns>A <see cref="Span{T}"/> of a <see cref="Pointer{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> AsSpan<T>(this Pointer<T> ptr, int size) => new Span<T>(ptr.Ptr, size);
        /// <summary>
        /// Get a <see cref="ReadOnlySpan{T}"/> of a <see cref="Pointer{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the pointer pointed to.</typeparam>
        /// <param name="ptr">The <see cref="Pointer{T}"/>.</param>
        /// <param name="size">Size of the <see cref="ReadOnlySpan{T}"/>.</param>
        /// <returns>A <see cref="ReadOnlySpan{T}"/> of a <see cref="Pointer{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this Pointer<T> ptr, int size) => new ReadOnlySpan<T>(ptr.Ptr, size);
        /// <summary>
        /// Initializes a block of memory at the given location with a given initial value.
        /// </summary>
        /// <param name="startAddress">The address of the start of the memory block to initialize.</param>
        /// <param name="value">The value to initialize the block to.</param>
        /// <param name="byteCount">The number of bytes to initialize.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MemorySet(IntPtr startAddress, byte value, uint byteCount) => InitBlock((void*)startAddress, value, byteCount);
        /// <summary>
        /// Copies bytes from the <paramref name="source"/> address to the <paramref name="destination"/> address.
        /// </summary>
        /// <param name="destination">The destination address to copy to.</param>
        /// <param name="source">The source address to copy from.</param>
        /// <param name="byteCount">The number of bytes to copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MemoryCopy(IntPtr destination, IntPtr source, uint byteCount) => CopyBlock((void*)destination, (void*)source, byteCount);
    }
}
