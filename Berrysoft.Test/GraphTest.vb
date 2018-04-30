Imports System.Text
Imports Berrysoft.Data
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class GraphTest

    <TestMethod()>
    Public Sub HeadTailTest()
        Dim graph As New Diagraph(Of Integer)
        graph.Add(123)
        graph.Add(456)
        graph.AddAsHead(789, 123, 456)
        graph.AddArc(123, 456)
        Dim i As Integer = 0
        Dim tails() As Integer = {123, 456}
        For Each tail In graph.GetTails(789)
            Assert.AreEqual(tail, tails(i))
            i += 1
        Next
    End Sub

End Class