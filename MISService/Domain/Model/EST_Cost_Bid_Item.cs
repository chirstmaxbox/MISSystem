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
    
    public partial class EST_Cost_Bid_Item
    {
        public int RowID { get; set; }
        public int ChildrenTypeID { get; set; }
        public int EstRevID { get; set; }
        public int OrderNumber { get; set; }
        public string Unit { get; set; }
        public double Qty { get; set; }
        public double UnitPriceA { get; set; }
        public Nullable<double> SubTotalA { get; set; }
        public double UnitPriceB { get; set; }
        public Nullable<double> SubTotalB { get; set; }
        public string OtherItemName { get; set; }
    
        public virtual EST_Cost_Bid_Type_Children EST_Cost_Bid_Type_Children { get; set; }
        public virtual Sales_JobMasterList_EstRev Sales_JobMasterList_EstRev { get; set; }
    }
}
