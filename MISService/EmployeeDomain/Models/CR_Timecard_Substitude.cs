//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EmployeeDomain.Models
{
    public partial class CR_Timecard_Substitude
    {
        public int TempID { get; set; }
        public int PrintingEmployeeID { get; set; }
        public string EmployeeID { get; set; }
        public int DepartmentID { get; set; }
        public int HolidayID { get; set; }
        public string eName { get; set; }
        public long TRANSACTION_ID { get; set; }
        public System.DateTime CLOCK_IN { get; set; }
        public System.DateTime CLOCK_OUT { get; set; }
    
        public virtual FW_Employees FW_Employees { get; set; }
    }
    
}
