namespace MyCommon.MyEnum
{
    public enum NPageID
    {
        //Configuration
        Configuration=10,
            


        MisHome = 80,



        UnderConstruction=90,

        //Project

        ProjectDefault = 100,
        ProjectDetails=102,
        ProjectBIQ = 101,

        ProjectFollowUp=103,

        ProjectStage = 104,
        ProjectWip = 105,

        ProjectConfiguration = 107,

        Estimation=111,
        EstimationUploadMultipleFile=112,
        GenerateNewQuoteFromEstimation=113,
        GenerateNewInvoiceFromEstimation=114,
        GenerateNewWorkorderFromEstimation=115,

        Quotation=131,
        QuotationFollowUp=132,
            


        //Work Order

        WorkorderDefault = 150,
        WorkorderDetail=151,
        WorkorderItem=152,
        WorkorderInstruction=153,
        WorkorderDoc=154,
        WorkorderValidation=155,

        WorkorderAddressLabel = 156,
        WorkorderDeliveryNote = 157,

        WorkorderConfiguration = 159,
        WorkorderReadOnly=159,

        //Invoice
        InvoiceDefault = 160,
        InvoiceDetail=161,
        InvoiceConfiguration = 162,

            


        //Employee
        EmployeeBasic=301,
        EmployeeManagement = 8100,
        MyTimeCard = 8171,

        TimeCardManipulate = 8172,
        TimeCardManipulateV2 = 8173,
        TimeCardHoliday = 8174,
        TimeCardSubstitude = 8175,
        TimeCardDetailsV2 = 8176,
        TimeCardOvertimeManagement = 8177,
        TimeCardOvertimeReport = 8178,
        TimeCardLateForWorkReport = 8179,

        CommissionInvoice=302,
        CommissionList=303,
        SalesCommission = 8400,
        SalesCommissionFactorOfProject = 8401,
        SalesCommissionValidation = 8402,

        SpecialMaterial=340,

        PermitIndex=350,

        //Production

        SubcontractIndex=420,
        SubContract = 135,



        WipProject = 140,
        WipDefault = 141,
        WipPublic = 145,
        WipConfiguration = 146,

            
            

        ProductionDefault = 8300,
        ProjectProduction = 8301,
        ProductionSchedule = 8302,
        InstallationSchedule = 8303,
        SiteCheckSchedule = 8304,

        PppIndex=8305,
        PppLabourHour=8306,
        PppPlan=8307,
        PppWeeklyFinishedWorkorder=8308,

        SubcontractDefault = 8550,
        SubcontractRequest = 8551,
        SubcontractResponse = 8552,
        SubcontractShipping = 8553,
        SubcontractCommunication = 8555,
        SubcontractRating = 8556,


        //Response
        ResponseCenter = 8223,
        ArtRoomProductionSchedule = 8306,
        ArtRoomInputLabourHour = 8307,
        ArtRoomProductionLabourHourInquiry = 8308,

        ResponseDefault = 180,
        WorkorderApproval = 181,
        ResponseConfiguration = 182,

        //Report
        ReportDefault = 150,
        SalesReport = 8121,
        ArtRoomAndQdReport = 8122,
        AdminIndex = 1001,
        AdminDetails = 1002,


        //Customer
        CustomerDefault = 260,

        CustomerDetail = 261,
        CustomerOrganize = 262,

        CustomerCreateNew = 263,
        CustomerOrganization=264,
        CustomerConfiguration=266,

        //Lead
        LeadDefault = 240,
        LeadDetail = 241,       
        LeadConvertToProject=242,

        JobCosting = 8330,
        MaterialExplor = 8340,   
    
        AdminTask = 8670,
        Test = 8888,

    

    }


}