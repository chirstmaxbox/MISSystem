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
    
    public partial class EST_Cost_Db_LabourHourGroup
    {
        public EST_Cost_Db_LabourHourGroup()
        {
            this.EST_Cost_Db_LabourHourProcedure = new HashSet<EST_Cost_Db_LabourHourProcedure>();
            this.tblWorkshopProductionProcessSteps = new HashSet<tblWorkshopProductionProcessStep>();
        }
    
        public int GroupID { get; set; }
        public int DisplayOrder { get; set; }
        public int DepartmentID { get; set; }
        public bool Active { get; set; }
        public int EstCostTypeID { get; set; }
        public string Name { get; set; }
        public int StatusID { get; set; }
        public bool IsEnterHour { get; set; }
        public bool IsSchedule { get; set; }
    
        public virtual ICollection<EST_Cost_Db_LabourHourProcedure> EST_Cost_Db_LabourHourProcedure { get; set; }
        public virtual ICollection<tblWorkshopProductionProcessStep> tblWorkshopProductionProcessSteps { get; set; }
    }
}
