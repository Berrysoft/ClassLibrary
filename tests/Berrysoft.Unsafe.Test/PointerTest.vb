Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Berrysoft.Unsafe.UnsafeMethods

<TestClass()>
Public Class PointerTest

    <TestMethod()>
    Public Sub StackAllocTest()
        StackAlloc(Of Byte)(2,
            Sub(ptr)
                ptr(0) = 111
                ptr(1) = 222
                Assert.AreEqual(ptr.Target, CByte(111))
            End Sub)
    End Sub

    <TestMethod()>
    Public Sub MemoryTest()
        StackAlloc(Of Byte)(2,
            Sub(ptr)
                MemorySet(ptr, 100, 2)
                Assert.AreEqual(ptr(0), CByte(100))
                ptr += 1
                Assert.AreEqual(ptr(0), CByte(100))
                ptr.Target = 200
                StackAlloc(Of Byte)(1,
                    Sub(ptr2)
                        MemoryCopy(ptr2, ptr, 1)
                        Assert.AreEqual(ptr.Target, CByte(200))
                    End Sub)
            End Sub)
    End Sub

    <TestMethod()>
    Public Sub PointerTest()
        Dim i As Byte = 1
        Dim ptr = PointerOf(i)
        ptr.Target = 2
        Assert.AreEqual(i, CByte(2))
        TargetOf(ptr) = 3
        Assert.AreEqual(i, CByte(3))
    End Sub

    <TestMethod()>
    Public Sub SizeTest()
        Assert.AreEqual(GetSize(Of Integer), 4)
    End Sub

    <TestMethod()>
    Public Sub CalTest()
        StackAlloc(Of Short)(2,
            Sub(ptr)
                ptr(0) = 2
                ptr(1) = 1
                Dim lptr As New Pointer(Of Integer)(ptr)
                Assert.AreEqual(lptr.Target, 65538)
            End Sub)
    End Sub

    <TestMethod()>
    Public Sub ArrayTest()
        Dim arr As Integer() = {1, 2, 3}
        Dim ptr As Pointer(Of Integer) = PointerOf(arr)
        ptr(1) = 222
        Assert.AreEqual(arr(1), 222)
    End Sub

    <TestMethod()>
    Public Sub SpanTest()
        StackAlloc(Of Integer)(2,
            Sub(ptr)
                Dim span = ptr.AsSpan(2)
                span(0) = 123
                span(1) = 456
                Assert.AreEqual(ptr.Target, 123)
                Assert.AreEqual(ptr(1), 456)
            End Sub)
    End Sub

End Class
