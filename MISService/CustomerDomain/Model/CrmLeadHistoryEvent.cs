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
    
    public partial class CrmLeadHistoryEvent
    {
        public CrmLeadHistoryEvent()
        {
            this.CrmLeadHistories = new HashSet<CrmLeadHistory>();
        }
    
        public int EventID { get; set; }
        public string EventName { get; set; }
        public string Note { get; set; }
    
        public virtual ICollection<CrmLeadHistory> CrmLeadHistories { get; set; }
    }
}
