//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SpecDomain.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class EST_Cost_Bid_Type_Parent
    {
        public EST_Cost_Bid_Type_Parent()
        {
            this.EST_Cost_Bid_Type_Children = new HashSet<EST_Cost_Bid_Type_Children>();
            this.EST_Cost_Bid_ParentItem = new HashSet<EST_Cost_Bid_ParentItem>();
        }
    
        public int ParentTypeID { get; set; }
        public string ParentTypeName { get; set; }
        public int CategoryID { get; set; }
        public double ParentOrderNumber { get; set; }
    
        public virtual ICollection<EST_Cost_Bid_Type_Children> EST_Cost_Bid_Type_Children { get; set; }
        public virtual ICollection<EST_Cost_Bid_ParentItem> EST_Cost_Bid_ParentItem { get; set; }
    }
}
