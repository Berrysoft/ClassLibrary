Imports Berrysoft.Unsafe.UnsafeMethods
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class PointerTest

    <TestMethod()>
    Public Sub StackAllocTest()
        StackAllocByte(2,
            Sub(ptr)
                ptr(0) = 111
                ptr(1) = 222
                Assert.AreEqual(ptr.Target, CByte(111))
            End Sub)
    End Sub

    <TestMethod()>
    Public Sub MemoryTest()
        StackAllocByte(2,
            Sub(ptr)
                MemorySet(ptr, 100, 2)
                Assert.AreEqual(ptr(0), CByte(100))
                ptr += 1
                Assert.AreEqual(ptr(0), CByte(100))
                ptr.Target = 200
                StackAllocByte(1,
                    Sub(ptr2)
                        MemoryCopy(ptr2, ptr, 1)
                        Assert.AreEqual(ptr.Target, CByte(200))
                    End Sub)
            End Sub)
    End Sub

    <TestMethod()>
    Public Sub PointerTest()
        Dim i As Byte = 1
        Dim ptr = AddressOfByte(i)
        ptr.Target = 2
        Assert.AreEqual(i, CByte(2))
        TargetOfBytePtr(ptr) = 3
        Assert.AreEqual(i, CByte(3))
    End Sub

    <TestMethod()>
    Public Sub SizeTest()
        Assert.AreEqual(GetSize(Of Integer), 4)
    End Sub

End Class
