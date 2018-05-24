Module Program
    Sub Main(args As String())
        Dim int1 As Integer = 111
        Console.WriteLine("Init an Int32: {0}", int1)
        Dim ptr1 As Pointer(Of Integer) = UnsafeMethods.AddressOf(int1)
        Console.WriteLine("The address of the Int32: 0x{0}", ptr1.ToString("x16"))
        UnsafeMethods.TargetOf(ptr1) = 222
        Console.WriteLine("Now the Int32 is: {0}", int1)
        UnsafeMethods.StackAlloc(Of Integer)(2,
            Sub(ptr2)
                Console.WriteLine("Allocated two Int32: {0}, {1}", ptr2(0), ptr2(1))
                UnsafeMethods.MemorySet(ptr2, &H33, 4)
                UnsafeMethods.MemorySet(ptr2 + 1, &H22, 4)
                Console.WriteLine("Now they are: 0x{0}, 0x{1}", ptr2(0).ToString("x8"), ptr2(1).ToString("x8"))
                UnsafeMethods.MemoryCopy(ptr1, ptr2, 4)
            End Sub)
        Console.WriteLine("Now the first Int32 is: 0x{0}", int1.ToString("x8"))
    End Sub
End Module
