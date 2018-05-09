using System;
using static System.Runtime.CompilerServices.Unsafe;

namespace Berrysoft.Unsafe
{
    public unsafe static class UnsafeMethods
    {
        public static int GetSize<T>()
        {
            return SizeOf<T>();
        }
        public static Pointer<T> AddressOf<T>(ref T value)
            where T : unmanaged
        {
            return new Pointer<T>(ref value);
        }
        public static Pointer<byte> AddressOfByte(ref byte value) => AddressOf(ref value);
        public static ref T TargetOf<T>(Pointer<T> ptr)
            where T : unmanaged
        {
            return ref ptr.Target;
        }
        public static ref byte TargetOfBytePtr(Pointer<byte> ptr) => ref TargetOf(ptr);
        public static void StackAlloc<T>(int size, Action<Pointer<T>> action)
            where T : unmanaged
        {
            T* ptr = stackalloc T[size];
            action(new Pointer<T>(ptr));
        }
        public static void StackAllocSByte(int size, Action<Pointer<sbyte>> action) => StackAlloc(size, action);
        public static void StackAllocByte(int size, Action<Pointer<byte>> action) => StackAlloc(size, action);
        public static void StackAllocInt16(int size, Action<Pointer<short>> action) => StackAlloc(size, action);
        public static void StackAllocUInt16(int size, Action<Pointer<ushort>> action) => StackAlloc(size, action);
        public static void MemorySet(IntPtr startAddress, byte value, uint byteCount) => InitBlock((void*)startAddress, value, byteCount);
        public static void MemoryCopy(IntPtr destination, IntPtr source, uint byteCount) => CopyBlock((void*)destination, (void*)source, byteCount);
    }
}
