Imports Berrysoft.Data
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class TreeTest

    <TestMethod()>
    Public Sub DepthTest()
        Dim tree As New Tree(Of Integer)(1)
        Dim node2 As New Node(Of Integer)(2)
        Dim node3 As New Node(Of Integer)(3)
        Dim node4 As New Node(Of Integer)(4)
        tree.Root.Add(node2)
        node2.Add(node3)
        node2.Add(node4)
        Assert.AreEqual(tree.GetDepth(), 3)
    End Sub

End Class