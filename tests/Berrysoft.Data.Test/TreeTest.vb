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

    <TestMethod()>
    Public Sub SearchTest()
        Dim tree As New Tree(Of Integer)(1)
        Dim node2 As New Node(Of Integer)(2)
        Dim node3 As New Node(Of Integer)(3)
        Dim node4 As New Node(Of Integer)(4)
        tree.Root.Add(node2)
        node2.Add(node3)
        node2.Add(node4)
        Dim node5 As New Node(Of Integer)(5)
        tree.Root.Add(node5)
        Dim exdfsArray = {1, 2, 3, 4, 5}
        Dim dfsArray = tree.AsDFSEnumerable().ToArray()
        For i = 0 To 4
            Assert.AreEqual(exdfsArray(i), dfsArray(i).Value)
        Next
    End Sub

End Class