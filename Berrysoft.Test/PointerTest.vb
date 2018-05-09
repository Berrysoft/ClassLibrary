Imports Berrysoft.Unsafe
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class PointerTest

    <TestMethod()>
    Public Sub StackAllocTest()
        UnsafeMethods.StackAlloc(Of Byte)(2,
            Sub(ptr)
                ptr(0) = 111
                ptr(1) = 222
                Assert.AreEqual(ptr.Target, CByte(111))
            End Sub)
    End Sub

    <TestMethod()>
    Public Sub MemoryTest()
        UnsafeMethods.StackAlloc(Of Byte)(2,
            Sub(ptr)
                UnsafeMethods.MemorySet(ptr, 100, 2)
                Assert.AreEqual(ptr(0), CByte(100))
                ptr += 1
                Assert.AreEqual(ptr(0), CByte(100))
                ptr.Target = 200
                UnsafeMethods.StackAlloc(Of Byte)(1,
                    Sub(ptr2)
                        UnsafeMethods.MemoryCopy(ptr2, ptr, 1)
                        Assert.AreEqual(ptr.Target, CByte(200))
                    End Sub)
            End Sub)
    End Sub

    <TestMethod()>
    Public Sub PointerTest()
        Dim i As Byte = 1
        Dim ptr = UnsafeMethods.AddressOf(i)
        ptr.Target = 2
        Assert.AreEqual(i, CByte(2))
        UnsafeMethods.TargetOf(ptr) = 3
        Assert.AreEqual(i, CByte(3))
    End Sub

    <TestMethod()>
    Public Sub SizeTest()
        Assert.AreEqual(UnsafeMethods.GetSize(Of Integer), 4)
    End Sub

    <TestMethod()>
    Public Sub CalTest()
        UnsafeMethods.StackAlloc(Of Short)(2,
            Sub(ptr)
                ptr(0) = 2
                ptr(1) = 1
                Dim lptr As New Pointer(Of Integer)(ptr)
                Assert.AreEqual(lptr.Target, 65538)
            End Sub)
    End Sub

End Class
