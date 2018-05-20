using System;
using static System.Runtime.CompilerServices.Unsafe;

namespace Berrysoft.Unsafe
{
    public unsafe readonly struct ByReference<T>
    {
        private readonly void* _ptr;
        public ByReference(ref T value)
        {
            _ptr = AsPointer(ref value);
        }
        public ref T Value => ref AsRef<T>(_ptr);
    }
}
