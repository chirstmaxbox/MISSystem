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
    
    public partial class tblWipTask
    {
        public tblWipTask()
        {
            this.Sales_Wip = new HashSet<Sales_Wip>();
        }
    
        public int ContentID { get; set; }
        public string ContentName { get; set; }
        public short OptionType { get; set; }
        public int LeadTime { get; set; }
        public int StageID { get; set; }
        public bool Active { get; set; }
        public int StartDateAssignType { get; set; }
        public int FinishDateAssignType { get; set; }
        public int StartDatePriorTo { get; set; }
        public bool IsInternal { get; set; }
        public bool IsPublic { get; set; }
        public string PublicContentName { get; set; }
    
        public virtual ICollection<Sales_Wip> Sales_Wip { get; set; }
    }
}
