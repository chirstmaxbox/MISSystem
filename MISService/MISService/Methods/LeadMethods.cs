using CustomerDomain.Model;
using MISService.Method;
using MISService.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISService.Methods
{
    public class LeadMethods
    {
        private readonly CustomerDbEntities _db = new CustomerDbEntities();
        public void CreateLead(LeadModifyViewMode createViewModel, string username)
        {
            try
            {
                var lead = createViewModel.Lead;
                //Svae Address
                _db.CrmLeadAddresses.Add(createViewModel.Address);
                _db.SaveChanges();
                var addressID = createViewModel.Address.AddressID;

                //Save Lead                
                lead.AddressID = addressID;
                var emp = new CrmEmployee(username);
                lead.CreatedBy = emp.UserEmployeeID;
                lead.CreatedAt = DateTime.Now;
                lead.LastUpdatedBy = emp.UserEmployeeID;
                lead.LastUpdatedAt = DateTime.Now;
                //status
                if (lead.AccountExcutive > 0)
                {
                    lead.AeAssignedAt = DateTime.Now;
                }
                else
                {
                    lead.AeAssignedAt = null;
                }
                _db.CrmLeads.Add(lead);
                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                LogMethods.Log.Error("CreateLead:Crash:" + dbEx.Message);
            }
        }

        private void UpdateHistory(int leadID, int employeeID, int eventID)
        {
            var history = new CrmLeadHistory()
            {
                LeadID = leadID,
                EmployeeNumber = employeeID,
                EventID = eventID,
                Date = DateTime.Now
            };

            _db.CrmLeadHistories.Add(history);
            _db.SaveChanges();
        }

        //45	UnQualified By Marketing	NULL
        //51	Quote Request	NULL
        //52	AE Assigned	NULL
        //81	UnQualified By Sales	NULL
        private void RecordLeadStatusChangdTime(CrmLead newLead)
        {
            var eventID = 0;
            switch (newLead.StageID)
            {
                case (int)LeadENumber.NStage.UnQualifiedBySales:
                    eventID = (int)LeadENumber.NHistoryEvent.UnQualifiedBySales;
                    break;
                case (int)LeadENumber.NStage.UnQualifiedByMarketing:
                    eventID = (int)LeadENumber.NHistoryEvent.UnQualifiedByMarketing;
                    break;
                case (int)LeadENumber.NStage.QuoteRequest:
                    eventID = (int)LeadENumber.NHistoryEvent.QuoteRequest;
                    break;
                default:
                    break;
            }

            if (eventID != 0)
            {
                UpdateHistory(newLead.LeadID, newLead.LastUpdatedBy, eventID);
            }
        }

        private void RecordLeadAeChangdTime(CrmLead newLead)
        {
            UpdateHistory(newLead.LeadID, newLead.LastUpdatedBy, (int)LeadENumber.NHistoryEvent.AEAssigned);
        }

        public void EditLead(LeadModifyViewMode createViewModel, string username)
        {
            var lead = createViewModel.Lead;
            var emp = new CrmEmployee(username);

            lead.LastUpdatedBy = emp.UserEmployeeID;

            lead.LastUpdatedAt = DateTime.Now;
            var cdt = lead.CriticalDeadline;
            lead.CriticalDeadline = cdt;

            try
            {
                _db.Entry(lead).State = EntityState.Modified;

                createViewModel.Address.AddressID = lead.AddressID;
                _db.Entry(createViewModel.Address).State = EntityState.Modified;

                _db.SaveChanges();

                RecordLeadStatusChangdTime(lead);
                RecordLeadAeChangdTime(lead);
            }
            catch (Exception ex)
            {
                LogMethods.Log.Error("EditLead:Crash:" + ex.Message);
            }
        }

        public void DeleteLead(int leadID)
        {
            var crmLeadHistory = _db.CrmLeadHistories.Where(x => x.LeadID == leadID).ToList();
            if (crmLeadHistory.Any())
            {
                _db.CrmLeadHistories.RemoveRange(crmLeadHistory);
                _db.SaveChanges();
            }

            var lead = _db.CrmLeads.Find(leadID);
            if (lead != null)
            {
                int addressID = lead.AddressID;
                _db.CrmLeads.Remove(lead);
                _db.SaveChanges();

                var crmLeadAddress = _db.CrmLeadAddresses.Where(x => x.AddressID == addressID).ToList();
                if (crmLeadAddress.Any())
                {
                    _db.CrmLeadAddresses.RemoveRange(crmLeadAddress);
                    _db.SaveChanges();
                }
            }
        }


    }
}
