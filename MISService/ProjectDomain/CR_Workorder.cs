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
    
    public partial class CR_Workorder
    {
        public int ID { get; set; }
        public int PrintingEmployeeID { get; set; }
        public int JobID { get; set; }
        public int WoID { get; set; }
        public string WorkorderNumber { get; set; }
        public string WorkorderTitle { get; set; }
        public System.DateTime WorkorderDate { get; set; }
        public double Amount { get; set; }
        public int RecordType { get; set; }
        public int SalesID { get; set; }
        public int PcID { get; set; }
        public int ShowingNumber { get; set; }
        public string SalesName { get; set; }
        public bool IsInvoiceIssued { get; set; }
        public int TempInt { get; set; }
        public string TempString { get; set; }
        public bool TempBool { get; set; }
    
        public virtual FW_WORK_ORDER FW_WORK_ORDER { get; set; }
    }
}
