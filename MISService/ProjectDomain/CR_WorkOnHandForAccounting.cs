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
    
    public partial class CR_WorkOnHandForAccounting
    {
        public long ID { get; set; }
        public int PrintingEmployeeID { get; set; }
        public int JobID { get; set; }
        public string JobTitle { get; set; }
        public int BillToCustomerID { get; set; }
        public string BillToCustomerName { get; set; }
        public int InstallToCustomerID { get; set; }
        public string InstallToAddress { get; set; }
        public System.DateTime TargetDate { get; set; }
        public Nullable<double> EstimatedCost { get; set; }
        public int QuoteRevID { get; set; }
        public Nullable<decimal> ContractAmount { get; set; }
        public Nullable<double> BilledToDate { get; set; }
        public Nullable<double> InstallationHours { get; set; }
        public Nullable<double> ProductionHours { get; set; }
    
        public virtual Sales_JobMasterList Sales_JobMasterList { get; set; }
    }
}
