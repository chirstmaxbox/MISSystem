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
    
    public partial class Sales_SpecialMaterialRequest
    {
        public int SpmID { get; set; }
        public int JobID { get; set; }
        public System.DateTime SubmitTime { get; set; }
        public int SubmitBy { get; set; }
        public bool Estimation { get; set; }
        public bool StructuralDrawing { get; set; }
        public bool GraphicDrawing { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> RequiredDate { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> EstimationSubmitDate { get; set; }
        public Nullable<System.DateTime> StructuralDrawingSubmitDate { get; set; }
        public Nullable<System.DateTime> GraphicDrawingSubmitDate { get; set; }
        public Nullable<System.DateTime> SubmitToPurchasingDate { get; set; }
    
        public virtual Sales_JobMasterList Sales_JobMasterList { get; set; }
    }
}
