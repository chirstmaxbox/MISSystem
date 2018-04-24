Imports System.Text.RegularExpressions
Imports System.Web

Public Class VbHtml

    Private Shared Function VbIsNullString(ByVal str As Object) As Boolean
        Dim b As Boolean = False
        If IsDBNull(str) Then
            b = True
        ElseIf str Is Nothing Then
            b = True
        ElseIf String.IsNullOrEmpty(str) Then
            b = True
        ElseIf "" = CStr(str) Or " " = CStr(str) Or "&nbsp;" = CStr(str) Then
            b = True
        ElseIf Len(str.ToString.Trim) = 0 Then
            b = True
        End If
        Return b
    End Function

    Public Shared Function MyHtmlDecode(ByVal inHTML As Object) As String
        Dim s As String = ""
        If Not VbIsNullString(inHTML) Then
            s = HttpContext.Current.Server.HtmlDecode(Regex.Replace(inHTML, "<(.|\n)*?>", ""))
        End If
        Return s
    End Function

    Public Shared Function ReplaceInviladCharacterInUrl(inHtmlString As Object) As String
        Dim s1 = MyHtmlDecode(inHtmlString)
        Dim aTemp As [String]() = {"<", "(", ".", "|", """, '*", "?", ">", "&", "'"}

        If s1.Length > 0 Then

            Dim count As Integer = aTemp.Length - 1

            For i As Integer = 0 To count
                If s1.Contains(aTemp(i)) Then
                    s1 = s1.Replace(aTemp(i), " ")
                End If
            Next
        End If
        Return s1
    End Function

End Class
