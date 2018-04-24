using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using CustomerDomain.BLL;
using CustomerDomain.Model;
using EmployeeDomain.BLL;
using MyCommon;
using ProjectDomain;

namespace SalesCenterDomain.BLL
{
    public class MyWorkorderShipping
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private CUSTOMER_CONTACT _contact;
        private CUSTOMER _customer;

        public MyWorkorderShipping(int woID)
        {
            Value = _db.WO_Shipping.FirstOrDefault(x => x.WoID == woID);
            if (Value == null)
            {
                Create(woID);
                Value = _db.WO_Shipping.Find(ShippingID);
            }
            ShippingID = Value.ShippingID;
        }

        public string Result { get; private set; }

        public WO_Shipping Value { get; private set; }

        public int ShippingID { get; set; }


        public void ImportAddressFromCustomer(string importCustomerType)
        {
            //Subcontract-->Project-->IsInstallTo->Contact and Customer
            var workorder = new MyWorkorder(Value.WoID);
            var msc = new MySalesJobMasterListCustomer(workorder.Value.jobID);

            if (importCustomerType == "InstallTo")
            {
                msc.SetInstallTo();
            }

            if (importCustomerType == "QuoteTo")
            {
                msc.SetQuoteTo();
            }

            if (importCustomerType == "BillTo")
            {
                msc.SetBillTo();
            }

            var mc = new MyCustomer(msc.CustomerID);
            _customer = mc.Value;
            var mcc = new MyCustomerContact(msc.ContactID);
            _contact = mcc.Value;


            if (_customer != null)
            {
                string addr = _customer.ADDR_1;
                if (!Convert.IsDBNull(_customer.ADDR_2))
                {
                    addr = addr + " " + _customer.ADDR_2;
                }

                Value.Address = addr;

                Value.City = _customer.CITY;
                Value.Postcode = _customer.ZIPCODE;
                Value.Province = _customer.STATE;
                Value.ShipToName = _customer.NAME;

                string attenName = MyConvert.ConvertToString(_contact.CONTACT_HONORIFIC) + " ";
                attenName += MyConvert.ConvertToString(_contact.CONTACT_FIRST_NAME) + " ";
                attenName += MyConvert.ConvertToString(_contact.CONTACT_LAST_NAME);

                Value.AttnName = attenName.Trim();
                Value.AttnPhone = MyConvert.ConvertToString(_contact.CONTACT_PHONE);


                //For Delivery Note

                Value.DeliveryDate = DateTime.Today;

                Value.WorkorderNumber = workorder.Value.WorkorderNumber;


                var emp = new FsEmployee(Convert.ToInt32(workorder.Value.Sales));

                Value.AeName = emp.NickName;
                Value.AePhone = emp.GetCompanyPhoneExtension();


                Value.InvoiceNumber = workorder.GetInvoices();

                Value.NoteTypeID = 0;


                _db.Entry(Value).State = EntityState.Modified;
                _db.SaveChanges();

                Result = "ok";
            }
            else
            {
                Result = "Could not find the specified Customer.";
            }
        }


        private void Create(int woID)
        {
            var shipping = new WO_Shipping();
            shipping.WoID = woID;
            _db.WO_Shipping.Add(shipping);

            try
            {
                //Check Validation Errors
          //      IEnumerable<> error = _db.GetValidationErrors();
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