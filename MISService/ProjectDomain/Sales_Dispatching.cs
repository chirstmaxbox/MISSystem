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
    
    public partial class Sales_Dispatching
    {
        public long TaskID { get; set; }
        public short TaskType { get; set; }
        public string RequestType { get; set; }
        public int JobID { get; set; }
        public int EstRevID { get; set; }
        public Nullable<int> QuoteRevID { get; set; }
        public Nullable<int> WoID { get; set; }
        public Nullable<int> InvoiceID { get; set; }
        public int SubmitBy { get; set; }
        public short Responsible { get; set; }
        public Nullable<short> CopyTo { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public short Status { get; set; }
        public System.DateTime SubmitTime { get; set; }
        public Nullable<System.DateTime> LastUpdateTime { get; set; }
        public System.DateTime RequiredTime { get; set; }
        public bool Rush { get; set; }
        public Nullable<short> Priority { get; set; }
        public Nullable<short> Importance { get; set; }
        public Nullable<System.DateTime> Reminder { get; set; }
        public Nullable<decimal> CompletePercentage { get; set; }
        public Nullable<bool> OwnerFinish { get; set; }
        public Nullable<bool> ResponsibleFinish { get; set; }
        public Nullable<System.DateTime> FinishedDate { get; set; }
        public Nullable<double> WorkedHour { get; set; }
        public Nullable<int> NumberOfDrawing { get; set; }
        public string Note { get; set; }
        public bool IncompleteInfo { get; set; }
        public Nullable<System.DateTime> IIMarkedTime { get; set; }
        public Nullable<System.DateTime> IIOkTime { get; set; }
        public string IIReason { get; set; }
    
        public virtual Sales_JobMasterList Sales_JobMasterList { get; set; }
        public virtual FW_Employees FW_Employees { get; set; }
    }
}
