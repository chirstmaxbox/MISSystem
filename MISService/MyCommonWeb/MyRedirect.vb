

Imports MyCommon
Imports System.Web
Imports MyCommon.MyEnum

Public Class MyRedirect

    Private Function GetUrls() As List(Of MyKeyValuePair)

        Dim v1000 = New MyKeyValuePair() With {.Key = NPageID.MisHome, .Value = "http://f01/Default.aspx"}
        Dim v1010 = New MyKeyValuePair() With {.Key = NPageID.UnderConstruction, .Value = "http://f01/UnderConstruction.aspx"}

        Dim v1020 = New MyKeyValuePair() With {.Key = NPageID.EmployeeManagement, .Value = "http://f01:8100/Employee/EmployeeBasic.aspx"}
        Dim v1030 = New MyKeyValuePair() With {.Key = NPageID.SalesCommission, .Value = "http://f01:8400/Commission/InvoiceListOfSalesCommission.aspx"}
        Dim v1040 = New MyKeyValuePair() With {.Key = NPageID.SalesCommissionFactorOfProject, .Value = "http://f01:8400/Commission/FactorOfCommissionProject.aspx"}
        Dim v1041 = New MyKeyValuePair() With {.Key = NPageID.SalesCommissionValidation, .Value = "http://f01:8400/Commission/Validation.aspx"}
        Dim v1050 = New MyKeyValuePair() With {.Key = NPageID.MyTimeCard, .Value = "http://f01:8170/TimeCard/TimeCardViewLogIn.aspx"}
        Dim v1060 = New MyKeyValuePair() With {.Key = NPageID.TimeCardManipulate, .Value = "http://f01:8170/TimeCard/TimeCardManipulate.aspx"}
        Dim v1061 = New MyKeyValuePair() With {.Key = NPageID.TimeCardManipulateV2, .Value = "http://f01:8170/TimeCard/TimeCardManipulateV2.aspx"}
        Dim v1062 = New MyKeyValuePair() With {.Key = NPageID.TimeCardHoliday, .Value = "http://f01:8170/TimeCard/HolidayMaintenance.aspx"}
        Dim v1063 = New MyKeyValuePair() With {.Key = NPageID.TimeCardSubstitude, .Value = "http://f01:8170/TimeCard/Substitude.aspx"}
        Dim v1064 = New MyKeyValuePair() With {.Key = NPageID.TimeCardOvertimeManagement, .Value = "http://f01:8170/TimeCard/OvertimeControl.aspx"}
        Dim v1065 = New MyKeyValuePair() With {.Key = NPageID.TimeCardOvertimeReport, .Value = "http://f01:8170/TimeCard/OvertimeReport.aspx"}
        Dim v1066 = New MyKeyValuePair() With {.Key = NPageID.TimeCardLateForWorkReport, .Value = "http://f01:8170/TimeCard/ReportClockInLate.aspx"}


        Dim v1070 = New MyKeyValuePair() With {.Key = NPageID.CustomerDetail, .Value = "http://f01:8210/Customer/Detail.aspx"}
        Dim v1080 = New MyKeyValuePair() With {.Key = NPageID.CustomerDefault, .Value = "http://f01:8210/Customer/Customers.aspx"}
        Dim v1090 = New MyKeyValuePair() With {.Key = NPageID.CustomerCreateNew, .Value = "http://f01:8210/Customer/CreateA.aspx"}
        Dim v1100 = New MyKeyValuePair() With {.Key = NPageID.LeadDefault, .Value = "http://f01:8200"}
        Dim v1101 = New MyKeyValuePair() With {.Key = NPageID.LeadConvertToProject, .Value = "http://f01:8220/Sales/Project/OnLeadConvertedToProject.aspx"}

        Dim v1110 = New MyKeyValuePair() With {.Key = NPageID.CustomerConfiguration, .Value = "http://f01:8210/Customer/Configuration.aspx"}
        Dim v1111 = New MyKeyValuePair() With {.Key = NPageID.CustomerOrganization, .Value = "http://f01:8210/Customer/Organize/AssignSales.aspx"}

        Dim v1120 = New MyKeyValuePair() With {.Key = NPageID.ProjectDefault, .Value = "http://f01:8220/Sales/Project/Default.aspx"}
        Dim v1130 = New MyKeyValuePair() With {.Key = NPageID.ProjectDetails, .Value = "http://f01:8220/Sales/Project/Detail.aspx"}
        Dim v1131 = New MyKeyValuePair() With {.Key = NPageID.ProjectConfiguration, .Value = "http://f01:8220/Sales/Project/Configuration.aspx"}

        Dim v1140 = New MyKeyValuePair() With {.Key = NPageID.ProjectProduction, .Value = "http://f01/Production.aspx"}

        Dim v1150 = New MyKeyValuePair() With {.Key = NPageID.ProjectStage, .Value = "http://f01:8600/Project/ProjectStage"}
        Dim v1160 = New MyKeyValuePair() With {.Key = NPageID.ProjectWip, .Value = "http://f01:8600/Project/ProjectWip"}

        Dim v1170 = New MyKeyValuePair() With {.Key = NPageID.ProjectBIQ, .Value = "http://f01:8220/Sales/Project/BQIto.aspx"}
        Dim v1180 = New MyKeyValuePair() With {.Key = NPageID.Estimation, .Value = "http://f01:8230"}


        Dim v1182 = New MyKeyValuePair() With {.Key = NPageID.GenerateNewQuoteFromEstimation, .Value = "http://f01:8220/Sales/Quotation/CommandGenerateNewQuote.aspx"}
        Dim v1183 = New MyKeyValuePair() With {.Key = NPageID.GenerateNewWorkorderFromEstimation, .Value = "http://f01:8220/Sales/Workorder/CommandGenerateFromEstimation.aspx"}
        Dim v1184 = New MyKeyValuePair() With {.Key = NPageID.GenerateNewInvoiceFromEstimation, .Value = "http://f01:8220/Sales/Invoice/CommandGenerateFromEstimation.aspx"}

        '      Dim v1190 = New MyKeyValuePair() With {.Key = NPageID.EstimationUploadMultipleFile, .Value = "http://f01:8220/Sales/Estimation/MultipleFileUpload.aspx"}

        Dim v1200 = New MyKeyValuePair() With {.Key = NPageID.Quotation, .Value = "http://f01:8220/Sales/Quotation/Detail.aspx"}
        Dim v1210 = New MyKeyValuePair() With {.Key = NPageID.QuotationFollowUp, .Value = "http://f01:8220/Sales/Quotation/FollowUpDetails.aspx"}

        Dim v1220 = New MyKeyValuePair() With {.Key = NPageID.WorkorderDetail, .Value = "http://f01:8220/Sales/Workorder/Detail.aspx"}
        'Dim v1230 = New MyKeyValuePair() With {.Key = NPageID.WorkorderItem, .Value = "http://f01:8220/Sales/Workorder/Item.aspx"}
        'Dim v1240 = New MyKeyValuePair() With {.Key = NPageID.WorkorderValidation, .Value = "http://f01:8220/Sales/Workorder/Validation.aspx"}
        'Dim v1250 = New MyKeyValuePair() With {.Key = NPageID.WorkorderAddressLabel, .Value = "http://f01:8220/Sales/Workorder/AddressLabel.aspx"}
        'Dim v1260 = New MyKeyValuePair() With {.Key = NPageID.WorkorderDeliveryNote, .Value = "http://f01:8220/Sales/Workorder/DeliveryOrPickupNote.aspx"}
        Dim v1270 = New MyKeyValuePair() With {.Key = NPageID.WorkorderConfiguration, .Value = "http://f01:8220/Sales/Workorder/Configuration.aspx"}

        Dim v1280 = New MyKeyValuePair() With {.Key = NPageID.InvoiceDetail, .Value = "http://f01:8220/Sales/Invoice/Detail.aspx"}
        Dim v1290 = New MyKeyValuePair() With {.Key = NPageID.InvoiceConfiguration, .Value = "http://f01:8220/Sales/Invoice/Configuration.aspx"}


        Dim v1300 = New MyKeyValuePair() With {.Key = NPageID.ResponseCenter, .Value = "http://f01:8220/Sales/Response/Default.aspx"}
        Dim v1310 = New MyKeyValuePair() With {.Key = NPageID.ResponseConfiguration, .Value = "http://f01:8220/Sales/Response/Configuration.aspx"}
        Dim v1320 = New MyKeyValuePair() With {.Key = NPageID.WorkorderApproval, .Value = "http://f01:8300/Approval/ApproveWorkorder.aspx"}
        Dim v1330 = New MyKeyValuePair() With {.Key = NPageID.ArtRoomProductionSchedule, .Value = "http://f01:8300/Production/Schedule/ScheduleArtRoom.aspx"}
        Dim v1340 = New MyKeyValuePair() With {.Key = NPageID.ArtRoomInputLabourHour, .Value = "http://f01:8300/Production/LabourTicket/LabourTicketArtRoom.aspx"}
        Dim v1350 = New MyKeyValuePair() With {.Key = NPageID.ArtRoomProductionLabourHourInquiry, .Value = "http://f01:8300/Production/LabourTicket/InquiryLabourTicketByArtRoom.aspx"}


        Dim v1360 = New MyKeyValuePair() With {.Key = NPageID.ProductionDefault, .Value = "http://f01:8300/Default.aspx"}
        Dim v1370 = New MyKeyValuePair() With {.Key = NPageID.ProductionSchedule, .Value = "http://f01:8300/Production/Schedule/ScheduleMeeting.aspx"}
        Dim v1380 = New MyKeyValuePair() With {.Key = NPageID.InstallationSchedule, .Value = "http://f01:8300/Production/Schedule/ScheduleInstallation.aspx?id=7"}
        Dim v1390 = New MyKeyValuePair() With {.Key = NPageID.SiteCheckSchedule, .Value = "http://f01:8300/Production/Schedule/ScheduleSiteCheck.aspx"}

        Dim v1400 = New MyKeyValuePair() With {.Key = NPageID.SubcontractDefault, .Value = "http://f01:8550/Default.aspx"}
        Dim v1410 = New MyKeyValuePair() With {.Key = NPageID.SubcontractRequest, .Value = "http://f01:8550/SubContract/Request.aspx"}
        Dim v1420 = New MyKeyValuePair() With {.Key = NPageID.SubcontractResponse, .Value = "http://f01:8550/SubContract/Response.aspx"}
        Dim v1430 = New MyKeyValuePair() With {.Key = NPageID.SubcontractShipping, .Value = "http://f01:8550/SubContract/Shipping.aspx"}
        Dim v1440 = New MyKeyValuePair() With {.Key = NPageID.SubcontractRating, .Value = "http://f01:8550/SubContract/Rating.aspx"}
        Dim v1450 = New MyKeyValuePair() With {.Key = NPageID.SubcontractCommunication, .Value = "http://f01:8550/SubContract/Cummunication.aspx"}



        Dim v1460 = New MyKeyValuePair() With {.Key = NPageID.WipPublic, .Value = "http://f01:8220/Sales/Wip/DefaultPublic.aspx"}
        Dim v1470 = New MyKeyValuePair() With {.Key = NPageID.WipDefault, .Value = "http://f01:8220/Sales/Wip/DefaultInternal.aspx"}
        Dim v1480 = New MyKeyValuePair() With {.Key = NPageID.WipConfiguration, .Value = "http://f01:8220/Sales/Wip/Configuration.aspx"}

        Dim v1491 = New MyKeyValuePair() With {.Key = NPageID.MaterialExplor, .Value = "http://f01:8230/Material/Explorer"}

        Dim v1490 = New MyKeyValuePair() With {.Key = NPageID.JobCosting, .Value = "http://f01:8330"}


        Dim v1500 = New MyKeyValuePair() With {.Key = NPageID.ArtRoomAndQdReport, .Value = "http://f01:8120/DrawingReport/Reports/Default.aspx"}
        Dim v1510 = New MyKeyValuePair() With {.Key = NPageID.SalesReport, .Value = "http://f01:8120/Report/Default.aspx"}


        Dim v1520 = New MyKeyValuePair() With {.Key = NPageID.Configuration, .Value = "http://f01:8500/Configuration/Default.aspx"}

        Dim v1530 = New MyKeyValuePair() With {.Key = NPageID.Test, .Value = "http://f01:8888/Default.aspx"}
        Dim v1540 = New MyKeyValuePair() With {.Key = NPageID.AdminTask, .Value = "http://f01:8670"}


        Return New List(Of MyKeyValuePair) From {v1000, v1010, v1020, v1030, v1040, v1041, v1050, v1060, v1061, v1062, v1063, v1064, v1065, v1066, v1070, v1080, v1090,
                                                 v1100, v1101, v1110, v1111, v1120, v1130, v1131, v1140, v1150, v1160, v1170, v1180, v1182, v1183, v1184,
                                                 v1200, v1210, v1220, v1270, v1280, v1290,
                                                 v1300, v1310, v1320, v1330, v1340, v1350, v1360, v1370, v1380, v1390,
                                                 v1400, v1410, v1420, v1430, v1440, v1450, v1460, v1470, v1480, v1490, v1491,
                                                 v1500, v1510, v1520, v1530, v1540}



    End Function


