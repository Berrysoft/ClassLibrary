Module Program
    Private dictionary As New MultiMap(Of Char, String)
    Sub Main(args As String())
        dictionary.Add("海"c, "hai")
        dictionary.Add("水"c, "shui")
        dictionary.Add("朝"c, "chao")
        dictionary.Add("朝"c, "zhao")
        dictionary.Add("落"c, "luo")

        dictionary.Add("浮"c, "fu")
        dictionary.Add("云"c, "yun")
        dictionary.Add("长"c, "zhang")
        dictionary.Add("长"c, "chang")
        dictionary.Add("消"c, "xiao")

        dictionary.Add("潮"c, "chao")
        dictionary.Add("常"c, "chang")

        Dim str = "海水朝朝朝朝朝朝朝落浮云长长长长长长长消".ToArray()
        Console.WriteLine(str)

        For i As Integer = 0 To str.Length - 1
            Dim pronounce = dictionary.GetValuesFromKey1(str(i))
            If pronounce.Count > 1 Then
                For j As Integer = 0 To pronounce.Count - 1
                    Console.Write("{0} {1}" + vbTab, j, pronounce(j))
                Next
                Console.WriteLine()
                Dim index As Integer = Console.ReadLine()
                Dim character = dictionary.GetValuesFromKey2(pronounce(index))
                If character.Count > 1 Then
                    str(i) = character.First(Function(c) c <> str(i))
                End If
            End If
        Next

        Console.WriteLine(str)
    End Sub
End Module
