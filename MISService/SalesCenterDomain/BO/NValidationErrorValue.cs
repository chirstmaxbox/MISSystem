namespace SalesCenterDomain.BO
{
    public enum NValidationErrorValue
    {
        OK = 0,
        SessionExpired = 10,
        UnAuthorization = 12,
        AtLeastOneItem = 15,
        Delete = 50,

        InstallTo = 101,
        BillTo = 102,
        QuoteTo = 103,
        QouteToContact = 104,

        SubProjectCannotHaveSubProject = 1002,
        EstimationLockedNoNewItem = 1101,
        EstimationLockedCannotBeDeleted = 1103,
        ProductionNotBeEstimated = 1102,
        ProductItemDetailsRequired = 1104,
        ProjectCanOnlyHaveOneEstimation = 1105,
        SubProjectCreateNewEstimation = 1106,
        EstimationLockedCannotBeSubmited = 1107,
        EstimationAtLeastOneActiveItemRequired = 1108,

        QuoteChangeFromEstimation = 1301,
        QuoteCannotFindContract = 1302,

        WorkorderLockedCantNotDelete = 1501,
        WorkorderCopyToRevised = 1502,
        WorkorderCopyToRedo = 1503,
        WorkorderLockedCanNotAddNewItem = 1504,
        WorkorderDidNotFound = 1505,
        WorkorderAddNewItem = 1506,

        InvoiceDelete = 1601,
        InvoiceProductAmount = 1602,
        InvoiceServiceAmount = 1603,
        InvoiceApproveStatus = 1604
    }
}