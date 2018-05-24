Imports System.Json
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class JsonSettingsTest
    Class StdSettingsClass
        Inherits JsonSettings

        <Settings("aaa")>
        Public Property A As Integer

        <Settings("bbb", AllowMultiple:=True)>
        Public Property B As String()

        Protected Overrides Function ChangeType(name As String, value As JsonValue, conversionType As Type) As Object
            If name = "bbb" Then
                Dim jsarr As JsonArray = value
                Dim array As String() = jsarr.Select(Function(v) CStr(CType(v, JsonPrimitive))).ToArray()
                Return array
            ElseIf name = "aaa" Then
                Dim jsp As JsonPrimitive = value
                Return CInt(jsp)
            Else
                Return Convert.ChangeType(value, conversionType)
            End If
        End Function

        Protected Overrides Function ChangeBackType(name As String, value As Object, conversionType As Type) As JsonValue
            If name = "bbb" Then
                Dim array As String() = value
                Return New JsonArray(array.Select(Function(s) New JsonPrimitive(s)))
            ElseIf name = "aaa" Then
                Return CInt(value)
            Else
                Return value.ToString()
            End If
        End Function
    End Class
    Private Const FileName As String = "jssettings.json"
    <TestMethod()>
    Public Sub StdSettingsTest()
        Dim settings As New StdSettingsClass()
        settings.Open(FileName)
        Assert.AreEqual(settings.A, 123)
        Assert.AreEqual(settings.B(0), "a")
        Assert.AreEqual(settings.B(1), "b")
        settings.B(0) = "hhh"
        settings.Save(FileName)
        settings.B(0) = "asdf"
        settings.Open(FileName)
        Assert.AreEqual(settings.B(0), "hhh")
        settings.B(0) = "a"
        settings.Save(FileName)
    End Sub

End Class
