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
        /// <summary>
        /// Create a new Lead
        /// </summary>
        /// <param name="createViewModel"></param>
        /// <param name="username">user name, such as type whoami under command line \\F01\anh.tran</param>
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
                LogMethods.Log.Debug("CreateLead:Debug:" + "DONE");
            }
            catch (DbEntityValidationException dbEx)
            {
                LogMethods.Log.Error("CreateLead:Crash:" + dbEx.Message);
            }
        }

        /// <summary>
        /// Track Lead History
        /// </summary>
        /// <param name="leadID"></param>
        /// <param name="employeeID"></param>
        /// <param name="eventID"></param>
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
            LogMethods.Log.Debug("UpdateHistory:Debug:" + "DONE");
        }

        //45	UnQualified By Marketing	NULL
        //51	Quote Request	NULL
        //52	AE Assigned	NULL
        //81	UnQualified By Sales	NULL
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newLead"></param>
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
            LogMethods.Log.Debug("RecordLeadStatusChangdTime:Debug:" + "DONE");
        }

        private void RecordLeadAeChangdTime(CrmLead newLead)
        {
            UpdateHistory(newLead.LeadID, newLead.LastUpdatedBy, (int)LeadENumber.NHistoryEvent.AEAssigned);
            LogMethods.Log.Debug("RecordLeadAeChangdTime:Debug:" + "DONE");
        }

        /// <summary>
        /// Edit an existing lead
        /// </summary>
        /// <param name="createViewModel"></param>
        /// <param name="username"></param>
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
                LogMethods.Log.Debug("EditLead:Debug:" + "DONE");
            }
            catch (Exception ex)
            {
                LogMethods.Log.Error("EditLead:Crash:" + ex.Message);
            }
        }

        /// <summary>
        /// Delete an existing lead
        /// </summary>
        /// <param name="leadID"></param>
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
            LogMethods.Log.Debug("DeleteLead:Debug:" + "DONE");
        }

        private void AddMainContact(CrmLeadContact contact)
        {
            var contacts = _db.CrmLeadContacts.Where(x => x.ContactID > 100 && x.LeadID == contact.LeadID);
            if (contacts.Count() == 1)
            {
                var lead = _db.CrmLeads.Find(contact.LeadID);
                UpdateCrmLeadContactID(lead, contact.ContactID);
            }
            LogMethods.Log.Debug("AddMainContact:Debug:" + "DONE");
        }

        private void UpdateCrmLeadContactID(CrmLead lead, int contactID)
        {
            try
            {
                lead.ContactID = contactID;
                _db.Entry(lead).State = EntityState.Modified;
                _db.SaveChanges();
                LogMethods.Log.Debug("UpdateCrmLeadContactID:Debug:" + "DONE");
            }
            catch (Exception ex)
            {
                LogMethods.Log.Error("UpdateCrmLeadContactID:Crash:" + ex.Message);
            }
        }

        /// <summary>
        /// Add new contact to corresponding lead
        /// </summary>
        /// <param name="contact"></param>
        public void CreateLeadContact(CrmLeadContact contact)
        {
            _db.CrmLeadContacts.Add(contact);
            try
            {
                _db.SaveChanges();
                AddMainContact(contact);
                LogMethods.Log.Debug("CreateLeadContact:Debug:" + "DONE");
            }
            catch (DbEntityValidationException dbEx)
            {
                LogMethods.Log.Error("CreateLeadContact:Crash:" + dbEx.Message);
            }
        }

        /// <summary>
        /// Edit an existing contact
        /// </summary>
        /// <param name="contact"></param>
        public void EditLeadContact(CrmLeadContact contact)
        {
            try
            {
                _db.Entry(contact).State = EntityState.Modified;
                _db.SaveChanges();
                AddMainContact(contact);
                LogMethods.Log.Debug("EditLeadContact:Debug:" + "DONE");
            }
            catch (DbEntityValidationException dbEx)
            {
                LogMethods.Log.Error("EditLeadContact:Crash:" + dbEx.Message);
            }
        }

        /// <summary>
        /// Delete an exising contact to corresponding lead
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="username">user name, such as type whoami under command line \\F01\anh.tran</param>
        public void DeleteLeadContact(CrmLeadContact contact, string username)
        {
            var leadID = contact.LeadID;
            //Validation
            if (contact.CustomerContactID > 0)
            {
                LogMethods.Log.Error("DeleteLeadContact:Error:" + "The Lead has been convert to Customer, can not be deleted.");
                return;
            }
            var emp = new CrmEmployee(username);
            var lead = _db.CrmLeads.Find(contact.LeadID);

            if (emp.UserEmployeeID != lead.OriginalOwner)
            {
                LogMethods.Log.Error("DeleteLeadContact:Error:" + "Only owner can delete contact.");
                return;
            }

            //to be deleted contact is Main Contact
            if (lead.ContactID == contact.ContactID)
            {
                lead.ContactID = 0;
                _db.Entry(lead).Property(x => x.ContactID).IsModified = true;
                _db.SaveChanges();
            }

            _db.Entry(contact).State = EntityState.Deleted;
            _db.SaveChanges();

            var contacts = _db.CrmLeadContacts.Where(x => x.LeadID == contact.LeadID);
            if (contacts.Count() > 0)
            {
                var firstContact = contacts.First();
                lead.ContactID = firstContact.ContactID;
                _db.Entry(lead).State = EntityState.Modified;
                _db.SaveChanges();
            }
            LogMethods.Log.Debug("DeleteLeadContact:Debug:" + "DONE");
        }
    }
}
