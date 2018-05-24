using System;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.Unsafe;

namespace Berrysoft.Unsafe
{
    public unsafe static class UnsafeMethods
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSize<T>() => SizeOf<T>();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<T> AddressOf<T>(ref T value) => new Pointer<T>(ref value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<T> AddressOf<T>(T[] array) => new Pointer<T>(ref array[0]);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ByReference<T> ByRef<T>(ref T value) => new ByReference<T>(ref value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T TargetOf<T>(Pointer<T> ptr) => ref ptr.Target;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StackAlloc<T>(int size, Action<Pointer<T>> action)
        {
            byte* ptr = stackalloc byte[size * SizeOf<T>()];
            action(new Pointer<T>(ptr));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> AsSpan<T>(this Pointer<T> ptr, int size) => new Span<T>(ptr.Ptr, size);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this Pointer<T> ptr, int size) => new ReadOnlySpan<T>(ptr.Ptr, size);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MemorySet(IntPtr startAddress, byte value, uint byteCount) => InitBlock((void*)startAddress, value, byteCount);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MemorySet<T>(Pointer<T> startAddress, byte value, uint byteCount) => InitBlock(startAddress, value, byteCount);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MemoryCopy(IntPtr destination, IntPtr source, uint byteCount) => CopyBlock((void*)destination, (void*)source, byteCount);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MemoryCopy<TDestination, TSource>(Pointer<TDestination> destination, Pointer<TSource> source, uint byteCount) => CopyBlock(destination, source, byteCount);
    }
}
