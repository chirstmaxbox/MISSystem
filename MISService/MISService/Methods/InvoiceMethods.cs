﻿using MISService.Method;
using MISService.Models;
using ProjectDomain;
using SalesCenterDomain.BDL;
using SalesCenterDomain.BDL.Invoice;
using SalesCenterDomain.BDL.Project;
using SalesCenterDomain.BDL.Service;
using SalesCenterDomain.BO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using enterprise = MISService.SfdcReference;

namespace MISService.Methods
{
    public class InvoiceMethods
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private EndpointAddress apiAddr;
        private enterprise.SessionHeader header;
        private string salesForceProjectID;

        public InvoiceMethods(string salesForceProjectID)
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
            this.salesForceProjectID = salesForceProjectID;
        }

        public void GetAllInvoices(string sfProjectID, int jobID, int estRevID, int userEmployeeID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Name, Issue_Date__c, Shipping_Method__c, Contract_Number__c, Contract_Date__c, List_Item_Name__c, "
                        + " List_Service_Name__c, Terms__c, SubTotal__c, Discount__c, HST__c, Deposit__c, Quote_Name__r.Tax_Option__c "
                        + " FROM Invoice__c where Project_Name__c = '" + sfProjectID + "'";

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
                    IEnumerable<enterprise.Invoice__c> invoiceList = result.records.Cast<enterprise.Invoice__c>();

                    foreach (var ql in invoiceList)
                    {
                        /* check if the work order exists */
                        int invoiceID = CommonMethods.GetMISID(TableName.Sales_JobMasterList_Invoice, ql.Id, salesForceProjectID);
                        if (invoiceID == 0)
                        {
                            // not exist
                            ProjectCompany cp = new ProjectCompany(jobID);
                            if (cp.isThereABilltoCompany)
                            {
                                InvoiceTitleGenerateFromProject inv = new InvoiceTitleGenerateFromProject(jobID);
                                inv.Generate();
                                invoiceID = inv.MyID;
                                CommonMethods.InsertToMISSalesForceMapping(TableName.Sales_JobMasterList_Invoice, ql.Id, invoiceID.ToString(), salesForceProjectID);
                            }
                            else
                            {
                                LogMethods.Log.Debug("GetAllInvoices:Debug:" + "It doesn't have a bill to company");
                            }
                        }

                        if (invoiceID != 0)
                        {
                            UpdateInvoice(invoiceID, ql.Name, ql.Issue_Date__c, userEmployeeID, ql.Terms__c, ql.Contract_Number__c,
                                ql.Shipping_Method__c, ql.Contract_Date__c, ql.Quote_Name__r.Tax_Option__c, ql.HST__c, ql.Deposit__c, ql.Discount__c);

                            /* handle item */
                            HandleInvoiceItem(invoiceID, ql.List_Item_Name__c, ql.Id);

                            /* handle service */
                            HandleInvoiceService(invoiceID, ql.List_Service_Name__c, ql.Id);
                        }

                    }
                    LogMethods.Log.Debug("GetAllInvoices:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllInvoices:Error:" + e.Message);
            }
        }

        private void HandleInvoiceService(int invoiceID, string listServiceID, string sfInvoiceID)
        {
            try
            {
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    if (string.IsNullOrEmpty(listServiceID)) return;
                    string[] services = listServiceID.Split(',');
                    /* if no any items, return */
                    if (services.Length == 0) return;

                    //create SQL query statement
                    string query = "SELECT Id, Service_Name__r.Name, Detail__c, Service_Cost__c,Note__c, Service_Name__r.MIS_Service_Number__c FROM Service_Cost__c where Id in (";
                    foreach (string e in services)
                    {
                        if (!string.IsNullOrEmpty(e.Trim()))
                        {
                            query += "'" + e + "',";
                        }
                    }
                    query = query.Remove(query.Length - 1);
                    query += ")";

                    enterprise.QueryResult result;
                    queryClient.query(
                        header,
                        null,
                        null,
                        null,
                        query, out result);

                    /* if no any record, return */
                    if (result.size == 0) return;

                    IEnumerable<enterprise.Service_Cost__c> serviceList = result.records.Cast<enterprise.Service_Cost__c>();
                    var svc = new FsService(invoiceID, "Invoice");
                    foreach (var sl in serviceList)
                    {
                        long estServiceID = CommonMethods.GetMISID(TableName.Fw_Quote_Service, sl.Id, sfInvoiceID, salesForceProjectID);
                        if (estServiceID == 0)
                        {
                            int printOrder = svc.GetQsMaxPrintOrder() + 1;
                            if (sl.Service_Cost__c1 != null && sl.Service_Cost__c1 > 0)
                            {
                                svc.InsertRecord(Convert.ToInt32(sl.Service_Name__r.MIS_Service_Number__c),
                                     sl.Service_Cost__c1 == null ? "0" : sl.Service_Cost__c1.ToString(),
                                     1,
                                     sl.Detail__c == null ? "" : sl.Detail__c,
                                     sl.Service_Name__r.Name,
                                     sl.Service_Cost__c1 == null ? "0" : sl.Service_Cost__c1.ToString(),
                                     printOrder
                                );

                                int qs_id = SqlCommon.GetNewlyInsertedRecordID(TableName.Fw_Quote_Service);
                                CommonMethods.InsertToMISSalesForceMapping(TableName.Fw_Quote_Service, sl.Id, qs_id.ToString(), sfInvoiceID, salesForceProjectID);
                            }
                            else
                            {
                                LogMethods.Log.Debug("HandleInvoiceService:Debug:" + "Service cost must be a number");
                            }
                        }
                        else
                        {
                            UpdateInvoiceService(estServiceID, sl.Service_Cost__c1, sl.Detail__c, sl.Service_Name__r.Name, Convert.ToInt16(sl.Service_Name__r.MIS_Service_Number__c), sl.Note__c);
                        }

                    }

                    DeleteAllDeletedInvoiceServices(services, sfInvoiceID);
                    LogMethods.Log.Debug("HandleInvoiceService:Debug:" + "Done");
                }

            }
            catch (Exception e)
            {
                LogMethods.Log.Error("HandleInvoiceService:Error:" + e.Message);
            }
        }

        private void DeleteInvoiceService(int qsId)
        {
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                Connection.Open();
                string SqlDelString = "DELETE FROM FW_QUOTE_SERVICE WHERE ([qsID] = @psID)";
                var DelCommand = new SqlCommand(SqlDelString, Connection);
                DelCommand.Parameters.AddWithValue("@psID", qsId);
                DelCommand.ExecuteNonQuery();
                Connection.Close();
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("DeleteInvoiceService:Crash:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }
        }

        private void DeleteAllDeletedInvoiceServices(string[] services, string sfInvoiceID)
        {
            try
            {
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.Fw_Quote_Service, sfInvoiceID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(services, i) == -1)
                    {
                        // get MISID
                        int serviceIDTemp = CommonMethods.GetMISID(TableName.Fw_Quote_Service, i, sfInvoiceID, salesForceProjectID);
                        // delete a row
                        DeleteInvoiceService(serviceIDTemp);
                        // remove MISID out of MISSalesForceMapping
                        CommonMethods.Delete(TableName.Fw_Quote_Service, i, sfInvoiceID, salesForceProjectID);
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedInvoiceServices:Error:" + e.Message);
            }
        }

        private void UpdateInvoiceService(long invoiceServiceID, double? cost, string detail, string name, short qsServiceID, string note)
        {
            using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE FW_QUOTE_SERVICE SET qsAmount = @qsAmount, qsAmountText = @qsAmountText, qsTitle = @qsTitle, qsDescription = @qsDescription, qsServiceID = @qsServiceID WHERE (qsID = @qsID)";
                var UpdateCommand = new SqlCommand(UpdateString, Connection);
                if (cost != null && cost > 0)
                {
                    UpdateCommand.Parameters.AddWithValue("@qsAmount", "$" + cost.ToString());
                    UpdateCommand.Parameters.AddWithValue("@qsAmountText", "$" + cost.ToString());
                }

                if (detail != null)
                {
                    UpdateCommand.Parameters.AddWithValue("@qsDescription", detail);
                }
                else
                {
                    UpdateCommand.Parameters.AddWithValue("@qsDescription", "");
                }

                UpdateCommand.Parameters.AddWithValue("@qsTitle", name);
                UpdateCommand.Parameters.AddWithValue("@qsServiceID", qsServiceID);
                UpdateCommand.Parameters.Add("@qsID", invoiceServiceID);

                try
                {
                    Connection.Open();
                    UpdateCommand.ExecuteNonQuery();
                    LogMethods.Log.Debug("UpdateInvoiceService:Debug:" + "DONE");
                }
                catch (SqlException ex)
                {
                    LogMethods.Log.Error("UpdateInvoiceService:Crash:" + ex.Message);
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        private void HandleInvoiceItem(int invoiceID, string listItemID, string sfInvoiceID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    if (string.IsNullOrEmpty(listItemID)) return;
                    string[] items = listItemID.Split(',');
                    /* if no any items, return */
                    if (items.Length == 0) return;

                    //create SQL query statement
                    string query = "SELECT Id, Item_Name__c, Requirement__c, Quote_Item_Description__c, Item_Cost__c, Quantity__c FROM Item__c where Id in (";
                    foreach (string e in items)
                    {
                        if (!string.IsNullOrEmpty(e.Trim()))
                        {
                            query += "'" + e + "',";
                        }
                    }
                    query = query.Remove(query.Length - 1);
                    query += ")";

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
                    IEnumerable<enterprise.Item__c> itemList = result.records.Cast<enterprise.Item__c>();

                    //show results
                    foreach (var il in itemList)
                    {
                        int itemIDTemp = CommonMethods.GetMISID(TableName.Invoice_Item, il.Id, sfInvoiceID, salesForceProjectID);
                        if (itemIDTemp == 0)
                        {
                            InvoiceItemBlank inv = new InvoiceItemBlank(invoiceID);
                            inv.CreateNew();
                            int newId = SqlCommon.GetNewlyInsertedRecordID(TableName.Invoice_Item);
                            CommonMethods.InsertToMISSalesForceMapping(TableName.Invoice_Item, il.Id, newId.ToString(), sfInvoiceID, salesForceProjectID);
                            itemIDTemp = newId;
                        }

                        if (itemIDTemp != 0)
                        {
                            UpdateInvoiceItem(il.Id, itemIDTemp, il.Item_Name__c, il.Requirement__c, il.Quote_Item_Description__c, il.Item_Cost__c, il.Quantity__c);
                        }
                    }

                    /* delete work order items which has been removed out of work order */
                    DeleteAllDeletedInvoiceItems(items, sfInvoiceID);

                    LogMethods.Log.Debug("HandleInvoiceItem:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("HandleInvoiceItem:Error:" + e.Message);
            }
        }

        private void DeleteAllDeletedInvoiceItems(string[] items, string sfInvoiceID)
        {
            try
            {
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.Invoice_Item, sfInvoiceID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(items, i) == -1)
                    {
                        // get MISID
                        int itemIDTemp = CommonMethods.GetMISID(TableName.Invoice_Item, i, sfInvoiceID, salesForceProjectID);
                        // get a row
                        var invoiceItem = _db.Invoice_Item.Where(x => x.quoteItemID == itemIDTemp).FirstOrDefault();
                        if (invoiceItem != null)
                        {
                            _db.Invoice_Item.Remove(invoiceItem);
                            _db.SaveChanges();
                        }
                        // remove MISID out of MISSalesForceMapping
                        CommonMethods.Delete(TableName.Invoice_Item, i, sfInvoiceID, salesForceProjectID);
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedWorkOrderItems:Error:" + e.Message);
            }
        }

        private void UpdateInvoiceItem(string salesforceItemID, long invoiceItemID, string itemName, string requirement, string description, double? itemCost, double? quality)
        {
            var invoiceItem = _db.Invoice_Item.Where(x => x.quoteItemID == invoiceItemID).FirstOrDefault();
            if (invoiceItem != null)
            {
                int estItemID = CommonMethods.GetMISID(TableName.EST_Item, salesforceItemID, salesForceProjectID);
                if (estItemID != 0)
                {
                    invoiceItem.estItemID = estItemID;
                }

                invoiceItem.qiItemTitle = itemName;
                if (description != null)
                {
                    invoiceItem.qiDescription = description; 
                }
                else
                {
                    invoiceItem.qiDescription = ""; 
                }

                int requirementID = 10;
                var jobType = _db.FW_JOB_TYPE.Where(x => x.JOB_TYPE.Trim() == requirement.Trim()).FirstOrDefault();
                if (jobType != null)
                {
                    requirementID = jobType.QUOTE_SUPPLY_TYPE;
                }
                else
                {
                    LogMethods.Log.Error("UpdateInvoiceItem:Debug:" + "Requirement of " + requirement + " doesn't exist on FW_JOB_TYPE table.");
                }
                invoiceItem.supplyType = Convert.ToInt16(requirementID);

                if (itemCost != null)
                {
                    invoiceItem.qiAmount = Convert.ToDecimal(itemCost);
                    invoiceItem.qiAmountText = "$" + itemCost;
                }

                if (quality != null)
                {
                    invoiceItem.qiQty = Convert.ToInt16(quality);
                }

                _db.Entry(invoiceItem).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }

        private void UpdateInvoice(int invoiceID, string invoiceNumber, DateTime? issueDate, int sale, string term, string contractNo,
            string shipMethod, DateTime? contractDate, string taxOption, double? tax, double? deposit, double? discount)
        {
            var invoice = _db.Sales_JobMasterList_Invoice.Where(x => x.invoiceID == invoiceID).FirstOrDefault();
            if (invoice != null)
            {
                invoice.invoiceNo = invoiceNumber;
                if (issueDate != null)
                {
                    invoice.invoiceDate = issueDate;
                }
                invoice.Sales = sale;

                if (!string.IsNullOrEmpty(term))
                {
                    switch (term)
                    {
                        case "Cash On Delivery":
                            invoice.Term = 0;
                            break;
                        case "Customer Net 7 Days":
                            invoice.Term = 7;
                            break;
                        case "Customer Net 10 Days":
                            invoice.Term = 10;
                            break;
                        case "Customer Net 15 Days":
                            invoice.Term = 15;
                            break;
                        case "Customer Net 20 Days":
                            invoice.Term = 20;
                            break;
                        case "Customer Net 30 Days":
                            invoice.Term = 30;
                            break;
                        case "Customer Net 45 Days":
                            invoice.Term = 45;
                            break;
                        case "Customer Net 60 Days":
                            invoice.Term = 60;
                            break;
                        case "Customer Net 180 Days":
                            invoice.Term = 180;
                            break;
                        case "Due Upon Receipt":
                            invoice.Term = 100;
                            break;
                        case "75 3WD":
                            invoice.Term = 200;
                            break;
                        default:
                            invoice.Term = 1000;
                            break;
                    }
                }

                invoice.ContractNo = contractNo;
                if (!string.IsNullOrEmpty(shipMethod))
                {
                    invoice.ShipVia = shipMethod;
                }

                if (contractDate != null)
                {
                    invoice.contractDate = contractDate;
                }

                switch (taxOption)
                {
                    case "HST":
                        invoice.TaxOption = (short)NTaxOption.HST;
                        break;
                    case "HST-BC":
                        invoice.TaxOption = (int)NTaxOption.HstBC;
                        break;
                    case "GST Only":
                        invoice.TaxOption = (int)NTaxOption.GstOnly;
                        break;
                    case "GST & PST":
                        invoice.TaxOption = (int)NTaxOption.GstAndPst;
                        break;
                    case "Manually":
                        invoice.TaxOption = (int)NTaxOption.Manually;
                        if (tax != null)
                        {
                            invoice.pstAmount = Convert.ToDecimal(tax);
                        }
                        break;
                    case "No Tax":
                        invoice.TaxOption = (int)NTaxOption.NoTax;
                        break;
                    default:
                        break;
                }

                if (deposit != null) {
                    invoice.Deposit = Convert.ToDecimal(deposit);
                }

                if (discount != null)
                {
                    invoice.Discount = (-1) * Convert.ToDecimal(discount);
                }

                _db.Entry(invoice).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }



    }
}