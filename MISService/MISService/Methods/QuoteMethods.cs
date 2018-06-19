﻿using MISService.Method;
using MISService.Models;
using MyCommon.MyEnum;
using ProjectDomain;
using SalesCenterDomain.BDL;
using SalesCenterDomain.BDL.Item;
using SalesCenterDomain.BDL.Project;
using SalesCenterDomain.BDL.Quote;
using SalesCenterDomain.BDL.Service;
using SalesCenterDomain.BLL;
using SpecDomain.Model;
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
    public class QuoteMethods
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private EndpointAddress apiAddr;
        private enterprise.SessionHeader header;

        public QuoteMethods()
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
        }

        public void GetAllQuotes(string sfProjectID, int jobID, int estRevID, int userEmployeeID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Name, (select Id, Title, TextPreview from AttachedContentNotes), Status, List_Item_Name__c, List_Service_Name__c, SubTotal_Discount__c, "
                        + " Contract_Number__c, Contract_Amount__c, Contract_Issue_Date__c, Contract_Due_Date__c, Deposit__c, Terms__c "
                        + " FROM Quote where OpportunityId = '" + sfProjectID + "'";

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
                    IEnumerable<enterprise.Quote> quoteList = result.records.Cast<enterprise.Quote>();

                    foreach (var ql in quoteList)
                    {
                        /* check if the quote exists */
                        int quoteID = CommonMethods.GetMISID(TableName.Sales_JobMasterList_quoteRev, ql.Id);
                        if (quoteID == 0)
                        {
                            // not exist
                            // generate quote title
                            var qt = new QuoteTitleGenerate(jobID, estRevID);
                            qt.GenerateTitle();
                            int quoteRevID = qt.GetNewID();
                            CommonMethods.InsertToMISSalesForceMapping(TableName.Sales_JobMasterList_quoteRev, ql.Id, quoteRevID.ToString());
                            quoteID = quoteRevID;
                        }

                        if (quoteID != 0)
                        {
                            UpdateQuote(quoteID, ql.SubTotal_Discount__c);

                            // generate quote items
                            GenerateQuoteItem(jobID, estRevID, quoteID, ql.List_Item_Name__c, ql.Id);

                            // generate services
                            GenerateQuoteService(jobID, estRevID, quoteID, ql.List_Service_Name__c, ql.Id);

                            // generate notes
                            GenerateNotes(jobID, estRevID, quoteID, ql.AttachedContentNotes);

                            if (ql.Status == "Accepted")
                            {
                                // update contract information
                                UpdateWINContract(quoteID, ql.Contract_Number__c, ql.Contract_Issue_Date__c, ql.Contract_Due_Date__c, ql.Contract_Amount__c, ql.Deposit__c, ql.Terms__c);
                            }
                            else if (ql.Status == "Denied")
                            {
                                UpdateLOSSNContract(quoteID);
                            }
                        }

                    }
                    LogMethods.Log.Debug("GetAllQuote:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllQuote:Error:" + e.Message);
            }
        }

        public void UpdateNote(string qnTitle, string qnDescription, int qnID)
        {
            using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE FW_Quote_Note SET qnTitle = @qnTitle, qnDescription = @qnDescription WHERE (qnID = @qnID)";
                var UpdateCommand = new SqlCommand(UpdateString, Connection);
                if (qnTitle != null)
                {
                    UpdateCommand.Parameters.AddWithValue("@qnTitle", qnTitle);
                }
                else
                {
                    UpdateCommand.Parameters.AddWithValue("@qnTitle", "");
                }

                if (qnDescription != null)
                {
                    UpdateCommand.Parameters.AddWithValue("@qnDescription", qnDescription);
                }
                else
                {
                    UpdateCommand.Parameters.AddWithValue("@qnDescription", "");
                }


                UpdateCommand.Parameters.AddWithValue("@qnID", qnID);

                try
                {
                    Connection.Open();
                    UpdateCommand.ExecuteNonQuery();
                    LogMethods.Log.Debug("UpdateNote:Debug:" + "DONE");
                }
                catch (SqlException ex)
                {
                    LogMethods.Log.Error("UpdateNote:Crash:" + ex.Message);
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        public void GenerateNotes(int jobID, int estRevID, int quoteRevID, enterprise.QueryResult result)
        {
            if(result != null) {

                IEnumerable<enterprise.AttachedContentNote> quoteList = result.records.Cast<enterprise.AttachedContentNote>();
                foreach (var q in quoteList)
                {
                    int noteID = CommonMethods.GetMISID(TableName.Fw_Quote_Note, q.Id);
                    if(noteID == 0) {
                        QuoteNoteCreateNew qnc = new QuoteNoteCreateNew(quoteRevID);
                        qnc.Insert();

                        int newNoteId = SqlCommon.GetNewlyInsertedRecordID(TableName.Fw_Quote_Note);
                        CommonMethods.InsertToMISSalesForceMapping(TableName.Fw_Quote_Note, q.Id, newNoteId.ToString());
                        noteID = newNoteId;
                    }

                    UpdateNote(q.Title, q.TextPreview, noteID);
                }
            }
        }

        private void UpdateLOSSNContract(int quoteRevID)
        {
            var sales_JobMasterList_quoteRev = _db.Sales_JobMasterList_QuoteRev.Where(x => x.quoteRevID == quoteRevID).FirstOrDefault();
            if (sales_JobMasterList_quoteRev != null)
            {
                sales_JobMasterList_quoteRev.quoteStatus = (int)NJobStatus.Loss; //WIN status is defined in Sales_JobStatus

                _db.Entry(sales_JobMasterList_quoteRev).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }

        private void UpdateWINContract(int quoteRevID, string contractNum, DateTime? issueDate, DateTime? dueDate, double? contractAmount, double? deposit, string term)
        {
            var sales_JobMasterList_quoteRev = _db.Sales_JobMasterList_QuoteRev.Where(x => x.quoteRevID == quoteRevID).FirstOrDefault();
            if (sales_JobMasterList_quoteRev != null)
            {
                if (deposit != null)
                {
                    sales_JobMasterList_quoteRev.termDeposit = Convert.ToInt16(deposit);
                }

                sales_JobMasterList_quoteRev.contractNumber = contractNum;

                if (issueDate != null)
                {
                    sales_JobMasterList_quoteRev.isssueDate = issueDate;
                }

                if (dueDate != null)
                {
                    sales_JobMasterList_quoteRev.contractDate = dueDate;
                }

                if (contractAmount != null)
                {
                    sales_JobMasterList_quoteRev.contractAmount = Convert.ToDecimal(contractAmount);
                }

                if (!string.IsNullOrEmpty(term))
                {
                    switch (term)
                    {
                        case "Cash On Delivery":
                            sales_JobMasterList_quoteRev.termBalance = 0;
                            break;
                        case "Customer Net 7 Days":
                            sales_JobMasterList_quoteRev.termBalance = 7;
                            break;
                        case "Customer Net 10 Days":
                            sales_JobMasterList_quoteRev.termBalance = 10;
                            break;
                        case "Customer Net 15 Days":
                            sales_JobMasterList_quoteRev.termBalance = 15;
                            break;
                        case "Customer Net 20 Days":
                            sales_JobMasterList_quoteRev.termBalance = 20;
                            break;
                        case "Customer Net 30 Days":
                            sales_JobMasterList_quoteRev.termBalance = 30;
                            break;
                        case "Customer Net 45 Days":
                            sales_JobMasterList_quoteRev.termBalance = 45;
                            break;
                        case "Customer Net 60 Days":
                            sales_JobMasterList_quoteRev.termBalance = 60;
                            break;
                        case "Customer Net 180 Days":
                            sales_JobMasterList_quoteRev.termBalance = 180;
                            break;
                        case "Due Upon Receipt":
                            sales_JobMasterList_quoteRev.termBalance = 100;
                            break;
                        case "75 3WD":
                            sales_JobMasterList_quoteRev.termBalance = 200;
                            break;
                        default:
                            sales_JobMasterList_quoteRev.termBalance = 1000;
                            break;
                    }
                }
                sales_JobMasterList_quoteRev.quoteStatus = (int)NJobStatus.win; //WIN status is defined in Sales_JobStatus

                _db.Entry(sales_JobMasterList_quoteRev).State = EntityState.Modified;
                _db.SaveChanges();

            }
        }

        private void UpdateQuote(int quoteRevID, double? discountAmount)
        {

            var sales_JobMasterList_quoteRev = _db.Sales_JobMasterList_QuoteRev.Where(x => x.quoteRevID == quoteRevID).FirstOrDefault();
            if (sales_JobMasterList_quoteRev != null)
            {
                if (discountAmount != null)
                {
                    sales_JobMasterList_quoteRev.DiscountAmount = Convert.ToDecimal(discountAmount);
                }

                _db.Entry(sales_JobMasterList_quoteRev).State = EntityState.Modified;
                _db.SaveChanges();
            }

        }

        public void GenerateQuoteService(int jobID, int estRevID, int quoteRevID, string listServiceID, string sfQuoteID)
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
                    string query = "SELECT Id, Service_Name__r.Name, Detail__c, Service_Cost__c, Service_Name__r.MIS_Service_Number__c FROM Service_Cost__c where Id in (";
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
                    var svc = new FsService(quoteRevID, "Quote");
                    foreach (var sl in serviceList)
                    {
                        long estServiceID = CommonMethods.GetMISID(TableName.Fw_Quote_Service, sl.Id, sfQuoteID);
                        if (estServiceID == 0)
                        {
                            int printOrder = svc.GetQsMaxPrintOrder() + 1;
                            svc.InsertRecord(Convert.ToInt32(sl.Service_Name__r.MIS_Service_Number__c),
                                 sl.Service_Cost__c1 == null? "0": sl.Service_Cost__c1.ToString(),
                                 1,
                                 sl.Detail__c == null ? "" : sl.Detail__c,
                                 sl.Service_Name__r.Name,
                                 sl.Service_Cost__c1 == null ? "0" : sl.Service_Cost__c1.ToString(),
                                 printOrder
                            );
                            int qs_id = SqlCommon.GetNewlyInsertedRecordID(TableName.Fw_Quote_Service);
                            CommonMethods.InsertToMISSalesForceMapping(TableName.Fw_Quote_Service, sl.Id, qs_id.ToString(), sfQuoteID);
                        }
                        else
                        {
                            UpdateQuoteService(estServiceID, sl.Service_Cost__c1, sl.Detail__c, sl.Service_Name__r.Name, Convert.ToInt16(sl.Service_Name__r.MIS_Service_Number__c));
                        }
                        LogMethods.Log.Debug("GenerateQuoteService:Debug:" + "Done");
                    }
                }

            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GenerateQuoteService:Error:" + e.Message);
            }
        }

        private void UpdateQuoteService(long quoteServiceID, double? cost, string detail, string name, short qsServiceID)
        {
            using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE FW_QUOTE_SERVICE SET qsAmount = @qsAmount, qsAmountText = @qsAmountText, qsTitle = @qsTitle, qsDescription = @qsDescription, qsServiceID = @qsServiceID WHERE (qsID = @qsID)";
                var UpdateCommand = new SqlCommand(UpdateString, Connection);
                if (cost != null)
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
                UpdateCommand.Parameters.Add("@qsID", quoteServiceID);

                try
                {
                    Connection.Open();
                    UpdateCommand.ExecuteNonQuery();
                    LogMethods.Log.Debug("UpdateQuoteService:Debug:" + "DONE");
                }
                catch (SqlException ex)
                {
                    LogMethods.Log.Error("UpdateQuoteService:Crash:" + ex.Message);
                }
                finally
                {
                    Connection.Close();
                }
            }

        }

        private void GenerateQuoteItem(int jobID, int estRevID, int quoteRevID, string listItemID, string sfQuoteID)
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
                    string query = "SELECT Id, Item_Name__c, Requirement__c, Description__c, Item_Cost__c, Quantity__c FROM Item__c where Id in (";
                    foreach (string e in items)
                    {
                        if(!string.IsNullOrEmpty(e.Trim())) {
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
                        int itemIDTemp = CommonMethods.GetMISID(TableName.Quote_Item, il.Id, sfQuoteID);
                        if (itemIDTemp == 0)
                        {
                            var qt = new QuoteTitleGenerate(jobID, estRevID);
                            qt.MyID = quoteRevID;

                            int itemID = CommonMethods.GetMISID(TableName.EST_Item, il.Id);
                            if (itemID != 0)
                            {
                                int quoteItemID = qt.GenerateNewItems(itemID);
                                if (quoteItemID != 0)
                                {
                                    CommonMethods.InsertToMISSalesForceMapping(TableName.Quote_Item, il.Id, quoteItemID.ToString(), sfQuoteID);
                                }
                            }
                        }
                        else
                        {
                            UpdateQuoteItem(itemIDTemp, il.Item_Name__c, il.Requirement__c, il.Description__c, il.Item_Cost__c, il.Quantity__c);
                        }
                    }

                    LogMethods.Log.Debug("GenerateQuoteItem:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GenerateQuoteItem:Error:" + e.Message);
            }
        }

        private void UpdateQuoteItem(long quoteItemID, string itemName, string requirement, string description, double? itemCost, double? quality)
        {
            using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE Quote_Item SET qiItemTitle = @qiItemTitle, supplyType = @supplyType, qiDescription = @qiDescription, "
                               + " qiAmount = @qiAmount, qiAmountText = @qiAmountText, qiQty = @qiQty WHERE (quoteItemID = @quoteItemID)";
                var UpdateCommand = new SqlCommand(UpdateString, Connection);
                if (itemName != null)
                {
                    UpdateCommand.Parameters.AddWithValue("@qiItemTitle", itemName);
                }
                else
                {
                    UpdateCommand.Parameters.AddWithValue("@qiItemTitle", "");
                }

                int requirementID = 10;
                var jobType = _db.FW_JOB_TYPE.Where(x => x.JOB_TYPE.Trim() == requirement.Trim()).FirstOrDefault();
                if (jobType != null)
                {
                    requirementID = jobType.QUOTE_SUPPLY_TYPE;
                }
                else
                {
                    LogMethods.Log.Error("UpdateEstItem:Debug:" + "Requirement of " + requirement + " doesn't exist on FW_JOB_TYPE table.");
                }
                UpdateCommand.Parameters.AddWithValue("@supplyType", requirementID);

                if (description != null)
                {
                    UpdateCommand.Parameters.AddWithValue("@qiDescription", description);
                }
                else
                {
                    UpdateCommand.Parameters.AddWithValue("@qiDescription", "");
                }

                if (itemCost != null)
                {
                    UpdateCommand.Parameters.AddWithValue("@qiAmount", itemCost);
                    UpdateCommand.Parameters.AddWithValue("@qiAmountText", "$" + itemCost);
                }
                else
                {
                    UpdateCommand.Parameters.AddWithValue("@qiAmount", 0);
                    UpdateCommand.Parameters.AddWithValue("@qiAmountText", "$" + 0);
                }

                if (quality != null)
                {
                    UpdateCommand.Parameters.AddWithValue("@qiQty", quality);
                }
                else
                {
                    UpdateCommand.Parameters.AddWithValue("@qiQty", 1);
                }
                UpdateCommand.Parameters.AddWithValue("@quoteItemID", quoteItemID);
                    
                try
                {
                    Connection.Open();
                    UpdateCommand.ExecuteNonQuery();
                    LogMethods.Log.Debug("UpdateQuoteItem:Debug:" + "DONE");
                }
                catch (SqlException ex)
                {
                    LogMethods.Log.Error("UpdateQuoteItem:Crash:" + ex.Message);
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

    }
}