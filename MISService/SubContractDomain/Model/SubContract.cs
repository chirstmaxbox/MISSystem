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

namespace SubContractDomain.Model
{
    public partial class SubContract
    {
        public SubContract()
        {
            this.SubcontractCommunications = new HashSet<SubcontractCommunication>();
            this.SubcontractShippings = new HashSet<SubcontractShipping>();
            this.SubcontractDocuments = new HashSet<SubcontractDocument>();
            this.SubcontractItems = new HashSet<SubcontractItem>();
            this.SubcontractNotes = new HashSet<SubcontractNote>();
            this.SubcontractWorkorders = new HashSet<SubcontractWorkorder>();
            this.SubcontractResponses = new HashSet<SubcontractResponse>();
        }
    
        public int SubcontractID { get; set; }
        public int JobID { get; set; }
        public System.DateTime RequestDate { get; set; }
        public int RequestBy { get; set; }
        public int StatusID { get; set; }
        public int ContactPerson1 { get; set; }
        public int ContactPerson2 { get; set; }
        public Nullable<double> Budget { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> TargetDate { get; set; }
        public string BudgetProvideBy { get; set; }
        public bool IsRush { get; set; }
        public int RequirementID { get; set; }
        public string Requirement { get; set; }
        public string ReviseReason { get; set; }
        public int RequestNumber { get; set; }
        public int Rating { get; set; }
        public string RatingRemark { get; set; }
        public Nullable<double> EstimatedShippingCost { get; set; }
        public Nullable<double> ItemValue { get; set; }
    
        public virtual TblSubcontractStatus TblSubcontractStatu { get; set; }
        public virtual TblSubcontractRequirement TblSubcontractRequirement { get; set; }
        public virtual ICollection<SubcontractCommunication> SubcontractCommunications { get; set; }
        public virtual ICollection<SubcontractShipping> SubcontractShippings { get; set; }
        public virtual ICollection<SubcontractDocument> SubcontractDocuments { get; set; }
        public virtual ICollection<SubcontractItem> SubcontractItems { get; set; }
        public virtual ICollection<SubcontractNote> SubcontractNotes { get; set; }
        public virtual ICollection<SubcontractWorkorder> SubcontractWorkorders { get; set; }
        public virtual ICollection<SubcontractResponse> SubcontractResponses { get; set; }
    }
    
}