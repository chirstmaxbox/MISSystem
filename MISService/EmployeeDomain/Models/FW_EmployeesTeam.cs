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
    public partial class FW_EmployeesTeam
    {
        public FW_EmployeesTeam()
        {
            this.FW_Employees = new HashSet<FW_Employees>();
        }
    
        public int teamID { get; set; }
        public string teamName { get; set; }
        public string teamNote { get; set; }
        public Nullable<short> teamType { get; set; }
        public Nullable<bool> Presentation { get; set; }
        public string AbbrName { get; set; }
        public int Estimation { get; set; }
        public int Artist { get; set; }
        public int Engineer { get; set; }
        public int woApproval { get; set; }
        public bool Replace { get; set; }
        public int EstimatorReplace { get; set; }
        public int ArtistReplace { get; set; }
        public int EngineerReplace { get; set; }
        public int woApprovalReplace { get; set; }
        public bool IsShowOnTV { get; set; }
    
        public virtual ICollection<FW_Employees> FW_Employees { get; set; }
    }
    
}
