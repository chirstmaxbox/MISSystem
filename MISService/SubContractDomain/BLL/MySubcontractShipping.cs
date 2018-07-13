using System;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using CustomerDomain.BLL;
using MyCommon;
using SubContractDomain.Model;
using System.Data.Entity;


namespace SubContractDomain.BLL
{

	public class MySubcontractShippingModify
	{
		public string Result { get; private set; }

		public SubcontractShipping Value { get; private set; }
	
		public int ShippingID { get; set; }
		
		private readonly SubContractDbEntities _db = new SubContractDbEntities();

		public MySubcontractShippingModify(int subcontractID, int shippingID)
		{
			ShippingID = shippingID;
			if (shippingID <= 0)
			{
				Create(subcontractID);
			}
			Value = _db.SubcontractShippings.Find(ShippingID);
		}

		public void ImportAddressFromInstaller()
		{

			var response = _db.SubcontractResponses.FirstOrDefault(x => x.SubcontractID == Value.SubContractID );
			if (response ==null)
			{
				Result = "Request Failed, No Installer Found.";
			}
			else
			{
				var installerID = response.InstallerID;
				var installer = _db.SubContractInstallers.Find(installerID);

				Value.Address = installer.Address;
				Value.City = installer.City;
				Value.Postcode = installer.PostCode;
				Value.Province = installer.Province;
				Value.ShipToName = installer.CompanyName;
				Value.AttnName = installer.ContactName;
				Value.AttnPhone = installer.Phone;

				_db.Entry(Value).State = EntityState.Modified;
				_db.SaveChanges();

				Result = "ok";
			}
		
		}


		public void ImportAddressFromCustomer()
		{
			//Subcontract-->Project-->IsInstallTo->Contact and Customer

			var subcontract = new MySubContract(Value.SubContractID);
			var msc = new MySalesJobMasterListCustomer(subcontract.Value.JobID );
			msc.SetInstallTo();
			
			if (msc.CustomerID <1000)
			{
				Result = "Request Failed, No Install to Customer Selected.";
			}
			else
			{
			    var myCustomer = new MyCustomer(msc.CustomerID);
			    var myContact = new MyCustomerContact(msc.ContactID);
                
				string  addr = myCustomer.Value.ADDR_1;
				if (! Convert.IsDBNull( myCustomer.Value .ADDR_2))
				{
					addr=addr+ " "+ myCustomer.Value.ADDR_2;
				}

				Value.Address = addr;

				Value.City = myCustomer.Value.CITY ;
				Value.Postcode = myCustomer.Value.ZIPCODE;
				Value.Province =myCustomer.Value.STATE ;
				Value.ShipToName =myCustomer.Value .NAME ;

				string attenName =MyConvert.ConvertToString(myContact.Value.CONTACT_HONORIFIC) +" ";
				attenName += MyConvert.ConvertToString(myContact.Value.CONTACT_FIRST_NAME ) +" " ;
				attenName += MyConvert.ConvertToString(myContact.Value.CONTACT_LAST_NAME );

				Value.AttnName = attenName.Trim();
				Value.AttnPhone = MyConvert.ConvertToString( myContact.Value .CONTACT_PHONE) ;

				_db.Entry(Value).State = EntityState.Modified;
				_db.SaveChanges();

				Result = "ok";
			}

		}


		private void  Create(int subcontractID )
		{
			var shipping = new SubcontractShipping();
			shipping.SubContractID = subcontractID;
			_db.SubcontractShippings.Add (shipping );

			try
			{
				//Check Validation Errors
				var error = _db.GetValidationErrors();
				_db.SaveChanges();
				ShippingID = shipping.ShippingID;

			}
			catch (DbEntityValidationException dbEx)
			{
				var s = dbEx.Message;
			}
		}


	}
}