//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CustomerDomain.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class CrmLeadHistory
    {
        public int HistoryID { get; set; }
        public int EmployeeNumber { get; set; }
        public System.DateTime Date { get; set; }
        public int EventID { get; set; }
        public int LeadID { get; set; }
    
        public virtual CrmLead CrmLead { get; set; }
        public virtual CrmLeadHistoryEvent CrmLeadHistoryEvent { get; set; }
        public virtual FW_Employees FW_Employees { get; set; }
    }
}
