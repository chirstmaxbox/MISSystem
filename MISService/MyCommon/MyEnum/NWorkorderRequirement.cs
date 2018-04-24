namespace MyCommon.MyEnum
{
    public enum NWorkorderRequirement
    {
        //workorder 1507
        Installation = 10,
        Service = 20,
        SiteCheck = 30,
	
//           TakeDownDispose=41,
//TakeDownDelivertoClient=42,
//TakeDownStorage=43,
//TakeDownModify=	44,
//TakeDownReLocate=50
        

        Fabrication = 55,               //Supply and Install by Sub-contractor
        //		 InstallationOnly = 60,
        DeliveryWithinGTA=70,
        DeliverywithinGTAWithCrating=71,
        DeliverOutsideGTA=80,
        DeliverOutsideGTAWithCrating=81,
        PickupByClient=	90,	
        Retrofit=	95,



    }
}