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
    
    public partial class FW_QUOTE_IC
    {
        public FW_QUOTE_IC()
        {
            this.CrmLeads = new HashSet<CrmLead>();
            this.CUSTOMERs = new HashSet<CUSTOMER>();
        }
    
        public int QIC_ID { get; set; }
        public string QIC_CONTENTS { get; set; }
    
        public virtual ICollection<CrmLead> CrmLeads { get; set; }
        public virtual ICollection<CUSTOMER> CUSTOMERs { get; set; }
    }
}
