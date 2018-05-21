using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.Unsafe;

namespace Berrysoft.Unsafe
{
    [DebuggerDisplay("{Ptr}")]
    public unsafe readonly struct Pointer<T> : IEquatable<Pointer<T>>
    {
        private readonly void* _ptr;
        public static readonly Pointer<T> Null;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pointer(void* ptr)
        {
            _ptr = ptr;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pointer(IntPtr ptr)
        {
            _ptr = (void*)ptr;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pointer(ref T ptr)
        {
            _ptr = AsPointer(ref ptr);
        }
        public void* Ptr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _ptr;
        }
        public ref T Target
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref AsRef<T>(_ptr);
        }

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Add(ref AsRef<T>(_ptr), index);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<T> operator +(Pointer<T> ptr, int offset) => new Pointer<T>(ref Add(ref AsRef<T>(ptr._ptr), offset));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<T> operator -(Pointer<T> ptr, int offset) => new Pointer<T>(ref Subtract(ref AsRef<T>(ptr._ptr), offset));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<T> operator ++(Pointer<T> ptr) => ptr + 1;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<T> operator --(Pointer<T> ptr) => ptr - 1;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Pointer<T> left, Pointer<T> right) => left._ptr == right._ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Pointer<T> left, Pointer<T> right) => left._ptr != right._ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Pointer<T>(void* ptr) => new Pointer<T>(ptr);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator void*(Pointer<T> ptr) => ptr._ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Pointer<T>(IntPtr ptr) => new Pointer<T>(ptr);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator IntPtr(Pointer<T> ptr) => (IntPtr)(ptr._ptr);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNull() => _ptr == null;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pointer<TTo> ToPointer<TTo>() => new Pointer<TTo>(_ptr);
        public override bool Equals(object obj)
        {
            if (obj is Pointer<T> p)
            {
                return this == p;
            }
            return false;
        }
        public bool Equals(Pointer<T> other) => this == other;
        public override int GetHashCode() => ((IntPtr)_ptr).GetHashCode();
        public override string ToString() => ((IntPtr)_ptr).ToString();
        public string ToString(string format) => ((IntPtr)_ptr).ToString(format);
    }
}
