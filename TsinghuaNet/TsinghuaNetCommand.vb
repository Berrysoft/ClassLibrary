Imports Berrysoft.Console

Class TsinghuaNetCommand
    Inherits CommandLine
    Public Sub New(args() As String)
        MyBase.New(args)
    End Sub
    <[Option]("i"c, "login")>
    Public Property Login As Boolean
    <[Option]("o"c, "logout")>
    Public Property Logout As Boolean
    <[Option]("f"c, "flux")>
    Public Property Flux As Boolean
    <[Option]("u"c, "username")>
    Public Property Username As String
    <[Option]("p"c, "password")>
    Public Property Password As String
    <[Option]("s"c, "host")>
    Public Property Host As String
End Class
