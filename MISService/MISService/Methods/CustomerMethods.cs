using CustomerDomain.BDL;
using CustomerDomain.BLL;
using CustomerDomain.Model;
using MISService.Method;
using MISService.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using enterprise = MISService.SfdcReference;

/* that is company information */
namespace MISService.Methods
{
    public class CustomerMethods
    {
        private readonly CustomerDbEntities _db = new CustomerDbEntities();
        private EndpointAddress apiAddr;
        private enterprise.SessionHeader header;

        public CustomerMethods()
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
        }

        public void GetAllCompanies(string sfProjectID, int misJobID, int employeeNumber)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Name, Billing_Company_City__c, Billing_Contact_Name__r.Account.Id, Billing_Company_Name__c, Billing_Company_Postal_Code__c, Billing_Company_Province__c, Billing_Company_Street__c, Billing_Contact_Name__c, Billing_Contact_Phone__c, Billing_Company_Country__c,"
                        + " Quoting_Company_City__c, Quoting_Company_Name__c, Quoting_Contact_Name__r.Account.Id, Quoting_Company_Postal_Code__c, Quoting_Company_Province__c, Quoting_Company_Street__c, Quoting_Contact_Name__c, Quoting_Contact_Phone__c, Quoting_Company_Country__c, "
                        + " Shipping_Company_City__c, Shipping_Company_Name__c, Shipping_Contact_Name__r.Account.Id, Shipping_Company_Postal_Code__c, Shipping_Company_Province__c, Shipping_Company_Street__c, Shipping_Contact_Name__c, Shipping_Contact_Phone__c, Shipping_Company_Country__c"         
                        + " FROM Bill_Quote_Ship__c where Project_Name__c = '" + sfProjectID + "'";

                    enterprise.QueryResult result;
                    queryClient.query(
                        header, //sessionheader
                        null, //queryoptions
                        null, //mruheader
                        null, //packageversion
                        query, out result);

                    //cast query results
                    IEnumerable<enterprise.Bill_Quote_Ship__c> billQuoteShipList = result.records.Cast<enterprise.Bill_Quote_Ship__c>();

                    //show results
                    foreach (var bqs in billQuoteShipList)
                    {
                        int customerID = CommonMethods.GetMISID(TableName.Customer, bqs.Billing_Contact_Name__r.Account.Id);
                        if (customerID == 0)
                        {
                            /* Add new billing address */
                            CUSTOMER customer = new CUSTOMER();
                            customer.NAME = bqs.Billing_Company_Name__c;
                            customer.LEGALNAME_SAMEAS_NAME = true;
                            customer.LEGAL_NAME = bqs.Billing_Company_Name__c;
                            customer.ADDR_1 = bqs.Billing_Company_Street__c;
                            customer.ADDR_2 = "";
                            customer.CITY = bqs.Billing_Company_City__c;
                            customer.STATE = bqs.Billing_Company_Province__c;
                            customer.ZIPCODE = bqs.Billing_Company_Postal_Code__c;
                            customer.COUNTRY = bqs.Billing_Company_Country__c;
                            customer.SalesID = employeeNumber;
                            customer.Sa1ID = employeeNumber;
                            Sales_JobMasterList_Customer sales_JobMasterList_Customer = _db.Sales_JobMasterList_Customer.FirstOrDefault(x => x.jobID == misJobID);
                            if (sales_JobMasterList_Customer != null)
                            {
                                int rowID = CreateCustomer(customer, sales_JobMasterList_Customer.jcID);
                                CommonMethods.InsertToMISSalesForceMapping(TableName.Customer, bqs.Billing_Contact_Name__r.Account.Id, rowID.ToString());
                            }
                            else
                            {
                                LogMethods.Log.Error("GetAllCompanies:Error:" + "No row in sales_JobMasterList_Customer for jobID = " + misJobID);
                            }
                        }
                        else
                        {
                            CUSTOMER customer = _db.CUSTOMERs.FirstOrDefault(x => x.ROWID == customerID);
                            customer.NAME = bqs.Billing_Company_Name__c;
                            customer.LEGALNAME_SAMEAS_NAME = true;
                            customer.LEGAL_NAME = bqs.Billing_Company_Name__c;
                            customer.ADDR_1 = bqs.Billing_Company_Street__c;
                            customer.ADDR_2 = "";
                            customer.CITY = bqs.Billing_Company_City__c;
                            customer.STATE = bqs.Billing_Company_Province__c;
                            customer.ZIPCODE = bqs.Billing_Company_Postal_Code__c;
                            customer.COUNTRY = bqs.Billing_Company_Country__c;
                            customer.SalesID = employeeNumber;
                            customer.Sa1ID = employeeNumber;
                            _db.Entry(customer).State = EntityState.Modified;
                            _db.SaveChanges();
                        }

                    }
                    LogMethods.Log.Debug("GetAllCompanies:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllCompanies:Error:" + e.Message);
            }
        }


        /// <summary>
        /// Create a new Customer which is company information
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="jcID">it is jcID in [Sales_JobMasterList_Customer]</param>
        public int CreateCustomer(CUSTOMER customer, int jcID)
        {
            int rowID = 0;
            try
            {
                var mc = new MyCustomer();
                mc.Insert(customer);
                if (jcID > 0) {
                    mc.UpdateSalesJobMasterListCustomerID(jcID);
                }
                rowID = mc.RowID;
                LogMethods.Log.Debug("CreateCustomer:Debug:" + "DONE");
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("CreateCustomer:Crash:" + e.Message);
            }
            return rowID;
        }

        /// <summary>
        /// Edit customer
        /// </summary>
        /// <param name="customer"></param>
        public void EditCustomer(CUSTOMER customer)
        {
            try
            {
                _db.Entry(customer).State = EntityState.Modified;
                _db.SaveChanges();
                LogMethods.Log.Debug("EditCustomer:Debug:" + "DONE");
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("EditCustomer:Crash:" + e.Message);
            }
        }

        /// <summary>
        /// Insert a default row to CUSTOMER_CONTACT table 
        /// </summary>
        /// <param name="customerID">It is RowID in CUSTOMER table</param>
        public void CreateCustomerContact(int customerID)
        {
            try
            {
                var cc = new FsCustomerContact(customerID);
                cc.InsertContact();
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("CreateCustomerContact:Crash:" + e.Message);
            }
        }

        /// <summary>
        /// Edit Customer's Contact
        /// </summary>
        /// <param name="cc"></param>
        public void EditCustomerContact(CUSTOMER_CONTACT cc)
        {
            try
            {
                _db.Entry(cc).State = EntityState.Modified;
                _db.SaveChanges();
                LogMethods.Log.Debug("EditCustomerContact:Debug:" + "DONE");
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("EditCustomerContact:Crash:" + e.Message);
            }

        }

    }
}
