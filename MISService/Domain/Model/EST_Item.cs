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
    
    public partial class EST_Item
    {
        public EST_Item()
        {
            this.CR_EstimationRequisitionItem = new HashSet<CR_EstimationRequisitionItem>();
            this.EST_Item_Drawing = new HashSet<EST_Item_Drawing>();
            this.EST_Item_Specification = new HashSet<EST_Item_Specification>();
            this.EST_Item_Specification_Size = new HashSet<EST_Item_Specification_Size>();
            this.Sales_Dispatching_DrawingRequisition_EstimationItem = new HashSet<Sales_Dispatching_DrawingRequisition_EstimationItem>();
            this.EST_Cost = new HashSet<EST_Cost>();
        }
    
        public long EstItemID { get; set; }
        public int EstRevID { get; set; }
        public int SerialID { get; set; }
        public short EstItemNo { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Version { get; set; }
        public short ItemOption { get; set; }
        public bool IsFinalOption { get; set; }
        public int StatusID { get; set; }
        public bool BySubcontractor { get; set; }
        public int ItemPurposeID { get; set; }
        public int RequirementID { get; set; }
        public int PositionID { get; set; }
        public int Qty { get; set; }
        public int QtyPC { get; set; }
        public string Permit { get; set; }
        public string Description { get; set; }
        public bool IsHide { get; set; }
        public Nullable<double> EstimatorPrice { get; set; }
        public string SpecialRequirement { get; set; }
        public string Remark { get; set; }
        public bool contentsChanged { get; set; }
        public int DirectionID { get; set; }
        public bool IsTemplateApplicable { get; set; }
        public bool IsThereSpecialMaterial { get; set; }
        public string SpecialMaterial { get; set; }
        public int SpecialMaterialLeadTime { get; set; }
        public int IsPreviousEstimationAvailable { get; set; }
        public string ReportDescription { get; set; }
        public bool IsValidated { get; set; }
        public string ErrorMessage { get; set; }
        public string Size { get; set; }
        public int SizeRows { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Thickness { get; set; }
        public int EstPart { get; set; }
        public Nullable<double> PriceA { get; set; }
        public Nullable<double> PriceB { get; set; }
        public Nullable<double> PriceExtra { get; set; }
        public string BidSignIdCode { get; set; }
        public string BidReferenceDrawing { get; set; }
        public string BidDescription { get; set; }
        public string BidRemark { get; set; }
        public string SalesDescription { get; set; }
        public Nullable<double> SupplyA { get; set; }
        public Nullable<double> InstallationA { get; set; }
        public Nullable<double> UnitPriceA { get; set; }
        public Nullable<double> SupplyB { get; set; }
        public Nullable<double> InstallationB { get; set; }
        public Nullable<double> UnitPriceB { get; set; }
    
        public virtual ICollection<CR_EstimationRequisitionItem> CR_EstimationRequisitionItem { get; set; }
        public virtual ICollection<EST_Item_Drawing> EST_Item_Drawing { get; set; }
        public virtual EST_Item_Status EST_Item_Status { get; set; }
        public virtual EST_Item_TablePurpose EST_Item_TablePurpose { get; set; }
        public virtual FW_JOB_TYPE FW_JOB_TYPE { get; set; }
        public virtual Product Product { get; set; }
        public virtual RequiredItemPosition RequiredItemPosition { get; set; }
        public virtual Sales_JobMasterList_EstRev Sales_JobMasterList_EstRev { get; set; }
        public virtual ICollection<EST_Item_Specification> EST_Item_Specification { get; set; }
        public virtual ICollection<EST_Item_Specification_Size> EST_Item_Specification_Size { get; set; }
        public virtual ICollection<Sales_Dispatching_DrawingRequisition_EstimationItem> Sales_Dispatching_DrawingRequisition_EstimationItem { get; set; }
        public virtual ICollection<EST_Cost> EST_Cost { get; set; }
    }
}
