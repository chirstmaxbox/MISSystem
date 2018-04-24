using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using CustomerDomain.Model;
using MyCommon;
using System.Data.Entity;

namespace CustomerDomain.BLL
{
    public class MyCustomer
    {
		public int RowID {get; private set;}

		public CUSTOMER Value {get; private set; }
        private readonly CustomerDbEntities _db = new CustomerDbEntities();

		public MyCustomer ()
		{
		}
		public MyCustomer( int customerID)
		{
			Value = _db.CUSTOMERs.Find(customerID);
		}

	
		public List<CUSTOMER_CONTACT > GetCustomerContacts(int customerID)
		{
			return _db.CUSTOMER_CONTACT.Where(x => x.CUSTOMER_ROWID == customerID).OrderBy(x => x.CONTACT_FIRST_NAME).ToList();

		}


		#region ***************************  Insert ***************************************
		

    	public void Insert(CUSTOMER customer)
		{
			_db.CUSTOMERs.Add(customer);
			_db.SaveChanges();
			RowID = customer.ROWID;
		}

        #endregion

        public int PostBackProjectID { get; set; }

        public void UpdateSalesJobMasterListCustomerID(int jcID)
        {
            var jc = _db.Sales_JobMasterList_Customer.Find(jcID);
            PostBackProjectID = jc.jobID;

            jc.cID = RowID;
        
            _db.Entry(jc).State = EntityState.Modified;
            _db.SaveChanges();
        
        }
        

        public void UpdateCommissionFactor(bool isDeemedAsOld, bool isNewLineOfHouseAccount)
        {
            Value.DeemedAsOld = isDeemedAsOld;
            Value.IsNewLineOfHouseAccount = isNewLineOfHouseAccount; 
            _db.Entry(Value).State = EntityState.Modified;
            try
            {
                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
            }   
        }

        public string GetMyAddress()
        {
            var address = Value.ADDR_1;
      
                if (MyConvert.ConvertToString(Value.ADDR_2 ).Length >2)
                {
                    address += "  " + Value.ADDR_2; 
                }


                if (MyConvert.ConvertToString(Value.CITY).Length > 1)
                {
                    address += "  " + Value.CITY;
                }

                if (MyConvert.ConvertToString(Value.STATE ).Length > 1)
                {
                    address += ", " + Value.STATE ;
                }

                if (MyConvert.ConvertToString(Value.ZIPCODE).Length >4)
                {
                    address += ", " + Value.ZIPCODE ;
                }
            return address;

        }


        public List<Sales_JobMasterList_Customer> GetProjectCustomer(int customerID)
        {
            return _db.Sales_JobMasterList_Customer.Where(x => x.cID == customerID).OrderBy(x => x.jobID).ToList();
        }

	}



}