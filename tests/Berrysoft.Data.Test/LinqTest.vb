Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class LinqTest

    <TestMethod()>
    Public Sub SelectWhereTest()
        Dim arr() As Integer = {1, 2, 3, 4, 5}
        Dim result() As Integer = arr.SelectWhen(
            Function(i)
                If i Mod 2 = 0 Then
                    Return (True, i * i)
                Else
                    Return (False, 0)
                End If
            End Function).ToArray()
        Assert.AreEqual(4, result(0))
        Assert.AreEqual(16, result(1))
    End Sub

    <TestMethod()>
    Public Sub ForEachTest()
        Dim arr As New List(Of Integer) From {2, 4, 6, 8, 10}
        arr.ForEach(Sub(i) Assert.IsTrue(i Mod 2 = 0))
    End Sub

End Class
