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
    
    public partial class WO_ShippingItem
    {
        public int ItemID { get; set; }
        public int WoID { get; set; }
        public int ItemNumber { get; set; }
        public string Qty { get; set; }
        public string Description { get; set; }
    
        public virtual Sales_JobMasterList_WO Sales_JobMasterList_WO { get; set; }
    }
}
