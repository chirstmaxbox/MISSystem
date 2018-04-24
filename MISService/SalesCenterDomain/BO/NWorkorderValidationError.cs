namespace SalesCenterDomain.BO
{
    public enum NWorkorderValidationError
    {
        Validated = -1,
        DeadlineRequired = 10,
        DelalineIsLatterThanToday = 11,
        ReasonofRedoRequired = 21,
        ReasonofRushRequired = 22,
        ReasonofReviseRequired = 23,
        ReasonofNoContract = 30,
        ExceedContractAmount = 31,
        NoContract = 32,
        IssuedateisInvalid = 35,
        SiteContactnameandPhoneOrCellisRequired = 40,
        MainIntersectionOrConnerorPostCode = 45,
        InstalltoAddressOrCityRequired = 50,
        SiteCheckPurposeRequired = 70,
        ChecklistDocumentsAttachedRequired = 75,

        //Lead time
        ItemNameRequired = 200,
        ItemPriceSumEqualsToZero = 201,
        ItemLeadtimeRequired = 205,
        IsSpecialProcedureRequired = 210,

        //Item Instruction
        InstructionRelocation = 320,
        InstructionTakedownAndDispose = 321,
        InstructionTakedownAndBringbackStock = 322,
        InstructionTakedownAndDelivery = 323
    }
}