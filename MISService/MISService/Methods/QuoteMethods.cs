using MISService.Method;
using MISService.Models;
using MyCommon.MyEnum;
using ProjectDomain;
using SalesCenterDomain.BDL;
using SalesCenterDomain.BDL.Item;
using SalesCenterDomain.BDL.Project;
using SalesCenterDomain.BDL.Quote;
using SalesCenterDomain.BDL.Service;
using SalesCenterDomain.BLL;
using SalesCenterDomain.BO;
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
        private string salesForceProjectID;

        public QuoteMethods(string salesForceProjectID)
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
            this.salesForceProjectID = salesForceProjectID;
        }

        public void GetAllQuotes(string sfProjectID, int jobID, int estRevID, int userEmployeeID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Name, Status__c, Sub_Total__c, SubTotal_Discount__c, "
                        + " Contract_Number__c, Contract_Amount__c, Contract_Issue_Date__c, Contract_Due_Date__c, Deposit__c, Terms__c, Version__c, "
                        + " Tax_Option__c, Tax_Rate__c, Project_Name__r.Currency__c, "
                        + " (SELECT Id, Title__c, Content__c FROM Notes__r), "
                        + " (SELECT Id, Item_Name__c, Item_Order__c, Requirement__c, Item_Description__c, Item_Cost__c, Quantity__c, Item_Option__c FROM Items__r), "
                        + " (SELECT Id, Service_Name__r.Name, Detail__c, Service_Cost__c,Note__c, Service_Name__r.MIS_Service_Number__c FROM Service_Costs__r) "
                        + " FROM Quotation__c "
                        + " WHERE Project_Name__c = '" + sfProjectID + "'";
                    
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
                    IEnumerable<enterprise.Quotation__c> quoteList = result.records.Cast<enterprise.Quotation__c>();

                    foreach (var ql in quoteList)
                    {
                        /* check if the quote exists */
                        int quoteID = CommonMethods.GetMISID(TableName.Sales_JobMasterList_quoteRev, ql.Id, salesForceProjectID);
                        if (quoteID == 0)
                        {
                            // not exist
                            // generate quote title
                            var qt = new QuoteTitleGenerate(jobID, estRevID);
                            qt.GenerateTitle();
                            int quoteRevID = qt.GetNewID();
                            if (quoteRevID > 0)
                            {
                                CommonMethods.InsertToMISSalesForceMapping(TableName.Sales_JobMasterList_quoteRev, ql.Id, quoteRevID.ToString(), salesForceProjectID);
                            }
                            quoteID = quoteRevID;
                        }

                        if (quoteID != 0)
                        {
                            UpdateQuote(quoteID, ql.Sub_Total__c, ql.SubTotal_Discount__c, ql.Version__c, ql.Tax_Option__c, ql.Tax_Rate__c, ql.Terms__c, ql.Project_Name__r.Currency__c);

                            // handle quote items
                            HandleQuoteItem(jobID, estRevID, quoteID, ql.Id, ql.Items__r);

                            // handle services
                            HandleQuoteService(jobID, estRevID, quoteID, ql.Id, ql.Service_Costs__r);

                            // handle notes
                            HandleNotes(jobID, estRevID, quoteID, ql.Notes__r, ql.Id);

                            if (ql.Status__c == "Accepted")
                            {
                                // update contract information
                                UpdateWINContract(quoteID, ql.Contract_Number__c, ql.Contract_Issue_Date__c, ql.Contract_Due_Date__c, ql.Contract_Amount__c, ql.Deposit__c, ql.Terms__c);
                            }
                            else if (ql.Status__c == "Denied")
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

        private void UpdateNote(string qnTitle, string qnDescription, int qnID)
        {
            try
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
                        LogMethods.Log.Debug("UpdateNote:Debug:" + "Done");
                    }
                    catch (SqlException ex)
                    {
                        LogMethods.Log.Error("UpdateNote:Error:" + ex.Message);
                    }
                    finally
                    {
                        Connection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("UpdateNote:Error :" + e.Message);
            }
        }

        private void HandleNotes(int jobID, int estRevID, int quoteRevID, enterprise.QueryResult result, string sfQuoteID)
        {
            try
            {
                if (result != null)
                {
                    IEnumerable<enterprise.Note__c> quoteList = result.records.Cast<enterprise.Note__c>();
                    List<string> notes = new List<string>();
                    foreach (var q in quoteList)
                    {
                        notes.Add(q.Id);

                        int noteID = CommonMethods.GetMISID(TableName.Fw_Quote_Note, q.Id, sfQuoteID, salesForceProjectID);
                        if (noteID == 0)
                        {
                            QuoteNoteCreateNew qnc = new QuoteNoteCreateNew(quoteRevID);
                            qnc.Insert();

                            int newNoteId = SqlCommon.GetNewlyInsertedRecordID(TableName.Fw_Quote_Note);
                            if (newNoteId > 0)
                            {
                                CommonMethods.InsertToMISSalesForceMapping(TableName.Fw_Quote_Note, q.Id, newNoteId.ToString(), sfQuoteID, salesForceProjectID);
                            }
                            noteID = newNoteId;
                        }

                        if (noteID != 0)
                        {
                            UpdateNote(q.Title__c, q.Content__c, noteID);
                        }
                    }

                    DeleteAllDeletedQuoteNotes(notes.ToArray(), sfQuoteID);
                    LogMethods.Log.Debug("HandleNotes:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("HandleNotes:Error:" + e.Message);
            }
        }

        private void DeleteQuoteNote(int qnId)
        {
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                Connection.Open();
                string SqlDelString = "DELETE FROM FW_Quote_Note WHERE ([qnID] = @pnID)";
                var DelCommand = new SqlCommand(SqlDelString, Connection);
                DelCommand.Parameters.AddWithValue("@pnID", qnId);
                DelCommand.ExecuteNonQuery();
                Connection.Close();
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("DeleteQuoteNote:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }
        }

        private void DeleteAllDeletedQuoteNotes(string[] notes, string sfQuoteID)
        {
            try
            {
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.Fw_Quote_Note, sfQuoteID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(notes, i) == -1)
                    {
                        // get MISID
                        int serviceIDTemp = CommonMethods.GetMISID(TableName.Fw_Quote_Note, i, sfQuoteID, salesForceProjectID);
                        // delete a row
                        DeleteQuoteNote(serviceIDTemp);
                        // remove MISID out of MISSalesForceMapping
                        CommonMethods.Delete(TableName.Fw_Quote_Note, i, sfQuoteID, salesForceProjectID);
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedQuoteNotes:Error:" + e.Message);
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
            try
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
            catch (Exception e)
            {
                LogMethods.Log.Error("UpdateWINContract:Error:" + e.Message);
            }
        }

        private void UpdateQuote(int quoteRevID, double? subTotal, double? discountAmount, double? version, string taxOption, double? taxRate, string term, string currency)
        {
            try
            {
                var sales_JobMasterList_quoteRev = _db.Sales_JobMasterList_QuoteRev.Where(x => x.quoteRevID == quoteRevID).FirstOrDefault();
                if (sales_JobMasterList_quoteRev != null)
                {
                    if (discountAmount != null)
                    {
                        sales_JobMasterList_quoteRev.DiscountAmount = Convert.ToDecimal(discountAmount);
                    }

                    if (version != null)
                    {
                        sales_JobMasterList_quoteRev.quoteRev = Convert.ToByte(version);
                    }

                    if (currency != null)
                    {
                        sales_JobMasterList_quoteRev.Currency = currency;
                    }

                    switch (taxOption)
                    {
                        case "HST":
                            sales_JobMasterList_quoteRev.TaxOption = (int)NTaxOption.HST;
                            break;
                        case "HST-BC":
                            sales_JobMasterList_quoteRev.TaxOption = (int)NTaxOption.HstBC;
                            break;
                        case "GST Only":
                            sales_JobMasterList_quoteRev.TaxOption = (int)NTaxOption.GstOnly;
                            break;
                        case "GST & PST":
                            sales_JobMasterList_quoteRev.TaxOption = (int)NTaxOption.GstAndPst;
                            break;
                        case "Manually":
                            sales_JobMasterList_quoteRev.TaxOption = (int)NTaxOption.Manually;
                            if (subTotal != null && discountAmount != null)
                            {
                                sales_JobMasterList_quoteRev.pstAmount = Convert.ToDecimal((subTotal - discountAmount) * Convert.ToInt16(taxRate) * 0.01);
                            }
                            break;
                        case "No Tax":
                            sales_JobMasterList_quoteRev.TaxOption = (int)NTaxOption.NoTax;
                            break;
                        default:
                            break;
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

                    _db.Entry(sales_JobMasterList_quoteRev).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("UpdateQuote:Error:" + e.Message);
            }

        }

        private void HandleQuoteService(int jobID, int estRevID, int quoteRevID, string sfQuoteID, enterprise.QueryResult result)
        {
            try
            {
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    if (result == null || (result != null && result.size == 0)) return;

                    IEnumerable<enterprise.Service_Cost__c> serviceList = result.records.Cast<enterprise.Service_Cost__c>();
                    var svc = new FsService(quoteRevID, "Quote");
                    List<string> services = new List<string>();
                    foreach (var sl in serviceList)
                    {
                        services.Add(sl.Id);
                        long estServiceID = CommonMethods.GetMISID(TableName.Fw_Quote_Service, sl.Id, sfQuoteID, salesForceProjectID);
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
                            }
                            else
                            {
                                svc.InsertRecord(Convert.ToInt32(sl.Service_Name__r.MIS_Service_Number__c),
                                    sl.Note__c,
                                    1,
                                    sl.Detail__c == null ? "" : sl.Detail__c,
                                    sl.Service_Name__r.Name,
                                    sl.Note__c,
                                    printOrder
                               );
                            }
                            int qs_id = SqlCommon.GetNewlyInsertedRecordID(TableName.Fw_Quote_Service);
                            if (qs_id > 0)
                            {
                                CommonMethods.InsertToMISSalesForceMapping(TableName.Fw_Quote_Service, sl.Id, qs_id.ToString(), sfQuoteID, salesForceProjectID);
                            }
                        }
                        else
                        {
                            UpdateQuoteService(estServiceID, sl.Service_Cost__c1, sl.Detail__c, sl.Service_Name__r.Name, Convert.ToInt16(sl.Service_Name__r.MIS_Service_Number__c), sl.Note__c);
                        }

                    }

                    DeleteAllDeletedQuoteServices(services.ToArray(), sfQuoteID);
                    LogMethods.Log.Debug("HandleQuoteService:Debug:" + "Done");
                }

            }
            catch (Exception e)
            {
                LogMethods.Log.Error("HandleQuoteService:Error:" + e.Message);
            }
        }

        private void DeleteQuoteService(int qsId)
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
                LogMethods.Log.Error("DeleteQuoteService:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }
        }

        private void DeleteAllDeletedQuoteServices(string[] services, string sfQuoteID)
        {
            try
            {
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.Fw_Quote_Service, sfQuoteID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(services, i) == -1)
                    {
                        // get MISID
                        int serviceIDTemp = CommonMethods.GetMISID(TableName.Fw_Quote_Service, i, sfQuoteID, salesForceProjectID);
                        // delete a row
                        DeleteQuoteService(serviceIDTemp);
                        // remove MISID out of MISSalesForceMapping
                        CommonMethods.Delete(TableName.Fw_Quote_Service, i, sfQuoteID, salesForceProjectID);
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedQuoteServices:Error:" + e.Message);
            }
        }

        private void UpdateQuoteService(long quoteServiceID, double? cost, string detail, string name, short qsServiceID, string note)
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
                else
                {
                    UpdateCommand.Parameters.AddWithValue("@qsAmount", note);
                    UpdateCommand.Parameters.AddWithValue("@qsAmountText", note);
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
                UpdateCommand.Parameters.AddWithValue("@qsID", quoteServiceID);

                try
                {
                    Connection.Open();
                    UpdateCommand.ExecuteNonQuery();
                    LogMethods.Log.Debug("UpdateQuoteService:Debug:" + "Done");
                }
                catch (SqlException ex)
                {
                    LogMethods.Log.Error("UpdateQuoteService:Error:" + ex.Message);
                }
                finally
                {
                    Connection.Close();
                }
            }

        }

        private void HandleQuoteItem(int jobID, int estRevID, int quoteRevID, string sfQuoteID, enterprise.QueryResult result)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    if (result == null || (result != null && result.size == 0)) return;

                    //cast query results
                    IEnumerable<enterprise.Item__c> itemList = result.records.Cast<enterprise.Item__c>();
                    List<string> items = new List<string>();
                    //show results
                    foreach (var il in itemList)
                    {
                        items.Add(il.Id);
                        int itemIDTemp = CommonMethods.GetMISID(TableName.Quote_Item, il.Id, sfQuoteID, salesForceProjectID);
                        if (itemIDTemp == 0)
                        {
                            var qt = new QuoteTitleGenerate(jobID, estRevID);
                            qt.MyID = quoteRevID;

                            int itemID = CommonMethods.GetEstimationItemID(estRevID, il.Item_Name__c);
                            if (itemID != 0)
                            {
                                int quoteItemID = qt.GenerateNewItems(itemID);
                                if (quoteItemID > 0)
                                {
                                    CommonMethods.InsertToMISSalesForceMapping(TableName.Quote_Item, il.Id, quoteItemID.ToString(), sfQuoteID, salesForceProjectID);
                                    itemIDTemp = quoteItemID;
                                }
                            }
                            else
                            {
                                QuoteItemBlank qib = new QuoteItemBlank(quoteRevID);
                                qib.CreateNew();
                                int quoteItemID = qib.NewID;
                                if (quoteItemID > 0)
                                {
                                    CommonMethods.InsertToMISSalesForceMapping(TableName.Quote_Item, il.Id, quoteItemID.ToString(), sfQuoteID, salesForceProjectID);
                                    itemIDTemp = quoteItemID;
                                }
                            }
                        }

                        if (itemIDTemp != 0)
                        {
                            UpdateQuoteItem(itemIDTemp, il.Item_Name__c, il.Requirement__c, il.Item_Description__c, il.Item_Cost__c, il.Quantity__c, il.Item_Order__c, il.Item_Option__c);
                        }
                    }

                    DeleteAllDeletedQuoteItems(items.ToArray(), sfQuoteID);
                    LogMethods.Log.Debug("HandleQuoteItem:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("HandleQuoteItem:Error:" + e.Message);
            }
        }

        private void DeleteAllDeletedQuoteItems(string[] items, string sfQuoteID)
        {
            try
            {
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.Quote_Item, sfQuoteID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(items, i) == -1)
                    {
                        // get MISID
                        int itemIDTemp = CommonMethods.GetMISID(TableName.Quote_Item, i, sfQuoteID, salesForceProjectID);
                        // get a row
                        var quoteItem = _db.Quote_Item.Where(x => x.quoteItemID == itemIDTemp).FirstOrDefault();
                        if (quoteItem != null)
                        {
                            _db.Quote_Item.Remove(quoteItem);
                            _db.SaveChanges();
                        }
                        // remove MISID out of MISSalesForceMapping
                        CommonMethods.Delete(TableName.Quote_Item, i, sfQuoteID, salesForceProjectID);
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedQuoteItems:Error:" + e.Message);
            }
        }

        private void UpdateQuoteItem(long quoteItemID, string itemName, string requirement, string description, double? itemCost, double? quality, double? itemOrder, double? itemOption)
        {
            using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE Quote_Item SET qiItemTitle = @qiItemTitle, supplyType = @supplyType, qiDescription = @qiDescription, "
                               + " qiAmount = @qiAmount, qiAmountText = @qiAmountText, qiQty = @qiQty, qiPrintOrder = @qiPrintOrder, quoteOption = @quoteOption, quoteOptionText = @quoteOptionText WHERE (quoteItemID = @quoteItemID)";
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
                    if (requirement != null && requirement.Trim() == "Supply Only")
                    {
                        requirementID = 70;
                    }
                    else
                    {
                        LogMethods.Log.Error("UpdateQuoteItem:Debug:" + "Requirement of " + requirement + " doesn't exist on FW_JOB_TYPE table.");
                    }
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

                if (itemOrder != null)
                {
                    UpdateCommand.Parameters.AddWithValue("@qiPrintOrder", itemOrder);
                }
                else
                {
                    UpdateCommand.Parameters.AddWithValue("@qiPrintOrder", 1);
                }

                if (itemOption != null)
                {
                    UpdateCommand.Parameters.AddWithValue("@quoteOption", itemOption);
                    UpdateCommand.Parameters.AddWithValue("@quoteOptionText", Convert.ToInt16(itemOption).ToString());
                }
                else
                {
                    UpdateCommand.Parameters.AddWithValue("@quoteOption", 1);
                    UpdateCommand.Parameters.AddWithValue("@quoteOptionText", "1");
                }
                    
                try
                {
                    Connection.Open();
                    UpdateCommand.ExecuteNonQuery();
                    LogMethods.Log.Debug("UpdateQuoteItem:Debug:" + "Done");
                }
                catch (SqlException ex)
                {
                    LogMethods.Log.Error("UpdateQuoteItem:Error:" + ex.Message);
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

    }
}
