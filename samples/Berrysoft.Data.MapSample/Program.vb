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

        'This is a famous Chinese couplet of Temple of Lady Meng Jiangnu in Shanhaiguan.
        'The correct pronunciation of it is:
        'Hǎi  shuǐ  cháo   zhāo  zhāo  cháo   zhāo  cháo  zhāo  luò
        '海    水    朝  /  朝    朝    朝  /  朝    朝    朝    落
        'Fú   yún  zhǎng   cháng cháng zhǎng  cháng zhǎng cháng xiāo
        '浮    云    长  /  长    长    长  /  长    长    长    消

        '朝 means "day" when pronounces zhāo and means "tide" when pronounces cháo. Now the latter meaning is usually written as 潮.
        '长 means "grow" when pronounces zhǎng and means "always" when pronounces cháng. Now the latter meaning is usually written as 常.

        'This program means to replace the old multi-tone characters to their commonly used forms for easier understanding.

        Dim str = "海水朝朝朝朝朝朝朝落浮云长长长长长长长消".ToArray()
        Console.WriteLine(str)

        For i As Integer = 0 To str.Length - 1
            Dim pronounce = dictionary.GetValuesFromKey1(str(i))
            If pronounce.Count > 1 Then
                Console.Write("{0}" + vbTab, str(i))
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

        'If you enter the correct number: 0 1 1 0 1 0 1  0 1 1 0 1 0 1
        'You will get: 海水潮朝朝潮朝潮朝落浮云长常常长常长常消
        'Now it is easier to understand.
        Console.WriteLine(str)
    End Sub
End Module
