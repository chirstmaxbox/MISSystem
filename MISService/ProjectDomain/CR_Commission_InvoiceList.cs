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
    
    public partial class CR_Commission_InvoiceList
    {
        public int ID { get; set; }
        public int PrintingEmployeeID { get; set; }
        public int EmployeeID { get; set; }
        public string SalesID { get; set; }
        public int AppliedRuleID { get; set; }
        public int ProductLine { get; set; }
        public int CategoryID { get; set; }
        public int InvoiceID { get; set; }
        public string InvoiceNumber { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public System.DateTime ZeroDate { get; set; }
        public double SalesAmount { get; set; }
        public Nullable<System.DateTime> FirstInvoiceDate { get; set; }
        public int CustomerRowID { get; set; }
        public string CustomerName { get; set; }
        public int ProjectID { get; set; }
        public bool IsAbnormal { get; set; }
        public string AbnormalReason { get; set; }
    
        public virtual Sales_JobMasterList_Invoice Sales_JobMasterList_Invoice { get; set; }
    }
}
