using System.Linq;
using CustomerDomain.Model;

namespace CustomerDomain.BLL
{
    public class MySalesJobMasterListCustomer
    {
		public Sales_JobMasterList_Customer Value { get;  set; }			
		public int CustomerID  { get; set; }								//?
		public int ContactID { get; set; }						//?

        private readonly CustomerDbEntities _db = new CustomerDbEntities();

    	private readonly int _jobID;
		
		public MySalesJobMasterListCustomer(int jobID)
		{
			_jobID = jobID;
		}

		public void SetInstallTo(){
    		
		
            Value  = _db.Sales_JobMasterList_Customer.FirstOrDefault( x => x.jobID == _jobID & x.isInstallOrShipTo == true);
			if (Value == null) return;
			if (Value.cID <100)
			{
				Value = null;
				return;
			}

		    CustomerID = Value.cID;
		    ContactID = Value.contactName;

		}

		public void  SetQuoteTo()
		{

			Value = _db.Sales_JobMasterList_Customer.FirstOrDefault(x => x.jobID == _jobID & x.isQuoteTo == true);

			if (Value == null) return;
			if (Value.cID < 100)
			{
				Value = null;
				return;
			}

            CustomerID = Value.cID;
            ContactID = Value.contactName;

		}

		public void SetBillTo()
		{

			Value  = _db.Sales_JobMasterList_Customer.FirstOrDefault(x => x.jobID == _jobID & x.isBillTo == true);
			if (Value == null) return;
			if (Value.cID < 100)
			{
				Value = null;
				return;
			}

            CustomerID = Value.cID;
            ContactID = Value.contactName;

		}

    }
}
