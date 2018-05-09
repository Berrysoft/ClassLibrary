using System;
using static System.Runtime.CompilerServices.Unsafe;

namespace Berrysoft.Unsafe
{
    public unsafe readonly struct Pointer<T>
        where T : unmanaged
    {
        private readonly T* _ptr;
        public Pointer(T* ptr)
        {
            _ptr = ptr;
        }
        public Pointer(IntPtr ptr)
        {
            _ptr = (T*)ptr;
        }
        public Pointer(ref T ptr)
        {
            _ptr = (T*)AsPointer(ref ptr);
        }
        public T* Ptr => _ptr;
        public ref T Target => ref _ptr[0];
        public ref T this[int index] => ref _ptr[index];
        public static Pointer<T> operator +(Pointer<T> ptr, int offset) => new Pointer<T>(&ptr._ptr[offset]);
        public static Pointer<T> operator -(Pointer<T> ptr, int offset) => new Pointer<T>(&ptr._ptr[-offset]);
        public static Pointer<T> operator ++(Pointer<T> ptr)
        {
            T* p = ptr._ptr;
            return new Pointer<T>(++p);
        }
        public static Pointer<T> operator --(Pointer<T> ptr)
        {
            T* p = ptr._ptr;
            return new Pointer<T>(--p);
        }
        public static implicit operator Pointer<T>(T* ptr) => new Pointer<T>(ptr);
        public static implicit operator T*(Pointer<T> ptr) => ptr._ptr;
        public static implicit operator Pointer<T>(IntPtr ptr) => new Pointer<T>(ptr);
        public static implicit operator IntPtr(Pointer<T> ptr) => (IntPtr)(ptr._ptr);
    }
}
