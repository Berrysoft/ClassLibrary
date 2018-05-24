Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class WorkflowTest

    <TestMethod()>
    Public Sub IfTest()
        Dim i As Integer = 0
        Dim trueFunc As Func(Of IExecutable) =
            Function()
                Assert.AreEqual(i, 0)
                Return Nothing
            End Function
        Dim falseFunc As Func(Of IExecutable) =
            Function()
                Assert.AreEqual(i, 1)
                Return Nothing
            End Function
        Dim condition As New Condition(Function() i = 0, trueFunc, falseFunc)
        condition.Work()
        i = 1
        condition.Work()
    End Sub

End Class
