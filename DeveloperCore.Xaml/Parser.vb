Imports System.Reflection
Imports System.Xml

Public Class Parser
    Public Shared Function Parse(xaml As String)
        Dim doc As New XmlDocument()
        doc.LoadXml(xaml)
        Dim root As XmlElement = doc.DocumentElement
        Dim obj As Object = ParseElement(root)
        Return obj
    End Function
    
    Private Shared Function ParseElement(element As XmlElement) As Object
        Dim typeName As String = element.LocalName
        Dim type As Type = TypeResolver.Resolve(typeName)
        Dim obj As Object = Activator.CreateInstance(type)
        For Each attribute As XmlAttribute In element.Attributes
            Dim propertyName As String = attribute.LocalName
            Dim propertyInfo As PropertyInfo = type.GetProperty(propertyName)
            Dim propertyValue As Object = ParseValue(propertyInfo.PropertyType, attribute.Value)
            propertyInfo.SetValue(obj, propertyValue)
        Next
        For Each child As XmlNode In element.ChildNodes
            Dim el As XmlElement = DirectCast(child, XmlElement)
            Dim propertyName As String = child.LocalName
            Dim propertyInfo As PropertyInfo = type.GetProperty(propertyName)
            If TypeOf child.FirstChild Is XmlElement Then
                Dim propertyValue As Object = ParseElement(el.FirstChild)
                propertyInfo.SetValue(obj, propertyValue)
            ElseIf TypeOf child.FirstChild Is XmlText Then
                propertyInfo.SetValue(obj, ParseValue(propertyInfo.PropertyType, el.FirstChild.Value))
            End If
        Next
        Return obj
    End Function
    
    Private Shared Function ParseValue(type As Type, value As String) As Object
        If type Is GetType(String) Then
            Return value
        ElseIf type Is GetType(Integer) Then
            Return Integer.Parse(value)
        ElseIf type Is GetType(Double) Then
            Return Double.Parse(value)
        ElseIf type Is GetType(Boolean) Then
            Return Boolean.Parse(value)
        End If
    End Function
End Class

Public Class TypeResolver
    Public Shared Function Resolve(typeName As String) As Type
        Dim t As Type = ByName(typeName)
        Return t
    End Function
    
    Private Shared Function ByName(ByVal name As String) As Type
        Return AppDomain.CurrentDomain.GetAssemblies().Reverse().[Select](Function(assembly) assembly.GetType(name)).FirstOrDefault(Function(t) t IsNot Nothing)
    End Function
End Class