#Region "********************** Program *********************************"

    Public Property Url As String

    Public Sub New(ByVal pageID As Integer)

        Dim urlKeyValuePair As MyKeyValuePair = (From x In GetUrls() Where x.Key = pageID).First
        Url = urlKeyValuePair.Value

    End Sub

    Public Sub RedirectTo()
        HttpContext.Current.Response.Redirect(Url)
    End Sub


    Public Sub RedirectTo(ByVal handlingItemID As Integer)
        Dim fullUrl As String = Url + "?HandlingItemID=" + handlingItemID.ToString
        HttpContext.Current.Response.Redirect(fullUrl)
    End Sub

    Public Sub RedirectTo(ByVal parentID As Integer, ByVal handlingItemID As Integer)
        Dim fullUrl As String = Url + "?ParentID=" + parentID.ToString + "&HandlingItemID=" + handlingItemID.ToString
        HttpContext.Current.Response.Redirect(fullUrl)
    End Sub


    Public Sub OpenNewWindowTo(ByVal parentID As Integer, ByVal handlingItemID As Integer)
        Dim fullUrl As String = Url + "?ParentID=" + parentID.ToString + "&HandlingItemID=" + handlingItemID.ToString
        MyWindow.OpenInsideUpdatePanel(fullUrl)
    End Sub

#End Region

End Class


Public Class MyDefaultRedirectTo
    Public Shared Sub LoginFailed()
        HttpContext.Current.Response.Redirect("http://f01/LoginFailed.aspx")
    End Sub

    Public Shared Sub UnderConstruction()
        HttpContext.Current.Response.Redirect("http://f01/UnderConstruction.aspx")
    End Sub

    Public Shared Sub MisHome()
        HttpContext.Current.Response.Redirect("http://f01")
    End Sub
End Class

