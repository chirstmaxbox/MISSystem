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
    
    public partial class CR_EstCostBidSummary
    {
        public int SummaryRowID { get; set; }
        public int EstRevID { get; set; }
        public int SummaryTypeID { get; set; }
        public double PaSubtotal { get; set; }
        public double Supply { get; set; }
        public double Shipping { get; set; }
        public double Travelling { get; set; }
        public double Installation { get; set; }
        public int TempInt1 { get; set; }
        public long EstItemID { get; set; }
        public int PrintingEmployeeID { get; set; }
    
        public virtual EST_Cost_Bid_Type_Summary EST_Cost_Bid_Type_Summary { get; set; }
        public virtual Sales_JobMasterList_EstRev Sales_JobMasterList_EstRev { get; set; }
    }
}
