Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class ByRefTest

    <TestMethod()>
    Public Sub RefTest()
        Dim i As Integer = 100
        Dim refi As ByReference(Of Integer) = UnsafeMethods.ByRef(i)
        refi.Value = 200
        Assert.AreEqual(200, i)
    End Sub

    <TestMethod()>
    Public Sub AssignTest()
        Dim i As Integer = 1
        Dim ref1 As ByReference(Of Integer) = UnsafeMethods.ByRef(i)
        ref1.Value = 2
        Assert.AreEqual(2, i)
        Dim ref2 = UnsafeMethods.ByRef(ref1.Value)
        ref2.Value = 3
        Assert.AreEqual(3, i)
    End Sub

End Class
