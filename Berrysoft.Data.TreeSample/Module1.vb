Module Module1
    Private tree As New BinaryTree(Of Integer)
    Private current As BinaryNode(Of Integer)

    Sub Main()
        Console.Write("How many groups do you want to test: ")
        Dim n As Integer = Console.ReadLine()
        For nn = 1 To n
            Console.Write("How many nodes: ")
            Dim m As Integer = Console.ReadLine()
            Console.WriteLine("Please enter the values by pre order, splited by space:")
            Dim front() As Integer = Console.ReadLine().Split(" "c).Select(Function(str) CInt(str)).ToArray()
            Console.WriteLine("Please enter the values by in order, splited by space:")
            Dim mid() As Integer = Console.ReadLine().Split(" "c).Select(Function(str) CInt(str)).ToArray()
            tree.Root.LeftChild = Nothing
            tree.Root.RightChild = Nothing
            current = tree.Root
            Create(front, mid, m)
            Console.WriteLine("The post order of this tree is:")
            For Each node In tree.AsPostOrderEnumerable()
                Console.Write("{0} ", node.Value)
            Next
            Console.WriteLine()
        Next
    End Sub

    Sub Create(front() As Integer, mid() As Integer, n As Integer)
        Dim tr As BinaryNode(Of Integer) = current
        tr.Value = front(0)
        Dim i As Integer
        For i = 0 To n - 1
            If mid(i) = front(0) Then
                Exit For
            End If
        Next
        If i > 0 Then
            Dim nl As New BinaryNode(Of Integer)
            tr.LeftChild = nl
            current = nl
            Create(front.Skip(1).ToArray(), mid, i)
        End If
        If n - 1 - i > 0 Then
            Dim nr As New BinaryNode(Of Integer)
            tr.RightChild = nr
            current = nr
            Create(front.Skip(i + 1).ToArray(), mid.Skip(i + 1).ToArray(), n - 1 - i)
        End If
    End Sub

End Module
