namespace SalesCenterDomain.BO
{
    public enum NCommissionProjectCategoryID
    {
        House = 0,
        New = 1,
        Bid = 2
    }

    public enum NEmployeeCommissionRole
    {
        AE = 1, //Account Executive
        OP = 2, //Operator?
        LeftAE = 3, //Quit or Fired
        ReferralAE = 4, //Internal Referral
        Aet = 5 //AE Trainee
    }

    public enum NEstItemPurpose
    {
        ForEstimation = 0,
        ForBackup = 10,
    }

    public enum NEstItemNameDetailsType
    {
        New = 0,
        Reface = 10
    }

    public enum NEstItemStatus
    {
        New = 0,
        ContentsChanged = 1,
        Estimated = 2,
    }
}