using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.Unsafe;

namespace Berrysoft.Unsafe
{
    /// <summary>
    /// Represents a .NET CLR pointer.
    /// </summary>
    /// <typeparam name="T">Type of the pointer targets to.</typeparam>
    /// <example>
    /// This example shows how to use pointer in Visual Basic.
    /// <code language="VB"><![CDATA[
    /// Imports Berrysoft.Unsafe.UnsafeMethods
    /// Module Program
    ///     Sub Main(args As String())
    ///         Dim int1 As Integer = 111
    ///         Console.WriteLine("Init an Int32: {0}", int1)
    ///         Dim ptr1 As Pointer(Of Integer) = PointerOf(int1)
    ///         Console.WriteLine("The address of the Int32: 0x{0}", ptr1.ToString("x16"))
    ///         TargetOf(ptr1) = 222
    ///         Console.WriteLine("Now the Int32 is: {0}", int1)
    ///         StackAlloc(Of Integer)(2,
    ///             Sub(ptr2)
    ///                 Console.WriteLine("Allocated two Int32: {0}, {1}", ptr2(0), ptr2(1))
    ///                 MemorySet(ptr2, &H33, 4)
    ///                 MemorySet(ptr2 + 1, &H22, 4)
    ///                 Console.WriteLine("Now they are: 0x{0}, 0x{1}", ptr2(0).ToString("x8"), ptr2(1).ToString("x8"))
    ///                 MemoryCopy(ptr1, ptr2, 4)
    ///             End Sub)
    ///         Console.WriteLine("Now the first Int32 is: 0x{0}", int1.ToString("x8"))
    ///     End Sub
    /// End Module
    /// ]]></code>
    /// </example>
    [DebuggerDisplay("{Ptr}")]
    [DebuggerTypeProxy(typeof(PointerDebugView<>))]
    public readonly unsafe struct Pointer<T> : IEquatable<Pointer<T>>
    {
        private readonly void* _ptr;
        /// <summary>
        /// Represents a <see langword="null"/> pointer.
        /// </summary>
        public static readonly Pointer<T> Null;
        /// <summary>
        /// Initialize a <see cref="Pointer{T}"/> with a <see cref="void"/> pointer.
        /// </summary>
        /// <param name="ptr">The pointer.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pointer(void* ptr)
        {
            _ptr = ptr;
        }
        /// <summary>
        /// Initialize a <see cref="Pointer{T}"/> with an <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="ptr">The pointer.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pointer(IntPtr ptr)
        {
            _ptr = (void*)ptr;
        }
        /// <summary>
        /// Initialize a <see cref="Pointer{T}"/> with a <typeparamref name="T"/> reference.
        /// </summary>
        /// <param name="ptr">The reference.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pointer(ref T ptr)
        {
            _ptr = AsPointer(ref ptr);
        }
        /// <summary>
        /// The contained pointer.
        /// </summary>
        public void* Ptr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _ptr;
        }
        /// <summary>
        /// The target reference of <see cref="Pointer{T}"/>.
        /// </summary>
        public ref T Target
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref AsRef<T>(_ptr);
        }
        /// <summary>
        /// The target reference of <see cref="Pointer{T}"/> with offset.
        /// </summary>
        /// <param name="index">The offset.</param>
        /// <returns>The target reference.</returns>
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Add(ref AsRef<T>(_ptr), index);
        }
        /// <summary>
        /// Adds offset to <see cref="Pointer{T}"/>.
        /// </summary>
        /// <param name="ptr">The pointer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>Added <see cref="Pointer{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<T> operator +(Pointer<T> ptr, int offset) => new Pointer<T>(ref Add(ref AsRef<T>(ptr._ptr), offset));
        /// <summary>
        /// Subtracts offset to <see cref="Pointer{T}"/>.
        /// </summary>
        /// <param name="ptr">The pointer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>Subtracted <see cref="Pointer{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<T> operator -(Pointer<T> ptr, int offset) => new Pointer<T>(ref Subtract(ref AsRef<T>(ptr._ptr), offset));
        /// <summary>
        /// Increses <see cref="Pointer{T}"/>.
        /// </summary>
        /// <param name="ptr">The pointer.</param>
        /// <returns>Incresed <see cref="Pointer{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<T> operator ++(Pointer<T> ptr) => ptr + 1;
        /// <summary>
        /// Decreses <see cref="Pointer{T}"/>.
        /// </summary>
        /// <param name="ptr">The pointer.</param>
        /// <returns>Decresed <see cref="Pointer{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<T> operator --(Pointer<T> ptr) => ptr - 1;
        /// <summary>
        /// Determines whether two <see cref="Pointer{T}"/> are equal.
        /// </summary>
        /// <param name="left">One pointer.</param>
        /// <param name="right">Another pointer.</param>
        /// <returns><see langword="true"/> if they are equal; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Pointer<T> left, Pointer<T> right) => left._ptr == right._ptr;
        /// <summary>
        /// Determines whether two <see cref="Pointer{T}"/> aren't equal.
        /// </summary>
        /// <param name="left">One pointer.</param>
        /// <param name="right">Another pointer.</param>
        /// <returns><see langword="true"/> if they aren't equap; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Pointer<T> left, Pointer<T> right) => left._ptr != right._ptr;
        /// <summary>
        /// Convert a <see cref="void"/> pointer to <see cref="Pointer{T}"/> implicitly.
        /// </summary>
        /// <param name="ptr">The <see cref="void"/> pointer.</param>
        /// <returns>A new <see cref="Pointer{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Pointer<T>(void* ptr) => new Pointer<T>(ptr);
        /// <summary>
        /// Convert a <see cref="Pointer{T}"/> to <see cref="void"/> pointer implicitly.
        /// </summary>
        /// <param name="ptr">The <see cref="Pointer{T}"/>.</param>
        /// <returns>The contained <see cref="void"/> pointer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator void*(Pointer<T> ptr) => ptr._ptr;
        /// <summary>
        /// Convert an <see cref="IntPtr"/> to <see cref="Pointer{T}"/> explicitly.
        /// </summary>
        /// <param name="ptr">The <see cref="IntPtr"/>.</param>
        /// <returns>A new <see cref="Pointer{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Pointer<T>(IntPtr ptr) => new Pointer<T>(ptr);
        /// <summary>
        /// Convert a <see cref="Pointer{T}"/> to <see cref="IntPtr"/> explicitly.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns>A new <see cref="IntPtr"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator IntPtr(Pointer<T> ptr) => (IntPtr)(ptr._ptr);
        /// <summary>
        /// Detenmines whether the pointer is <see langword="null"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the pointer is <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNull() => _ptr == null;
        /// <summary>
        /// Convert this pointer to a new pointer of <typeparamref name="TTo"/>.
        /// </summary>
        /// <typeparam name="TTo">The target type of new pointer.</typeparam>
        /// <returns>The new pointer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pointer<TTo> ToPointer<TTo>() => new Pointer<TTo>(_ptr);
        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance or null.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is an instance of <see cref="Pointer{T}"/> and equals the value of this instance; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Pointer<T> p)
            {
                return this == p;
            }
            return false;
        }
        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="Pointer{T}"/> value.
        /// </summary>
        /// <param name="other">An <see cref="Pointer{T}"/> value to compare to this instance.</param>
        /// <returns><see langword="true"/> if <paramref name="other"/> has the same value as this instance; otherwise, <see langword="false"/>.</returns>
        public bool Equals(Pointer<T> other) => this == other;
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => ((IntPtr)_ptr).GetHashCode();
        /// <summary>
        /// Converts the numeric value of the current <see cref="Pointer{T}"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the value of this instance.</returns>
        public override string ToString() => ((IntPtr)_ptr).ToString();
        /// <summary>
        /// Converts the numeric value of the current <see cref="Pointer{T}"/> object to its equivalent string representation.
        /// </summary>
        /// <param name="format">A format specification that governs how the current <see cref="Pointer{T}"/> object is converted.</param>
        /// <returns>The string representation of the value of the current <see cref="Pointer{T}"/> object.</returns>
        public string ToString(string format) => ((IntPtr)_ptr).ToString(format);
    }
}
