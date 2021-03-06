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
    public partial class tblTimeCard_Shift
    {
        public tblTimeCard_Shift()
        {
            this.FW_Employee_Shift = new HashSet<FW_Employee_Shift>();
        }
    
        public int SHIFT_ID { get; set; }
        public string SHIFT_NAME { get; set; }
        public System.DateTime SHIFT_START { get; set; }
        public System.DateTime SHIFT_START_WEEKEND { get; set; }
        public System.DateTime SHIFT_END { get; set; }
        public System.DateTime SHIFT_END_WEEKEND { get; set; }
        public Nullable<System.DateTime> MEAL_START { get; set; }
        public Nullable<System.DateTime> MEAL_END { get; set; }
        public bool MEAL_PAY { get; set; }
        public Nullable<System.DateTime> BREAK_START1 { get; set; }
        public Nullable<System.DateTime> BREAK_END1 { get; set; }
        public bool BREAK_PAY1 { get; set; }
        public Nullable<System.DateTime> BREAK_START2 { get; set; }
        public Nullable<System.DateTime> BREAK_END2 { get; set; }
        public bool BREAK_PAY2 { get; set; }
        public short SHIFT_START_TYPE { get; set; }
        public double OVERTIME_RATE { get; set; }
        public string EARNING_CODE { get; set; }
        public bool ACTIVE { get; set; }
        public bool WEEKEND_PAY_FIXSALARY { get; set; }
        public System.DateTime OVERTIME_START { get; set; }
        public short SHIFT_END_TYPE { get; set; }
    
        public virtual ICollection<FW_Employee_Shift> FW_Employee_Shift { get; set; }
    }
    
}
