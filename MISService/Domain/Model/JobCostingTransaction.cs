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
    
    public partial class JobCostingTransaction
    {
        public int TransactionID { get; set; }
        public int JobID { get; set; }
        public int WoID { get; set; }
        public int WoItemID { get; set; }
        public int EstCostTypeID { get; set; }
        public int MaterialID { get; set; }
        public double Count { get; set; }
        public int PriceUnitID { get; set; }
        public double Amount { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public short TransactionType { get; set; }
        public int DepartmentID { get; set; }
        public int EmployeeID { get; set; }
        public string Note { get; set; }
        public string PO { get; set; }
        public int ProcedureGroupID { get; set; }
        public Nullable<double> Count1 { get; set; }
        public Nullable<double> Amount1 { get; set; }
    
        public virtual EST_Cost_Type EST_Cost_Type { get; set; }
        public virtual Material Material { get; set; }
        public virtual Sales_JobMasterList_WO Sales_JobMasterList_WO { get; set; }
    }
}
