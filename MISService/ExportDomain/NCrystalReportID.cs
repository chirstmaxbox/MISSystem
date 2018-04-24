namespace ExportDomain
{
    public enum NCrystalReportID
    {
        EstimationRequisitionForm = 501,
        DrawingRequisitionForm = 503,

        EstimationCost = 502,
        EstimationItemCost = 504,


        QuotationBeforeJune1510 = 505,
        Quotation = 507,
        WipPublic = 510,
        WipPublicToBeSend = 511,
        WipInternalOnGoing = 512,
        WipInternalOfOneJob = 513,

        Workorder = 515,
        WorkorderShippingAddress = 516,
        WorkorderDeliveryNote = 517,

        Invoice = 518,

        SubContractInstallationChecklist = 520,
        SubContractInstallationPO = 521,
        SubContractInstallationShippingAddress = 522,
        SubContractInstallationReport = 523,

        WorkorderStatusOfOnGoingContract = 525,
        WorkorderStatusOfSmartCenter = 526,
        WorkorderStatusOfOnGoingContractByAE = 527,
        WorkorderStatusOfOnGoingContractByTargetDate = 528,

        WeeklyIssuedWorkorderByAE=529,
        WeeklyIssuedWorkorderByIssuedDate = 530,
        WeeklyFinishedWorkorderByAE = 531,
        WeeklyFinishedWorkorderByFinishedByDate = 532,

        
        ProjectAwarded = 2011,
        ToBeFollowedQuotation = 2012,
        ProjectLost = 2013,

        ContratWithOutWorkorder = 2036,

        ProjectStage = 2501,
        ProjectWip = 2502,

        TimeCard = 2701,
        TimeCardV2X = 2702,
        TimeCardOvertimeReport = 2703,
        SalesCommission = 2801,
        SalesCommissionInvoiceList = 2802,


        //EVO Html To PDF
        EstimationBidItemCost = 3011,
        EstimationBidCostSheet = 3012,
        EstimationBidCostSummary =3013,

//Pre-Production Plan
        PppWeeklyFinishedWorkorder = 8308,

    }
}