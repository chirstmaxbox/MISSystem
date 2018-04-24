Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports System.Web

Public Class MyDropdownlist

    Public Shared Sub DisplaySelectValue(ByVal ddl As DropDownList, ByVal itemValue As Integer)

        Dim selectitem As Boolean = False
        Dim itemlength As Integer = ddl.Items.Count
        Dim i As Integer = 0
        For i = 1 To itemlength - 1
            Dim s1 As Integer = ddl.Items(i).Value

            If itemValue = s1 Then
                ddl.SelectedIndex = i
                selectitem = True
            End If
        Next
        '
        If selectitem Then
            ddl.DataBind()
        End If
    End Sub

    Public Shared Sub DisplaySelectValue(ByVal ddl As DropDownList, ByVal itemValue As Integer, ByVal isWithDatabind As Boolean)

        Dim selectitem As Boolean = False
        Dim itemlength As Integer = ddl.Items.Count
        Dim i As Integer = 0
        For i = 1 To itemlength - 1
            Dim s1 As Integer = ddl.Items(i).Value

            If itemValue = s1 Then
                ddl.SelectedIndex = i
                selectitem = True
            End If
        Next
        '
        If isWithDatabind Then
            If selectitem Then
                ddl.DataBind()
            End If
        End If
    End Sub

    Public Shared Sub DisplaySelectItem(ByVal ddl As DropDownList, ByVal itemName As String)
        Dim selectitem As Boolean = False
        Dim itemlength As Integer = ddl.Items.Count
        Dim i As Integer = 0
        For i = 0 To itemlength - 1
            Dim s1 As String = ddl.Items(i).Text.Trim
            If itemName = s1 Then
                ddl.SelectedIndex = i
                selectitem = True
            End If
        Next
        '
        If selectitem Then
            '2.5
            ddl.DataBind()
        End If
    End Sub

    Public Shared Sub AddItem0All(ByVal ddl As DropDownList)
        ' Add the "All" item to dropdownlist
        Dim wildcard As ListItem = New ListItem()
        wildcard.Text = "All"
        wildcard.Value = 0
        ddl.Items.Insert(ddl.Items.Count, wildcard)
    End Sub

    Public Shared Sub AddItem(ByVal ddl As DropDownList, ByVal text As String, ByVal value As Integer)
        Dim wildcard As ListItem = New ListItem()
        If Not IsDBNull(Text) Then
            wildcard.Text = Text
        Else
            wildcard.Text = " "
        End If

        wildcard.Value = Value
        ddl.Items.Insert(ddl.Items.Count, wildcard)
    End Sub

    Public Shared Sub RegScript(ByVal ddl As DropDownList)
        Dim script As String = String.Format(vbCr & vbLf & "var select=$get('{0}'); " & vbCr & vbLf & "if (select) {{" & vbCr & vbLf & "var stub=document.createElement('input'); " & vbCr & vbLf & "stub.type='hidden'; " & vbCr & vbLf & "stub.id=select.id; " & vbCr & vbLf & "stub.name=select.name; " & vbCr & vbLf & "stub._behaviors=select._behaviors; " & vbCr & vbLf & "var val=new Array(); " & vbCr & vbLf & "for (var i=0; i<select.options.length; i++)" & vbCr & vbLf & "if (select.options[i].selected) {{" & vbCr & vbLf & "val[val.length]=select.options[i].value; " & vbCr & vbLf & "}}" & vbCr & vbLf & "stub.value=val.join(',');" & vbCr & vbLf & "select.parentNode.replaceChild(stub,select);" & vbCr & vbLf & "}};" & vbCr & vbLf, ddl.ClientID)
        Dim pageInvoke As Page = CType(HttpContext.Current.Handler, Page)
        Dim sm As ScriptManager = ScriptManager.GetCurrent(pageInvoke)
        If sm IsNot Nothing Then
            sm.RegisterDispose(ddl, script)
        End If

        script = "WebForm_InitCallback=function() {};"
        ScriptManager.RegisterStartupScript(pageInvoke, pageInvoke.GetType(), "removeWebForm_InitCallback", script, True)
    End Sub

    Public Shared Function GetMaxValue(ByVal ddl As DropDownList) As Integer
        ' get Max selected Value
        Dim i As Integer = 0
        Dim maxValue As Integer = 0
        Dim itemNum As Integer = ddl.Items.Count
        For i = 0 To itemNum - 1
            If CInt(ddl.Items(i).Text) > maxValue Then
                maxValue = CInt(ddl.Items(i).Text)
            End If
        Next
        Return maxValue
    End Function

    Public Shared Function GetCascadingDdl(ByVal ccd As Object) As String
        If IsDBNull(ccd) Then
            Return "0"
        Else
            Return ccd
        End If
    End Function

End Class