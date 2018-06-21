Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class GraphTest

    <TestMethod()>
    Public Sub HeadTailTest()
        Dim graph As New Graph(Of Integer)
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

    <TestMethod()>
    Public Sub DFSTest()
        Dim graph As New Graph(Of Integer)
        graph.Add(1)
        graph.Add(2)
        graph.Add(3)
        graph.Add(4)
        graph.Add(5)
        graph.Add(6)
        graph.Add(7)
        graph.Add(8)
        graph.AddEdge(1, 2)
        graph.AddEdge(1, 3)
        graph.AddEdge(2, 4)
        graph.AddEdge(2, 5)
        graph.AddEdge(3, 6)
        graph.AddEdge(3, 7)
        graph.AddEdge(6, 7)
        'graph.AddEdge(4, 8)
        'graph.AddEdge(5, 8)
        graph.AddAsHead(8, 4, 5)
        Dim dfsarray() As Integer = graph.AsDFSEnumerable(1).ToArray()
        Dim expect() As Integer = {1, 2, 4, 8, 5, 3, 6, 7}
        For i = 0 To 7
            Assert.AreEqual(dfsarray(i), expect(i))
        Next
    End Sub

    <TestMethod()>
    Public Sub BFSTest()
        Dim graph As New Graph(Of Integer)
        graph.Add(1)
        graph.Add(2)
        graph.Add(3)
        graph.Add(4)
        graph.Add(5)
        graph.Add(6)
        graph.Add(7)
        graph.Add(8)
        graph.AddEdge(1, 2)
        graph.AddEdge(1, 3)
        graph.AddEdge(2, 4)
        graph.AddEdge(2, 5)
        graph.AddEdge(3, 6)
        graph.AddEdge(3, 7)
        graph.AddEdge(6, 7)
        graph.AddEdge(4, 8)
        graph.AddEdge(5, 8)
        Dim bfsarray() As Integer = graph.AsBFSEnumerable(1).ToArray()
        Dim expect() As Integer = {1, 2, 3, 4, 5, 6, 7, 8}
        For i = 0 To 7
            Assert.AreEqual(bfsarray(i), expect(i))
        Next
    End Sub

    <TestMethod()>
    Public Sub DFSTreeTest()
        Dim graph As New Graph(Of Integer)
        graph.Add(1)
        graph.Add(2)
        graph.Add(3)
        graph.Add(4)
        graph.Add(5)
        graph.Add(6)
        graph.Add(7)
        graph.Add(8)
        graph.AddEdge(1, 2)
        graph.AddEdge(1, 3)
        graph.AddEdge(2, 4)
        graph.AddEdge(2, 5)
        graph.AddEdge(3, 6)
        graph.AddEdge(3, 7)
        graph.AddEdge(6, 7)
        graph.AddEdge(4, 8)
        graph.AddEdge(5, 8)
        Dim tree = graph.ToDFSTree(1)
        Assert.AreEqual(tree.GetDepth(), 5)
    End Sub

    <TestMethod()>
    Public Sub BFSTreeTest()
        Dim graph As New Graph(Of Integer)
        graph.Add(1)
        graph.Add(2)
        graph.Add(3)
        graph.Add(4)
        graph.Add(5)
        graph.Add(6)
        graph.Add(7)
        graph.Add(8)
        graph.AddEdge(1, 2)
        graph.AddEdge(1, 3)
        graph.AddEdge(2, 4)
        graph.AddEdge(2, 5)
        graph.AddEdge(3, 6)
        graph.AddEdge(3, 7)
        graph.AddEdge(6, 7)
        graph.AddEdge(4, 8)
        graph.AddEdge(5, 8)
        Dim tree = graph.ToBFSTree(1)
        Assert.AreEqual(tree.GetDepth(), 4)
    End Sub

    <TestMethod()>
    Public Sub DFSPathTest()
        Dim graph As New Graph(Of Integer)
        graph.Add(1)
        graph.Add(2)
        graph.Add(3)
        graph.Add(4)
        graph.Add(5)
        graph.Add(6)
        graph.Add(7)
        graph.Add(8)
        graph.AddEdge(1, 2)
        graph.AddEdge(1, 3)
        graph.AddEdge(2, 4)
        graph.AddEdge(2, 5)
        graph.AddEdge(3, 6)
        graph.AddEdge(3, 7)
        graph.AddEdge(6, 7)
        graph.AddAsHead(8, 4, 5)
        Dim expect() As Integer = {1, 3, 6}
        For Each item In graph.AsDFSWithPath(1)
            If item.Vertex = 6 Then
                For i = 0 To 2
                    Assert.AreEqual(expect(i), item.Path(i))
                Next
                Exit For
            End If
        Next
    End Sub

    <TestMethod()>
    Public Sub LoopTest()
        Dim graph As New Graph(Of Integer)
        graph.Add(1)
        graph.Add(2)
        graph.Add(4)
        graph.Add(5)
        graph.AddEdge(1, 2)
        graph.AddEdge(2, 4)
        graph.AddEdge(2, 5)
        graph.AddAsHead(8, 4, 5)
        graph.AddAsTail(8, 4, 5)
        Assert.IsTrue(graph.IsInLoop(8))
        Assert.IsFalse(graph.IsInLoop(1))
    End Sub

End Class