using System;
using System.Data;
using System.Linq;
using CustomerDomain.BLL;
using MyCommon;
using PermitDomain.Model;
using System.Data.Entity;

namespace PermitDomain.BLL
{
	public class MyApplicationForHoistingPermit: BasePermit 
	{
		public PermitForHoisting Value { get; private set; }
		private readonly int _appID;
		private readonly PermitDbEntities _db = new PermitDbEntities();
        private const int NLeadTime = 5;

		public MyApplicationForHoistingPermit(int baseAppID, int appID)
			: base(baseAppID)
		{
			_appID = appID;
			Value = _db.PermitForHoistings.Find(appID);
		}
		
		public override void DeleteChildRecord()
		{
            
			_db.Entry(Value).State = EntityState.Deleted;
			_db.SaveChanges();
		}



        public MyApplicationForHoistingPermit(int baseAppID)
            : base(baseAppID)
		{
            Value = _db.PermitForHoistings.First(x => x.BaseAppID == BaseValue.BaseAppID);
		    _appID = Value.AppID;
		}

        
		public override string IsValidated()
		{
			var b = true;
			string validationResult = "";

		    var isCityOfToronto = false;


            //Leadtime
            if (BaseValue.Deadline <= DateTime.Today)
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

            if (MyConvert.ConvertToString(Value.OccupationTimeStart).Length < 3)
            {
                validationResult += "Occupation Start Time Required" + System.Environment.NewLine;
                b = false;
            }

            if (MyConvert.ConvertToString(Value.OccupationTimeEnd).Length < 3)
            {
                validationResult += "Occupation End Time Required" + System.Environment.NewLine;
                b = false;
            }

            //
            if (Value .TypeOfTruck ==0)
		    {
                validationResult += "Please Fill out Type of Truck  " + System.Environment.NewLine;
                b = false;
		    }

            //Installation Address
            var msc = new MySalesJobMasterListCustomer(BaseValue.JobID );
		    msc.SetInstallTo(); 
            

            if (msc.CustomerID == 0)
            {
                validationResult += "No Installation Company Selected  " + System.Environment.NewLine;
                b = false;
            }
            else
            {
                var mc = new MyCustomer(msc.CustomerID);
                var customer = mc.Value;
                
            if (MyConvert.ConvertToString(customer.ADDR_1).Length < 5)
                {
                    validationResult += "No Installation Address  " + System.Environment.NewLine;
                    b = false;
                }
            if (MyConvert.ConvertToString(customer.CITY).Length < 3)
            {
                validationResult += "Landlord City Required " + System.Environment.NewLine;
                b = false;
            }
                else
            {
                isCityOfToronto = GetIsCityOfToronto(customer.CITY);
            }

            if (MyConvert.ConvertToString(customer.ZIPCODE).Length < 4)
            {
                validationResult += "Landlord Postcode Required " + System.Environment.NewLine;
                b = false;
            }
            }

            if (MyConvert.ConvertToString(Value .ForemanName ).Length < 2)
            {
                validationResult += "Foreman Name Required" + System.Environment.NewLine;
                b = false;
            }

            if (MyConvert.ConvertToString(Value.ForemanPhone).Length < 9)
            {
                validationResult += "Foreman Name Required" + System.Environment.NewLine;
                b = false;
            }




            //301	Insurance	2	30
            //302	Additional Insurance for City of Toronto	2	30
            
            //Document Attached
		        int docTypeID = 301;
                var docAttached = _db.PermitDocuments.Where(x => x.BaseAppID == BaseValue.BaseAppID & x.DocType==docTypeID  ).ToList();        
                if (!docAttached .Any( ))
                {
                    var docType = _db.PermitDocumentTypes.Find(docTypeID);
                    validationResult += "Document Required: " + docType.TypeName   + System.Environment.NewLine;
                    b = false;
                }
                //302	Additional Insurance for City of Toronto	2	30
            if (isCityOfToronto )
            {
                docTypeID = 302;
                docAttached = _db.PermitDocuments.Where(x => x.BaseAppID == BaseValue.BaseAppID & x.DocType == docTypeID).ToList();
                if (!docAttached.Any())
                {
                    var docType = _db.PermitDocumentTypes.Find(docTypeID);
                    validationResult += "Document Required: " + docType.TypeName + System.Environment.NewLine;
                    b = false;
                }
                
            }


		    return b ? "ok" : validationResult;

		}

        private bool GetIsCityOfToronto(string cityName)
        {
            var citiesOfToronto = new string[] { "NORTH YORK", "TORONTO", "SCARBOROUGH", "EAST YORK", "ETOBICOKE","YORK" };
            return citiesOfToronto.Contains(cityName.ToUpper());
        }
	
	}
}