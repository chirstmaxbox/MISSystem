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
    
    public partial class TblQuoteFollowupStatu
    {
        public TblQuoteFollowupStatu()
        {
            this.Sales_JobMasterList_QuoteFollow = new HashSet<Sales_JobMasterList_QuoteFollow>();
        }
    
        public int StatusID { get; set; }
        public string StatusName { get; set; }
    
        public virtual ICollection<Sales_JobMasterList_QuoteFollow> Sales_JobMasterList_QuoteFollow { get; set; }
    }
}
