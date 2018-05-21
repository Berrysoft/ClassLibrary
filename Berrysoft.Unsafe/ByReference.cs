using System;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.Unsafe;

namespace Berrysoft.Unsafe
{
    public readonly unsafe struct ByReference<T>
    {
        private readonly void* _ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByReference(ref T value)
        {
            _ptr = AsPointer(ref value);
        }
        public ref T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref AsRef<T>(_ptr);
        }
    }
}
