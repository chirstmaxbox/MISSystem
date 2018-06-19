using System;
using System.Data;
using System.Linq;
using CustomerDomain.BLL;
using MyCommon;
using PermitDomain.Model;


namespace PermitDomain.BLL
{
 public class MyApplicationForSignVariance: BasePermit 
	{
		public PermitForSignVariance Value { get; private set; }
     
		public int LandlordID
		{
			set { SetLandlordID(value); }
		}

		private readonly int _appID;
		private readonly PermitDbEntities _db = new PermitDbEntities();


		public MyApplicationForSignVariance(int baseAppID): base(baseAppID)
		{
            Value = _db.PermitForSignVariances.First(x=>x.BaseAppID ==BaseValue.BaseAppID );
		    _appID = Value.AppID;
		}

		public MyApplicationForSignVariance(int baseAppID, int appID): base(baseAppID)
		{
			_appID = appID;
			Value = _db.PermitForSignVariances.Find(appID);
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

            //Number of Sign
            if ( MyConvert .ConvertToDouble( Value.NumberOfSigns)  == 0)
            {
                validationResult += "Number of Signs required." + System.Environment.NewLine;
                b = false;
            }

            //Estimated Value
            if (MyConvert.ConvertToDouble(Value.ProjectValueEstimated) == 0)
            {
                validationResult += "Estimated Project Valued  required." + System.Environment.NewLine;
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
            if (MyConvert.ConvertToString(mc.Value.ADDR_1).Length < 5)
                {
                    validationResult += "No Installation Address  " + System.Environment.NewLine;
                    b = false;
                }    
            }


            //Landlord
            if (Value .LandlordID <1000)
            {
                validationResult += "Enter a Landlord Please  " + System.Environment.NewLine;
                b = false;
            }
            else
            {
                var landlord = _db.PermitLandlords.Find(Value.LandlordID);
                //Company Name
                if (MyConvert.ConvertToString(landlord .NAME ).Length < 5)
                {
                    validationResult += "Landlord Name Required" + System.Environment.NewLine;
                    b = false;
                }
                //Address
                if (MyConvert.ConvertToString(landlord.ADDR_1 ).Length < 5)
                {
                    validationResult += "Landlord Address Required " + System.Environment.NewLine;
                    b = false;
                }

                if (MyConvert.ConvertToString(landlord.CITY ).Length < 3)
                {
                    validationResult += "Landlord City Required " + System.Environment.NewLine;
                    b = false;
                }

                if (MyConvert.ConvertToString(landlord.ZIPCODE ).Length < 4)
                {
                    validationResult += "Landlord Postcode Required " + System.Environment.NewLine;
                    b = false;
                }
                
            }

                        //Landlord Contact
            if (Value.LandlordContactID < 1000)
            {
                validationResult += "Enter a Landlord Contact  " + System.Environment.NewLine;
                b = false;
            }
            else
            {
                var contact = _db.PermitLandlordContacts.Find( Value.LandlordContactID );

                if (MyConvert.ConvertToString(contact.CONTACT_FIRST_NAME).Length < 5)
                {
                    validationResult += "Landlord Contact Name Required" + System.Environment.NewLine;
                    b = false;
                }
                if (MyConvert.ConvertToString(contact.CONTACT_PHONE ).Length < 9)
                {
                    validationResult += "Landlord Contact Phone Required" + System.Environment.NewLine;
                    b = false;
                }
            }

            //Document Attached
		    var requiredDocuemntTypes = new int[] {401};
		    for (int i=0; i<requiredDocuemntTypes .Length; i++ )
		    {
		        var docTypeID = requiredDocuemntTypes[i];
                var docAttached = _db.PermitDocuments.Where(x => x.BaseAppID == BaseValue.BaseAppID & x.DocType==docTypeID  ).ToList();        
                if (!docAttached .Any( ))
                {
                    var docType = _db.PermitDocumentTypes.Find(docTypeID);
                    validationResult += "Document Required: " + docType.TypeName   + System.Environment.NewLine;
                    b = false;
                }
		    }
      
		    return b ? "ok" : validationResult;

		}

		private void SetLandlordID(int landlordID)
		{
			Value.LandlordID = landlordID;
			_db.Entry(Value).State = EntityState.Modified;
			_db.SaveChanges();
        }


    }

}