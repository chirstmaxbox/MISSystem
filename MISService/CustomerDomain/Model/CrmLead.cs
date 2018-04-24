//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CustomerDomain.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class CrmLead
    {
        public CrmLead()
        {
            this.CrmActivityMarketings = new HashSet<CrmActivityMarketing>();
            this.CrmLeadContacts = new HashSet<CrmLeadContact>();
            this.CrmLeadCustomers = new HashSet<CrmLeadCustomer>();
            this.CrmLeadDocuments = new HashSet<CrmLeadDocument>();
            this.CrmLeadHistories = new HashSet<CrmLeadHistory>();
            this.CrmLeadOthers = new HashSet<CrmLeadOther>();
        }
    
        public int LeadID { get; set; }
        public int AddressID { get; set; }
        public string CompanyName { get; set; }
        public string WebPage { get; set; }
        public int StageID { get; set; }
        public Nullable<System.DateTime> CriticalDeadline { get; set; }
        public string Referral { get; set; }
        public int OriginalOwner { get; set; }
        public int AccountExcutive { get; set; }
        public int SourceID { get; set; }
        public string KeyProductRequirement { get; set; }
        public Nullable<double> PotentialRevenue { get; set; }
        public string ChanceOfSale { get; set; }
        public int ProgressID { get; set; }
        public string ProgressContents { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public int LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedAt { get; set; }
        public Nullable<System.DateTime> AeAssignedAt { get; set; }
        public Nullable<System.DateTime> UnQualifiedBySalesAt { get; set; }
        public Nullable<System.DateTime> ProjectCreatedAt { get; set; }
        public Nullable<System.DateTime> EstRequirementSubmit { get; set; }
        public Nullable<System.DateTime> LastContactDate { get; set; }
        public int RatingID { get; set; }
        public int ContactID { get; set; }
        public int CategoryID { get; set; }
    
        public virtual ICollection<CrmActivityMarketing> CrmActivityMarketings { get; set; }
        public virtual FW_QUOTE_IC FW_QUOTE_IC { get; set; }
        public virtual CrmLeadAddress CrmLeadAddress { get; set; }
        public virtual CrmLeadContact CrmLeadContact { get; set; }
        public virtual CrmLeadContentsProgress CrmLeadContentsProgress { get; set; }
        public virtual CrmStage CrmStage { get; set; }
        public virtual FW_QUOTE_SOURCE FW_QUOTE_SOURCE { get; set; }
        public virtual ICollection<CrmLeadContact> CrmLeadContacts { get; set; }
        public virtual ICollection<CrmLeadCustomer> CrmLeadCustomers { get; set; }
        public virtual ICollection<CrmLeadDocument> CrmLeadDocuments { get; set; }
        public virtual ICollection<CrmLeadHistory> CrmLeadHistories { get; set; }
        public virtual ICollection<CrmLeadOther> CrmLeadOthers { get; set; }
        public virtual CrmLeadRating CrmLeadRating { get; set; }
        public virtual FW_Employees FW_Employees { get; set; }
    }
}
