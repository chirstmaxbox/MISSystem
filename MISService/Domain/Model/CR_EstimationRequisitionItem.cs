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
    
    public partial class CR_EstimationRequisitionItem
    {
        public long ID { get; set; }
        public long EstItemID { get; set; }
        public int PrintingEmployeeID { get; set; }
        public int OrderNumber { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
    
        public virtual EST_Item EST_Item { get; set; }
    }
}
