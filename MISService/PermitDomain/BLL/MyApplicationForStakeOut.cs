using System;
using System.Data;
using System.Linq;
using CustomerDomain.BLL;
using MyCommon;
using PermitDomain.Model;



namespace PermitDomain.BLL
{
 
	public class MyApplicationForStakeOut: BasePermit 
	{
		public PermitForStakeout Value { get; private set; }
		private readonly int _appID;
		private readonly PermitDbEntities _db = new PermitDbEntities();
	    private const int NLeadTime = 5;

        public MyApplicationForStakeOut(int baseAppID): base(baseAppID)
		{
            Value = _db.PermitForStakeouts.First(x=>x.BaseAppID ==BaseValue.BaseAppID );
		    _appID = Value.AppID;
		}

		public MyApplicationForStakeOut(int baseAppID, int appID): base(baseAppID)
		{
			_appID = appID;
			Value = _db.PermitForStakeouts.Find(appID);
		}
		
		public override void DeleteChildRecord()
		{
			_db.Entry(Value).State = EntityState.Deleted;
			_db.SaveChanges();
		}


		public override string IsValidated()
		{
            var b = true;
            string validationResult = "";

            //Installation Address
            var msc = new MySalesJobMasterListCustomer(BaseValue.JobID);
            msc.SetInstallTo();

            if (msc.CustomerID == 0)
            {
                validationResult += "No Installation Company Selected  " + System.Environment.NewLine;
                b = false;
            }
            else
            {
                var mc = new MyCustomer(msc.CustomerID);

                if (MyConvert.ConvertToString(mc.Value.ADDR_1).Length < 5)
                {
                    validationResult += "No Installation Address  " + System.Environment.NewLine;
                    b = false;
                }
            }
            

            //Document Attached
            var requiredDocuemntTypes = new int[] { 200};
            for (int i = 0; i < requiredDocuemntTypes.Length; i++)
            {
                var docTypeID = requiredDocuemntTypes[i];
                var docAttached = _db.PermitDocuments.Where(x => x.BaseAppID == BaseValue.BaseAppID & x.DocType == docTypeID).ToList();
                if (!docAttached.Any())
                {
                    var docType = _db.PermitDocumentTypes.Find(docTypeID);
                    validationResult += "Document Required: " + docType.TypeName + System.Environment.NewLine;
                    b = false;
                }
            }

            //Leadtime
            if (BaseValue.Deadline <=DateTime .Today )
            {
                validationResult += "Lead time is 5 business days  " + System.Environment.NewLine;
                b = false;
            }
            else
            {
                var endDate = Convert.ToDateTime(BaseValue.Deadline);
                if (MyDateTime.GetDiffHoursOfWeekday(DateTime.Today, endDate) < 24 * NLeadTime)
                {
                    validationResult += "Lead time is 5 business days  " + System.Environment.NewLine;
                    b = false;
                }
                
            }

            return b ? "ok" : validationResult;

		}

	
	}
}