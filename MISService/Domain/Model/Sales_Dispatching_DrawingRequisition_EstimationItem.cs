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
    
    public partial class Sales_Dispatching_DrawingRequisition_EstimationItem
    {
        public int RequisitionItemID { get; set; }
        public int RequisitionID { get; set; }
        public long EstItemID { get; set; }
        public int Status { get; set; }
        public string Qty { get; set; }
        public string PowerVoltage { get; set; }
        public string Description { get; set; }
        public string Remark1 { get; set; }
        public string Remark2 { get; set; }
        public string ItemName { get; set; }
        public bool IsIncludedWhenPrint { get; set; }
    
        public virtual EST_Item_Status EST_Item_Status { get; set; }
        public virtual Sales_Dispatching_DrawingRequisition_Estimation Sales_Dispatching_DrawingRequisition_Estimation { get; set; }
        public virtual EST_Item EST_Item { get; set; }
    }
}
