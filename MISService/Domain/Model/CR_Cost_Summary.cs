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
    
    public partial class CR_Cost_Summary
    {
        public long SummaryID { get; set; }
        public int ProjectID { get; set; }
        public int EstRevID { get; set; }
        public long EstItemID { get; set; }
        public int WorkorderID { get; set; }
        public long WorkorderItemID { get; set; }
        public int PrintingEmployeeID { get; set; }
        public int TypeID { get; set; }
        public int OrderNumber { get; set; }
        public string Name { get; set; }
        public string Column0 { get; set; }
        public string Column1 { get; set; }
        public string Column2 { get; set; }
    
        public virtual Sales_JobMasterList_EstRev Sales_JobMasterList_EstRev { get; set; }
    }
}
