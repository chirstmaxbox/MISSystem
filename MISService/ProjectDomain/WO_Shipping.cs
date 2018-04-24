//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjectDomain
{
    using System;
    using System.Collections.Generic;
    
    public partial class WO_Shipping
    {
        public int ShippingID { get; set; }
        public int WoID { get; set; }
        public string Carrier { get; set; }
        public string TrackingNumber { get; set; }
        public string WaybillNote { get; set; }
        public string ShipToName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Postcode { get; set; }
        public string AttnName { get; set; }
        public string AttnPhone { get; set; }
        public string WorkorderNumber { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string AeName { get; set; }
        public string AePhone { get; set; }
        public string InvoiceNumber { get; set; }
        public int NoteTypeID { get; set; }
        public string Total { get; set; }
    
        public virtual Sales_JobMasterList_WO Sales_JobMasterList_WO { get; set; }
    }
}
