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
    public partial class FW_Department1
    {
        public FW_Department1()
        {
            this.FW_Employees = new HashSet<FW_Employees>();
        }
    
        public int DEPARTMENTID { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string PARENT_DEPARTMENT { get; set; }
        public string NameInInfor { get; set; }
        public Nullable<int> REPORT_ID { get; set; }
        public string Category { get; set; }
        public Nullable<short> deptLevel { get; set; }
        public Nullable<bool> ncrPurpose { get; set; }
        public Nullable<short> wSelect { get; set; }
        public short InputHourSelect { get; set; }
        public string InputHourName { get; set; }
        public Nullable<int> supervisorID { get; set; }
        public Nullable<bool> pSelect { get; set; }
        public bool Active { get; set; }
        public bool AdminTaskSelect { get; set; }
    
        public virtual ICollection<FW_Employees> FW_Employees { get; set; }
    }
    
}
