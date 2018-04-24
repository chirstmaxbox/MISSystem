Imports System.Web.UI
Imports System.Web

Public Class MyAlert
    Public Shared Sub AlertMessage(ByVal alertString As String)
        Dim jsString As String = "<script>window.alert('"
        jsString += alertString
        jsString += "');</script>"
        Dim pageInvoke As Page = CType(HttpContext.Current.Handler, Page)
        pageInvoke.Response.Write(jsString)
        pageInvoke.Response.Write("<script>document.location=document.location;</script>")
    End Sub

    Public Shared Sub AlertMessageInsideUpdatePanel(ByVal msg As String)
        '-- replacing unwanted chars (')
        msg = Replace(msg, Chr(39), "`", 1, msg.Length)
        Dim pageInvoke As Page = CType(HttpContext.Current.Handler, Page)
        Dim winAlert As String = "alert('" & msg & "')"

        ScriptManager.RegisterStartupScript(pageInvoke, pageInvoke.GetType(), "click", winAlert, True)
    End Sub
End Class