Imports System.Json
Imports Berrysoft.Console
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class JsonSettingsTest
    Class StdSettingsClass
        Inherits JsonSettings

        <Settings("aaa", ConverterType:=GetType(AConverter))>
        Public Property A As Integer

        <Settings("bbb", ConverterType:=GetType(BConverter)), Multiple()>
        Public Property B As String()

        Class AConverter
            Implements ISimpleConverter

            Public Function Convert(value As Object) As Object Implements ISimpleConverter.Convert
                Dim jv As JsonPrimitive = value
                Return CInt(jv)
            End Function

            Public Function ConvertBack(value As Object) As Object Implements ISimpleConverter.ConvertBack
                Return value.ToString()
            End Function
        End Class

        Class BConverter
            Implements ISimpleConverter

            Public Function Convert(value As Object) As Object Implements ISimpleConverter.Convert
                Dim arr As JsonArray = value
                Return arr.Select(Function(a As JsonPrimitive) CStr(a)).ToArray()
            End Function

            Public Function ConvertBack(value As Object) As Object Implements ISimpleConverter.ConvertBack
                Dim arr As String() = value
                Return New JsonArray(arr.Select(Function(str) CType(str, JsonValue)).ToArray())
            End Function
        End Class
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
