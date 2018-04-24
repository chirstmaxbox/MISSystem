namespace SpecDomain.BO
{

    public enum NEstCostBidSummarySheetNoteTypeID : int
    {
        PriceIncluded = 1,
        PriceExcluded = 2
    }

    public enum NEstCostBidTypeSummaryCategoryID : int
    {
        PriceBOnly=1,
        PriceAandPriceB=2,
        GrandTotal=3,
    }

    public enum EstCostBidTypeChildrenDistributionToItemGroupID : int
    {
        TotalShopCostOfMaterialOutsourcingAndStandardItem=111,
        BidCertificate = 115,
        Travel = 121,
        SiteInspection = 132,
    }

    public enum NEstCostBidEstCostDbOther : int
    {
        PermitFee = 1042,
        HoistingPermitFee = 1044,
        DutycopTrafficControl = 1045,
        ElectricalHookup = 1046,
        ConstructionBin = 1047,
        Concrete = 1049,

        LabourCostForCrating = 1001,
        MaterailCostForCrating = 1050
    }


    public enum NEstCostBidCategoryID : int
    {
        Null = 0,
        IndirectCost = 1,
        DirectCost = 2
    }

    public enum NEstCostBidSummaryTypeID : int
    {
        NULL = 0,
        //Part one
        ShopDirectCost = 1,
        ShopInDirectCost = 2,
        ShopTotalCost = 3,

        //Part Two
        Overhead = 4,
        TargetProfit = 5,
        SupplyWithCrating = 6,
        StandardItems = 7,
        Shipping = 8,
        TravelAllowance = 9,
        OwnInstaller = 10,
        SubcontractInstaller = 11,

        //Grand Total
        TargetPriceBySummary = 12,
        TargetPriceByItems = 13,
        Discount = 14,
        FinalGrandTotal = 15,

    }


    public enum NEstCostBidParentItemTypeID : int
    {
        Null = 0,
        Office = 10,
        Shop = 12,
        OtherGeneralCost = 14,
        IndirectCostSummary=98,

        Supply=110,
        Crating = 112,
        Shipping = 114,
        Installation = 116,

        DirectCostSummary   =198

    }

    public enum NEstCostBidChildrenItemTypeID : int
    {
        Null = 0,
        //10.Office
        OfficeEstimation = 10,
        OfficeInternalMeeting = 11,
        OfficeExternalMeetingWithClient = 12,
        OfficeStructuralDrawing = 13,
        OfficeGraphicDrawing = 14,
        OfficeArrangeStakeout = 15,
        OfficePermitApplication = 16,
        OfficeDocumentPreparation = 17,
        OfficeProjectManagement = 18,
        OfficePurchasePricing = 19,
        OfficePurchaseHandling = 20,
        OfficeSiteLocated = 21,
        OfficeInspection = 22,

        //12.Shop
        ShopPlanningShop = 23,
        ShopQualityControlShop = 24,
        ShopLocalInstallerHandlingShop = 25,

        //14. Other General Cost
        OfficeSubtotal = 26,
        ShopSubtotal = 27,
        OtherGeneralCostSubtotal = 28,

        //Indirect Cost extra cost
        OfficeOther = 29,
        ShopOther = 30,
        OtherOther = 31,

        //Summary
        IndirectCostTotal = 90,
        RatioOfIndirectCostVsIndirectCost = 91,
        IndirectCostOfStandarditems = 92,

        //Supply
        SupplyShopLabourHoursCost = 110,
        SupplyShopMaterialCost = 111,
        SupplyStandardItems = 112,
        SupplyEngineeringStamp = 113,
        SupplyOutsource = 114,
        SupplyCertificateOfSubstantialPerformance = 115,
        SupplyBiddingFee = 116,
        SupplyBidBond = 117,

        //Crating
        CratingLabourHourCost = 118,
        CratingMaterialCost = 119,

        //Shipping
        ShippingShipping = 120,

        //Installation
        InstallationAirTicket = 121,
        InstallationAccommodation = 122,
        InstallationTravelAllowance = 123,
        InstallationVehicleRental = 124,
        InstallationPermitFee = 125,
        InstallationHoistingPermitFee = 126,
        InstallationDutycopTrafficControl = 127,
        InstallationElectricalHookup = 128,
        InstallationMaterialCost = 129,
        InstallationConstructionBin = 130,
        InstallationConcrete = 131,
        InstallationSiteCheck = 132,
        InstallationLocate = 133,
        InstallationInspection = 134,
        InstallationOwnInstallerHoursCost = 135,
        InstallationOwnEquipmentCost = 136,
        InstallationSubcontractorCost = 137,

        //Direct Cost Total
        DirectCostTotal = 198,

    }

    
    public enum NEstItemPurpose : short
    {
        ForEstimation = 0,
        ForBackup = 10
    }

    public enum NEstItemStatus : int
    {
        New = 0,
        Submitted = 1,
        Working = 2,
        Problem = 10,
        Estimated = 20,
        ContentsChanged=30,
    }

    public enum NYesNoNotApplicable: int
    {
        NotApplicable = 0,
        Yes = 1,
        No = 2,
    }

    public enum NEstPosition : int
    {
        //Null = 0,
        Indoor = 10,
        Outdoor = 20
    }

    public enum NEstCostTypeCategory : int
    {
        System = 0,
        ShopCostItem = 10,
        InstallationCostItem = 20,
        CratingAndShipping = 25,
        LocalInstallerCost = 30,
    }

    
    public enum NEstCostType : int
    {
        System = 0,

        ShopLabour = 100,
        ShopMaterail = 110,
        ShopSubcontract = 120,
        ShopOther = 130,
        ShopStandItem = 140,

        InstallationLabour = 200,
        
        InstallationMaterail = 210,
        InstallationEquipment = 220,
        InstallationOther = 230,
    
        InstallationTraveling = 240,

        
        CratingCost = 300,
        PermitCost = 305,
        ShippingCost = 310,

        LocalInstallerLabour = 320,
        LocalInstallerMaterials = 330,
        LocalInstallerEquipmentRental = 340,
        LocalInstallerOther = 350,

    }

    public enum NTemplateDbTypeID : int
    {
        Material = 10,
        Labour = 20,
        Other = 30,
    }

    public enum NLabourHourPosition : int
    {
        Null = 0,
        MetalWorker = 1002,
        PaintingWorker = 1003,
        PlasticAandPlasticBWorker = 1004,
        RA3Worker = 1006,
        InstallationInstaller = 1007,
        SiteCheckInspector = 1008,
        StructureTechnician = 1046,
        VinylWorker = 1061,
        Artist = 1204,
        Office = 1205,
        Estimator = 1207,
        Sales = 1208
    }

    public enum NLabourHourProcedure : int
    {
        Null = 0,
        Ra3CutFace = 101,
        Ra3FormLetter = 102,
        Ra3Router = 103,
        Ra3TrimCap = 104,
        RA3Other = 105,
        RA33D = 106,
        MetalAnchorBolt = 201,
        MetalBox = 202,
        MetalTempPlate = 203,
        MetalLetter = 204,
        MetalPole = 205,
        MetalWiring = 206,
        MetalOther = 207,
        Painting = 280,
        PlasticAwing = 401,
        PlasticLaminate = 402,
        PlasticPinPattern = 403,
        PlasticPlastic = 404,
        PlasticApplyVinyl = 405,
        PlasticOther = 406,
        PlasticWiring = 407,
        PlasticBanner = 408,
        PlasticAssemble=409,
        ArtRoomPattern = 501,
        ArtRoomDrawing = 502,
        ArtRoomCutVinyl = 503,
        OtherIndirectCost=550,
        SiteCheck = 901,
        TakeDown = 911,
        Install = 912,
        Travelling = 913,
        LocalInstallerInstall = 1001,
        LocalInstallerSiteCheck = 1002,
        LocalInstallerShopHours = 1003,
    }

    public enum NEstCostSummaryType : int
    {
        PriceExtra = 1,
        PriceA = 10,
        PriceB = 20,
    }

    public enum NEstCostSummaryPriceA : int
    {
        Null = 0,
        ShopMaterialCost = 10,
        ShopLabourCost = 20,
        SupplyOnly = 30,
        TravellingCost = 40,
        InstallationCost = 50,
        TargetPriceA = 60,
    }


    public enum NEstCostSummaryExtra : int
    {
        Null = 0,
        Crating = 1,
        Shipping = 2,
        LocalInstaller = 3,
        TotalOfCSL = 4,

    }

    public enum NEstCostSummaryPriceB : int
    {
        Null = 0,
        //DirectCost = 10,
        //Commission = 20,
        //TotalDirectCost = 30,
        //OverheadCost = 40,
        //TotalCost = 50,
        //TargetProfit = 60,
        //SupplyOnly = 70,
        //TravellingCost = 80,
        //InstallationCost = 90,
        TargetPriceB = 14
    }

    public enum NCostReportTypeID : int
    {
        ProjectID = 10,
        EstRevID = 20,
        EstItemID = 30,
        WorkorderID = 40,
        WorkorderItemID = 50
    }

    public enum NDrawingPurposeStep1 : int
    {
        Estimation = 102,
        PermitDrawing = 103,
        WorkorderDrawing = 104,
        ConceptDesign = 105
    }


    
#region ********************* Material *******************************

    public enum NMaterialStatus : int
    {
        New = 0,
        Approved = 1,
    }

    public enum NMaterialPriceSyncMethod : int
    {
        Average = 0,
        Max = 1,
        Mini = 2,
    }

   public enum NMaterialPriceSyncFrom : int
    {
        PriceSyncTable = 0,     //To Be Sync MaterialID==>list of MaterialID from
        SiblingChildren = 1,
    }

    public enum NMaterialCategoryType : int
    {
        Category = 0,
        SubCategory1 = 1,
        SubCategory2 = 2,
        SubCategory3 = 3,
        SubCategory4 = 4,
    }


    public enum NMaterialGenerateType : int
    {
        ByMaterialAdministrator = 0,
        ByEstimator = 1,
        ByWorker = 2,
    }


    public enum NMaterialCategory0 : int
    {
        Null = 0,
        Vinyl=119,
        Estimation=156,
    }

    public enum NMaterialUnitID : int
    {
        Null = 0,
        SquareFeet = 124,

    }

    

#endregion
}