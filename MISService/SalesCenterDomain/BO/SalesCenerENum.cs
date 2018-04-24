namespace SalesCenterDomain.BO
{

    #region NAccountingTerm enum

    public enum NAccountingTerm
    {
        DueUponReceipt = 100
    }

    #endregion

    #region NCopyType enum

    public enum NCopyType
    {
        ToNewItem = 1,
        ToOption = 2
    }

    #endregion

    #region NDatabase_Table_RowID enum

    public enum NDatabase_Table_RowID
    {
        Blank = 0,
        All = 99,
        TBD = 1
    }

    #endregion

    #region NGenerateNewItemType enum

    public enum NGenerateNewItemType : byte
    {
        NewItem = 1,
        NewItemOption = 2,
        CopyItem = 3
    }

    #endregion

    #region NGenerateTitleFrom enum

    public enum NGenerateTitleFrom
    {
        Blank = 1,
        Estimation = 2,
        Quote = 3,
        Workorder = 4
    }

    #endregion

    #region NInvoicePrintType enum

    public enum NInvoicePrintType : byte
    {
        Blank = 1,
        Regular = 2
    }

    #endregion

    #region NItemType enum

    public enum NItemType
    {
        Estimation = 0,
        Quote = 1,
        Workorder = 2,
        Invoice = 3
    }

    #endregion

    #region NLeadtimeType enum

    public enum NLeadtimeType : short
    {
        SupplyAndInstall = 0,
        SupplyOnly = 1
    }

    #endregion

    #region NQuoteItemType enum

    public enum NQuoteItemType : byte
    {
        Product = 1,
        Service = 2,
        Note = 3
    }

    #endregion

    #region NQuotePrintFormat enum

    public enum NQuotePrintFormat
    {
        Mar2010 = 0,
        Jun2010 = 1
    }

    #endregion

    #region NQuotePrintOption enum

    public enum NQuotePrintOption
    {
        DetailsOnly = 1,
        DetailsAndTotal = 2,
        TotalOnly = 3
    }

    #endregion

    #region NQuoteSupplyType enum

    public enum NQuoteSupplyType
    {
        SupplyAndInstallation = 10,
        SupplyOnly = 20,
        TakeDown = 30,
        Service = 40,
        InstallationOnly = 50
    }

    #endregion

    #region NRequisitionType enum

    public enum NRequisitionType : short
    {
        Estimation = 0,
        Drawing = 1
        //Graphic / Structural
    }

    #endregion

    #region NServiceID enum

    public enum NServiceID
    {
           CratingCharge = 2030,
            DeliveryCharge = 2020,
            DutyCop = 1050,
            DutyCopPlusHoistingPermit = 1060,
            EngineeringStampingCharge = 1080,
            HydroWiringInspectionCharge = 1090,
            MTOPermitValue = 1040,
            ShippingCharge = 2010,
            ElectricalPermit = 2011,
            SignPermit = 1032,
            SignVariance = 1034,
            SiteCheckCharge = 2040,
            StakeOutCharge = 1070,
            TravelingCharge = 2050,
            InstallationCharge = 2060,
            ServingLabourCharge = 2070,
            MaterialUsed = 2080,
            FreeType = 2099,
            NULL = 0
    
    }

    #endregion

    #region NTaxOption enum

    public enum NTaxOption
    {
        HST = 0,
        HstBC = 1,
        GstOnly = 2,
        GstAndPst = 3,
        GstAndManuallyPst = 4,
        Manually = 5,
        NoTax = 6
    }

    #endregion

    #region ****** Workorder **********

    #region NWorkorderCopyType enum

    public enum NWorkorderCopyType
    {
        ToAnotherNewWorkorder = 1,
        ToNewWorkorderRevision = 2,
        ToReviseNewWorkorder = 3,
        ToReDoNewWorkorder = 4
    }

    #endregion

    #region NWorkorderDeadlineType enum

    public enum NWorkorderDeadlineType
    {
        Changed = 0,
        Revised = 1
    }

    #endregion

    #region NWorkorderInstructionRecordType enum

    public enum NWorkorderInstructionRecordType
    {
        Installation = 0,
        Workshop = 1
    }

    #endregion

    #region NWorkorderLeadtimeSpecialType enum

    public enum NWorkorderLeadtimeSpecialType
    {
        Normal = 0,
        Sample = 1
        //...
    }

    #endregion

    #region NWorkorderNoteType enum

    public enum NWorkorderNoteType
    {
        Invoice = 1
    }

    #endregion

    #region NWorkorderRequirement enum

    #endregion

    #region NWorkorderResponseStatus enum

    public enum NWorkorderResponseStatus
    {
        NewlyCreated = 0,
        Read = 1
    }

    #endregion

    #region NWorkorderType enum

    #endregion

    #endregion

    #region NTimeRange enum

    public enum NTimeRange
    {
        JobListMasterChangeStatuw = 48
    }

    #endregion

    public enum NWipTaskDateAssignTypeStart
    {
        DateTimeToday = 10,
        EndingDateMinusLeadTime = 15,

        PriorTo = 30
    }

    public enum NWipTaskDateAssignTypeFinish
    {
        EndingDate = 10,
        StartDatePlusLeadtime = 15
    }

    public enum NWipTask
    {
        PriorityNormal = 10,

        StatusNew = 0,
        StatusFinish = 90,

        ContentStageOther = 65535,
        ContentIdNothing = 0
    }
}