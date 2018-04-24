using CustomerDomain.Model;
using MyCommon.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISService.Models
{
    public class LeadModifyViewMode
    {
        public CrmLead Lead { get; set; }
        public CrmLeadAddress Address { get; set; }
        public List<ItemOption> Countries { get; set; }

        public List<FW_Employees> Owners { get; set; }
        public List<FW_Employees> AEs { get; set; }
        public List<CrmStage> Stages { get; set; }
        public List<FW_QUOTE_SOURCE> Sources { get; set; }
        public List<CrmLeadContentsProgress> Progresses { get; set; }
        public List<FW_QUOTE_IC> Categories { get; set; }


        //Owner is setat create and can't be change
        public string OriginalOwnerName { get; set; }

        public int SelectedCustomerID { get; set; }

        private readonly CustomerDbEntities _db = new CustomerDbEntities();

        //Create, Get
        public LeadModifyViewMode()
        {
            Lead = new CrmLead();
            Address = new CrmLeadAddress();
            Address.Country = "Canada";
        }

        //Create, Post
        public LeadModifyViewMode(CrmLeadAddress address)
        {
            Lead = new CrmLead();
            Address = address;
        }

        //Edit
        public LeadModifyViewMode(int leadID)
        {
            //find existings
            Lead = _db.CrmLeads.Find(leadID);
            Address = _db.CrmLeadAddresses.Find(Lead.AddressID);
            OriginalOwnerName = _db.FW_Employees.Find(Lead.OriginalOwner).NickName;
        }

    }
}
