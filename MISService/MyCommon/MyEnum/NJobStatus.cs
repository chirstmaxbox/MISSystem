namespace MyCommon.MyEnum
{
   
    public enum NJobStatus
    {
        ProjectNew = 101,
        EstimationNew = 201,
        EstimationFinish = 249,
        scIssued = 301,
        scFinished = 311,

        qProcessing = 401,
        ToBeAwarded = 415,
        win = 420,

        artDrwNew = 501,
        artDrwFinish = 549,
        sDrwNew = 551,
        sDrwFinish = 599,

        smNew = 601,
        smSubmitted = 603,
        smFinish = 609,

        smResponseNew = 603,
        smWorking = 604,
        smResponseFinished = 605,

        permitNew = 611,

        woNew = 701,
        woPrepared = 720,
        woInvalid = 730,
        woApproved = 781,
        woObsolete = 790,
        shopNew = 801,
        ShopReady = 840,
        Deliveried = 880,
        PickedUp = 885,

        iNew = 911,
        iProblem = 921,
        iFinish = 939,

        invNew = 2001,
        InvPrepared = 2002,
        InvApproved = 2003,
        //InvRevised = 2004

        Complete = 2309,
        //Cancel = 3001
        Loss = 3101
    }


}