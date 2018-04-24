namespace MyCommon.MyEnum
{

    public enum NMenuCommand:int 
    {
        //Revision
        Null = 0,
  
        NewSubProject = 11,
        NewRevision = 10,
        CopyToNewRevision = 14,
        CopyToAnother = 12,

        Delete = 13,
  
        SubmitRequest = 15,     //?
        //to be updated

        AttachDocument = 16,
        AttachDocuments = 17,
        NewService = 19,

        NewQuote = 20,
        NewNote = 21,

        SetRevisionAsFinal = 25,
        ViewQuoteFollowUp = 29,

        Print = 30,
        PrintBlank = 31,
        PrintDrawing = 32,
        PrintInvoiceWithHeader = 33,
            

        CopyFromExisting = 41,
        WorkorderNew = 45,
        WorkordersView = 46,
        WorkorderCopyToRevise = 47,
        WorkorderCopyToRedo = 48,

        SubmitRequestForConceptDrawing = 50,
        SubmitRequestForEstimation = 51,
        SubmitRequestForProjectDesign = 52,
        SubmitRequestForWorkorderDesign = 53,
        SubmitRequestForWorkorderApproval = 54,
        SubmitRequestForSpecialMaterial = 55,


        ReasonOfNoContract = 58,
        Unlock = 59,



        InvoiceNew = 80,
        InvoiceView = 88,
        InvoiceSync = 81,
        CopytoCreditNote = 82,
        ApproveInvoice = 83,

        //Item
        CopyToNewItem = 101,
        CopyToAnotherOption = 102,
        GenerateNewOption = 103,
        DeleteItem = 104,
        EditItem = 105,
        AddNewItem = 111,

        //View
      SaveCurrentSearchSettingAsDefault = 200
    }
}