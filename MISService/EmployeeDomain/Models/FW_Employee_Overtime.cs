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
    public partial class FW_Employee_Overtime
    {
        public int OvertimeID { get; set; }
        public int EmployeeNumber { get; set; }
        public Nullable<System.DateTime> StartWorkingTime { get; set; }
        public Nullable<System.DateTime> EndWorkingTime { get; set; }
        public int Status { get; set; }
        public int ApplyBy { get; set; }
        public Nullable<System.DateTime> ApplyAt { get; set; }
        public int ApproveBy { get; set; }
        public Nullable<System.DateTime> ApproveAt { get; set; }
        public string Remark { get; set; }
        public bool IsMinusDinnerBreak { get; set; }
        public bool IsLocked { get; set; }
    
        public virtual FW_Employees FW_Employees { get; set; }
    }
    
}
