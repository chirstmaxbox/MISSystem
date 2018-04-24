
Imports System.Web.UI
Imports System.Web

Public Class MyWindow

    Public Shared Sub OpenNoToolBars(ByVal url As String, ByVal height As Integer)
        Dim pageInvoke As Page = CType(HttpContext.Current.Handler, Page)
        Dim jsString As String = "<script>window.open('" & url & "','Company','width=1000, height=700, toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,copyhistory=no,resizable=yes');</script>"""

        pageInvoke.Response.Write(jsString)
        pageInvoke.Response.Write("<script>document.location=document.location;</script>")
    End Sub

    Public Shared Sub OpenInsideUpdatePanel(ByVal url As String)
        url = Replace(url, Chr(39), "`", 1, url.Length)
        '--
        Dim winOpen As String = "open('" & url & "','')"
        Dim pageInvoke As Page = CType(HttpContext.Current.Handler, Page)
        ScriptManager.RegisterStartupScript(pageInvoke, pageInvoke.GetType(), "click", winOpen, True)
    End Sub


    Public Shared Sub OpenInsideUpdatePanelNoToolsbars(ByVal url As String, ByVal height As Integer)

        Dim pageInvoke As Page = CType(HttpContext.Current.Handler, Page)
        url = Replace(url, Chr(39), "`", 1, url.Length)
        '--
        Dim winOpen As String = "open('" & url & "','Company','width=1000,toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,copyhistory=no,resizable=yes')"
        Dim s As String = winOpen
        ScriptManager.RegisterStartupScript(pageInvoke, pageInvoke.GetType(), "click", winOpen, True)
    End Sub

    Public Shared Sub OpenInsideUpdatePanelNoToolsbars(ByVal url As String)

        Dim pageInvoke As Page = CType(HttpContext.Current.Handler, Page)
        url = Replace(url, Chr(39), "`", 1, url.Length)
        '--
        Dim winOpen As String = "open('" & url & "','Company','width=1000,toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,copyhistory=no,resizable=yes')"
        Dim s As String = winOpen
        ScriptManager.RegisterStartupScript(pageInvoke, pageInvoke.GetType(), "click", winOpen, True)
    End Sub

    Public Shared Sub OpenInsideUpdatePanelNoToolsbarSmall(ByVal url As String)
        Dim pageInvoke As Page = CType(HttpContext.Current.Handler, Page)
        url = Replace(url, Chr(39), "`", 1, url.Length)
        '--
        Dim winOpen As String = "open('" & url & "','Company','width=700, height=300, toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,copyhistory=no,resizable=yes')"
        Dim s As String = winOpen
        ScriptManager.RegisterStartupScript(pageInvoke, pageInvoke.GetType(), "click", winOpen, True)
    End Sub


    Public Shared Sub OpenGoogleMap(ByVal postcode As String)
        Dim url As String = "http://maps.google.ca/maps?source=ig&hl=en&rlz=1G1GGLQ_ENCA276&q="
        url += postcode
        url += "&um=1&ie=UTF-8&sa=N&tab=wl','"
        OpenInsideUpdatePanel(url)
    End Sub


    Public Shared Sub Open(ByVal pageAddress As String)
        Dim jsString As String = "<script>window.open('"
        jsString += pageAddress
        jsString += "');</script>"
        Dim pageInvoke As Page = HttpContext.Current.Handler
        pageInvoke.Response.Write(jsString)
        pageInvoke.Response.Write("<script>document.location=document.location;</script>")
    End Sub

    Public Shared Sub OpenInNewWindow(ByVal pageAddress As String)
        Dim jsString As String = "<script>window.open('"
        jsString += pageAddress
        jsString += "',   'blank');</script>"
        Dim pageInvoke As Page = HttpContext.Current.Handler
        pageInvoke.Response.Write(jsString)
        pageInvoke.Response.Write("<script>document.location=document.location;</script>")
    End Sub


End Class