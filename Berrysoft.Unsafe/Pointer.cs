using System;
using static System.Runtime.CompilerServices.Unsafe;

namespace Berrysoft.Unsafe
{
    public unsafe struct Pointer<T>
        where T : unmanaged
    {
        private T* _ptr;
        public Pointer(T* ptr)
        {
            _ptr = ptr;
        }
        public Pointer(ref T ptr)
        {
            _ptr = (T*)AsPointer(ref ptr);
        }
        public T* Ptr => _ptr;
        public IntPtr Handle => (IntPtr)_ptr;
        public ref T Target => ref _ptr[0];
        public ref T this[int index] => ref _ptr[index];
    }
}
