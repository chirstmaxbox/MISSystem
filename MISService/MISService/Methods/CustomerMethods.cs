using CustomerDomain.BDL;
using CustomerDomain.BLL;
using CustomerDomain.Model;
using MISService.Method;
using MISService.Models;
using SalesCenterDomain.BDL;
using SalesCenterDomain.BDL.Project;
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
        private string salesForceProjectID;

        public CustomerMethods(string salesForceProjectID)
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
            this.salesForceProjectID = salesForceProjectID;
        }

        public void GetAllAccounts(string sfProjectID, int misJobID, int employeeNumber)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Name, Billing_Company_City__c, Billing_Contact_Name__r.Account.Id, Billing_Company_Name__r.Name, Billing_Company_Name__r.Id, Billing_Company_Postal_Code__c, Billing_Company_Province__c, Billing_Company_Street__c, Billing_Contact_Name__r.FirstName, Billing_Contact_Name__r.LastName, Billing_Contact_Name__r.Id, Billing_Contact_Phone__c, Billing_Company_Country__c,"
                        + " Quoting_Company_City__c, Quoting_Company_Name__r.Name, Quoting_Company_Name__r.Id,  Quoting_Contact_Name__r.Account.Id, Quoting_Company_Postal_Code__c, Quoting_Company_Province__c, Quoting_Company_Street__c, Quoting_Contact_Name__r.FirstName, Quoting_Contact_Name__r.LastName, Quoting_Contact_Name__r.Id, Quoting_Contact_Phone__c, Quoting_Company_Country__c, "
                        + " Installing_Company_City__c, Installing_Company_Name__r.Name, Installing_Company_Name__r.Id, Installing_Contact_Name__r.Account.Id, Installing_Company_Postal_Code__c, Installing_Company_Province__c, Installing_Company_Street__c, Installing_Contact_Name__r.FirstName, Installing_Contact_Name__r.LastName, Installing_Contact_Name__r.Id, Installing_Contact_Phone__c, Installing_Company_Country__c"
                        + " FROM Bill_Quote_Install__c where Project_Name__c = '" + sfProjectID + "'";

                    enterprise.QueryResult result;
                    queryClient.query(
                        header, //sessionheader
                        null, //queryoptions
                        null, //mruheader
                        null, //packageversion
                        query, out result);

                    /* if no any record, return */
                    if (result.size == 0) return;

                    //cast query results
                    IEnumerable<enterprise.Bill_Quote_Install__c> billQuoteShipList = result.records.Cast<enterprise.Bill_Quote_Install__c>();

                    //show results
                    foreach (var bqs in billQuoteShipList)
                    {
                        bool hasOne = false;
                        if (bqs.Billing_Company_Name__r != null && bqs.Billing_Contact_Name__r != null)
                        {
                            HandleAccount(bqs.Billing_Company_Name__r.Name, bqs.Billing_Company_Street__c, bqs.Billing_Company_Province__c, bqs.Billing_Company_Postal_Code__c,
                                bqs.Billing_Company_City__c, bqs.Billing_Company_Country__c, bqs.Billing_Contact_Name__r.FirstName,
                                bqs.Billing_Contact_Name__r.LastName, bqs.Billing_Contact_Phone__c, bqs.Billing_Contact_Name__r.Id, misJobID, employeeNumber, bqs.Billing_Company_Name__r.Id, 1);
                            hasOne = true;
                        }

                        if (bqs.Quoting_Company_Name__r != null && bqs.Quoting_Contact_Name__r != null)
                        {
                            HandleAccount(bqs.Quoting_Company_Name__r.Name, bqs.Quoting_Company_Street__c, bqs.Quoting_Company_Province__c, bqs.Quoting_Company_Postal_Code__c,
                                bqs.Quoting_Company_City__c, bqs.Quoting_Company_Country__c, bqs.Quoting_Contact_Name__r.FirstName,
                                bqs.Quoting_Contact_Name__r.LastName, bqs.Quoting_Contact_Phone__c, bqs.Quoting_Contact_Name__r.Id, misJobID, employeeNumber, bqs.Quoting_Company_Name__r.Id, 2);
                            hasOne = true;
                        }

                        if (bqs.Installing_Company_Name__r != null && bqs.Installing_Contact_Name__r != null)
                        {
                            HandleAccount(bqs.Installing_Company_Name__r.Name, bqs.Installing_Company_Street__c, bqs.Installing_Company_Province__c, bqs.Installing_Company_Postal_Code__c,
                                bqs.Installing_Company_City__c, bqs.Installing_Company_Country__c, bqs.Installing_Contact_Name__r.FirstName,
                                bqs.Installing_Contact_Name__r.LastName, bqs.Installing_Contact_Phone__c, bqs.Installing_Contact_Name__r.Id, misJobID, employeeNumber, bqs.Installing_Company_Name__r.Id, 3);
                            hasOne = true;
                        }

                        /* delete default row if there exists at least one bill/quote/install */
                        if (hasOne)
                        {
                            var records = _db.Sales_JobMasterList_Customer.Where(x => x.jobID == misJobID && x.cID == 0 && x.contactName == 0 && x.isBillTo == false && x.isQuoteTo == false && x.isInstallOrShipTo == false).ToList();
                            if (records.Any())
                            {
                                foreach (var r in records)
                                {
                                    //delete it
                                    _db.Sales_JobMasterList_Customer.Remove(r);
                                }
                                _db.SaveChanges();
                            }
                        }
                    }
                    LogMethods.Log.Debug("GetAllAccounts:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllAccounts:Error:" + e.Message);
            }
        }

        private void UpdateSales_JobMasterList_Customer(int jcID, int jobID, int contactID, int type)
        {
            try
            {
                var sales_JobMasterList_Customers = _db.Sales_JobMasterList_Customer.Where(x => x.jobID == jobID).ToList();
                if (sales_JobMasterList_Customers.Any())
                {
                    foreach (var element in sales_JobMasterList_Customers)
                    {
                        switch (type)
                        {
                            case 1:
                                element.isBillTo = false;
                                break;
                            case 2:
                                element.isQuoteTo = false;
                                break;
                            case 3:
                                element.isInstallOrShipTo = false;
                                break;
                            default:
                                break;
                        }
                        _db.Entry(element).State = EntityState.Modified;
                    }
                    _db.SaveChanges();
                }

                Sales_JobMasterList_Customer sales_JobMasterList_Customer = _db.Sales_JobMasterList_Customer.FirstOrDefault(x => x.jcID == jcID);
                if (sales_JobMasterList_Customer != null)
                {
                    switch (type)
                    {
                        case 1:
                            sales_JobMasterList_Customer.isBillTo = true;
                            break;
                        case 2:
                            sales_JobMasterList_Customer.isQuoteTo = true;
                            break;
                        case 3:
                            sales_JobMasterList_Customer.isInstallOrShipTo = true;
                            break;
                        default:
                            break;
                    }
                    sales_JobMasterList_Customer.contactName = contactID;
                    _db.Entry(sales_JobMasterList_Customer).State = EntityState.Modified;
                    _db.SaveChanges();
                }
 
                LogMethods.Log.Debug("UpdateSales_JobMasterList_Customer:Debug:" + "Done");
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("UpdateSales_JobMasterList_Customer:Error:" + e.Message);
            }
        }

        private void HandleAccountContact(int jobID, int jcID, string contactID, int customerRowID, string firstName, string lastName, string phone, string accountID, int type)
        {
            try
            {
                /* contact info */
                int vContactID = CommonMethods.GetMISID(TableName.Customer_Contact, contactID, accountID, salesForceProjectID);
                if (vContactID == 0)
                {
                    /* add new contact */
                    FsCustomerContact cc = new FsCustomerContact(customerRowID);
                    cc.InsertContact();
                    int contact_id = SqlCommon.GetNewlyInsertedRecordID(TableName.Customer_Contact);
                    CommonMethods.InsertToMISSalesForceMapping(TableName.Customer_Contact, contactID, contact_id.ToString(), accountID, salesForceProjectID);
                    vContactID = contact_id;
                }

                /* update */
                CUSTOMER_CONTACT customer_contact = _db.CUSTOMER_CONTACT.FirstOrDefault(x => x.CONTACT_ID == vContactID);
                if (customer_contact != null)
                {
                    customer_contact.CONTACT_FIRST_NAME = firstName;
                    customer_contact.CONTACT_LAST_NAME = lastName;
                    customer_contact.CONTACT_PHONE = phone;
                    _db.Entry(customer_contact).State = EntityState.Modified;
                    _db.SaveChanges();
                }

                UpdateSales_JobMasterList_Customer(jcID, jobID, vContactID, type);

                LogMethods.Log.Debug("HandleAccountContact:Debug:" + "Done");
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("HandleAccountContact:Error:" + e.Message);
            }
        }

        /**
         * Type = 1 => Billing
         * Type = 2 => Quoting
         * Type = 3 => Shipping
        */
        private void HandleAccount(string companyName, string companyStreet, string companyProvince, string companyPostalCode,
            string companyCity, string companyCountry, string firstName, string lastName, string phone, string contactID, int misJobID, int employeeNumber, string accountID, int type)
        {
            try
            {
                int customerID = CommonMethods.GetMISID(TableName.Customer, accountID, salesForceProjectID);
                if (customerID == 0)
                {
                    /* Add new billing address */
                    CUSTOMER customer = new CUSTOMER();
                    customer.NAME = companyName;
                    customer.LEGALNAME_SAMEAS_NAME = true;
                    customer.LEGAL_NAME = companyName;
                    customer.ADDR_1 = companyStreet;
                    customer.ADDR_2 = "";
                    customer.CITY = companyCity;
                    customer.STATE = companyProvince;
                    customer.ZIPCODE = companyPostalCode;
                    customer.COUNTRY = companyCountry;
                    customer.SalesID = employeeNumber;
                    customer.Sa1ID = employeeNumber;
                    customer.TERRITORY = "6"; //other
                    customer.ACTIVE_FLAG = "Y";
                    customer.CURRENCY_ID = "CAD";

                    ProjectCompany cp = new ProjectCompany(misJobID);
                    cp.Insert(misJobID, 0, false, false, false);
                    int jcID = SqlCommon.GetNewlyInsertedRecordID(TableName.Sales_JobMasterList_Customer);
                    int rowID = CreateCustomer(customer, jcID);
                    CommonMethods.InsertToMISSalesForceMapping(TableName.Customer, accountID, rowID.ToString(), salesForceProjectID);
                    HandleAccountContact(misJobID, jcID, contactID, rowID, firstName, lastName, phone, accountID, type);
                }
                else
                {
                    CUSTOMER customer = _db.CUSTOMERs.FirstOrDefault(x => x.ROWID == customerID);
                    customer.NAME = companyName;
                    customer.LEGALNAME_SAMEAS_NAME = true;
                    customer.LEGAL_NAME = companyName;
                    customer.ADDR_1 = companyStreet;
                    customer.ADDR_2 = "";
                    customer.CITY = companyCity;
                    customer.STATE = companyProvince;
                    customer.ZIPCODE = companyPostalCode;
                    customer.COUNTRY = companyCountry;
                    customer.SalesID = employeeNumber;
                    customer.Sa1ID = employeeNumber;
                    customer.TERRITORY = "6"; //other
                    customer.ACTIVE_FLAG = "Y";
                    customer.CURRENCY_ID = "CAD";
                    _db.Entry(customer).State = EntityState.Modified;
                    _db.SaveChanges();

                    Sales_JobMasterList_Customer sales_JobMasterList_Customer = _db.Sales_JobMasterList_Customer.FirstOrDefault(x => x.jobID == misJobID && x.cID == customer.ROWID);
                    if (sales_JobMasterList_Customer != null)
                    {
                        HandleAccountContact(misJobID, sales_JobMasterList_Customer.jcID, contactID, customerID, firstName, lastName, phone, accountID, type);
                    }
                }

                LogMethods.Log.Debug("HandleAccount:Debug:" + "Done");
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("HandleAccount:Error:" + e.Message);
            }
        }

        /// <summary>
        /// Create a new Customer which is company information
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="jcID">it is jcID in [Sales_JobMasterList_Customer]</param>
        private int CreateCustomer(CUSTOMER customer, int jcID)
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
                LogMethods.Log.Debug("CreateCustomer:Debug:" + "Done");
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("CreateCustomer:Error:" + e.Message);
            }
            return rowID;
        }

    }
}
