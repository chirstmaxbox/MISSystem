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
    public partial class tblTimeCard_Holiday
    {
        public tblTimeCard_Holiday()
        {
            this.FW_Employee_Substitude = new HashSet<FW_Employee_Substitude>();
        }
    
        public int HolidayID { get; set; }
        public int Year { get; set; }
        public string HolidayName { get; set; }
        public System.DateTime DateOfHoliday { get; set; }
        public System.DateTime ActualDayOff { get; set; }
    
        public virtual ICollection<FW_Employee_Substitude> FW_Employee_Substitude { get; set; }
    }
    
}
