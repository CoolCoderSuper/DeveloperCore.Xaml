Imports DeveloperCore.Xaml

Public Module Program
    Public Sub Main(args As String())
        Dim x As String = "<ConsoleApp1.Thing Value=""1""><Name>Thing1</Name><Thing><ConsoleApp1.Thing Name=""Thing2"" Value=""2"" /></Thing></ConsoleApp1.Thing>"
        Dim thing As Thing = Parser.Parse(x)
    End Sub
End Module

Public Class Thing
    Public Property Name As String
    Public Property Value As Integer
    Public Property Thing As Thing
End Class