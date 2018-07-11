using MISService.Method;
using MISService.Models;
using ProjectDomain;
using SalesCenterDomain.BDL;
using SalesCenterDomain.BDL.Task;
using SalesCenterDomain.BDL.Workorder;
using SalesCenterDomain.BO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using enterprise = MISService.SfdcReference;

namespace MISService.Methods
{
    public class WorkOrderMethods
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private EndpointAddress apiAddr;
        private enterprise.SessionHeader header;
        private string salesForceProjectID;

        public WorkOrderMethods(string salesForceProjectID)
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
            this.salesForceProjectID = salesForceProjectID;
        }

        public void GetAllWorkOrders(string sfProjectID, int jobID, int estRevID, int userEmployeeID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Name, (select Id, Title, TextPreview from AttachedContentNotes), Work_Order_Type__c, Payment_Method__c, Version__c, Rush__c, Rush_Reason__c, Remarks__c, "
                        + " Issue_Date__c, Due_Date__c, Clone_Type__c, Previous_Work_Order_Number__c, List_Item_Name__c, Site_Check_Purpose__c, Site_Check_Purpose_As_Other__c "
                        + " FROM Work_Order__c where Project_Name__c = '" + sfProjectID + "'";

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
                    IEnumerable<enterprise.Work_Order__c> workOrderList = result.records.Cast<enterprise.Work_Order__c>();

                    foreach (var ql in workOrderList)
                    {
                        /* check if the work order exists */
                        int workOrderID = CommonMethods.GetMISID(TableName.Sales_JobMasterList_WO, ql.Id, salesForceProjectID);
                        if (workOrderID == 0)
                        {
                            // not exist
                            WorkorderGenerateFromEstimation gw = new WorkorderGenerateFromEstimation(jobID, estRevID);
                            gw.CreateNew();
                            workOrderID = gw.WoID;
                            CommonMethods.InsertToMISSalesForceMapping(TableName.Sales_JobMasterList_WO, ql.Id, workOrderID.ToString(), salesForceProjectID);
                        }

                        if (workOrderID != 0)
                        {
                            UpdateWorkOrder(workOrderID, ql.Name, ql.Work_Order_Type__c, ql.Payment_Method__c, ql.Version__c, ql.Rush__c, ql.Rush_Reason__c,
                                ql.Remarks__c, ql.Issue_Date__c, ql.Due_Date__c, ql.Clone_Type__c, ql.Previous_Work_Order_Number__c, ql.Site_Check_Purpose__c, ql.Site_Check_Purpose_As_Other__c, ql.Id);

                            // generate work order items
                            HandleWorkOrderItem(workOrderID, ql.List_Item_Name__c, ql.Id);

                            switch (ql.Work_Order_Type__c)
                            {
                                case "Production":
                                    ProductionWOMethods pm = new ProductionWOMethods(salesForceProjectID);
                                    pm.GetAllWorkShopInstructions(workOrderID, ql.Id);
                                    pm.GetAllInstallerInstructions(workOrderID, ql.Id);
                                    pm.GetAllCheckLists(workOrderID, ql.Id);
                                    pm.GetAllNotes(workOrderID, ql.AttachedContentNotes, ql.Id);
                                    break;
                                case "Service":
                                    ServiceWOMethods sm = new ServiceWOMethods(salesForceProjectID);
                                    sm.GetAllWorkShopInstructions(workOrderID, ql.Id);
                                    sm.GetAllServicerInstructions(workOrderID, ql.Id);
                                    sm.GetAllCheckLists(workOrderID, ql.Id);
                                    sm.GetAllNotes(workOrderID, ql.AttachedContentNotes, ql.Id);
                                    break;
                                case "Site Check":
                                    SiteCheckWOMethods scm = new SiteCheckWOMethods(salesForceProjectID);
                                    scm.GetAllInspectorInstructions(workOrderID, ql.Id);
                                    scm.GetAllCheckLists(workOrderID, ql.Id);
                                    scm.GetAllNotes(workOrderID, ql.AttachedContentNotes, ql.Id);
                                    break;
                                default:
                                    break;
                            }

                            /* check if the work order is approved */
                            HandleApprovalStatus(ql.Id, jobID, estRevID, workOrderID, userEmployeeID, ql.Remarks__c, ql.Due_Date__c, ql.Rush__c, ql.Work_Order_Type__c);

                        }

                    }
                    LogMethods.Log.Debug("GetAllWorkOrders:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllWorkOrders:Error:" + e.Message);
            }
        }

        private void HandleApprovalStatus(string sfWorkOrderID, int jobId, int estRevID, int woId, int userEmployeeID, string remarks, DateTime? dueDate, string rush, string woType )
        {
            try
            {
                var sales_Dispatching = _db.Sales_Dispatching.Where(x => x.JobID == jobId && x.TaskType == 201).FirstOrDefault();
                if (sales_Dispatching == null)
                {
                    //create service client to call API endpoint
                    using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                    {
                        string query = "SELECT Status, LastActor.Name, CompletedDate FROM ProcessInstance where TargetObjectId = '" + sfWorkOrderID + "'" + " order by CompletedDate desc limit 1";

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
                        IEnumerable<enterprise.ProcessInstance> processInstanceList = result.records.Cast<enterprise.ProcessInstance>();

                        //show results
                        foreach (var el in processInstanceList)
                        {
                            if (el.Status == "Approved")
                            {
                                //nothing to do as in production department they will fill some information before approving
                            }
                            else if (el.Status == "Pending")
                            {
                                int taskID = CommonMethods.GetMISID(TableName.Sales_Dispatching, sfWorkOrderID, salesForceProjectID);
                                if (taskID == 0)
                                {
                                    // Make a new approval request on MIS System
                                    int taskCategory = 720; //-- [Work order Approval] is defined in Sales_Dispatching_Task_Category
                                    int submitBy = userEmployeeID;
                                    string taskFromWhere = "wip";  // coresponding to stage of 2

                                    TaskSubmitFactory foTaskSubmit = new TaskSubmitFactory(taskCategory, submitBy, taskFromWhere);
                                    TaskSubmit dp = foTaskSubmit.ObjTaskSubmit;

                                    dp.ParameterDispatchingTask.Responsible = 8; // Mr. Fan is approved it
                                    dp.ParameterDispatchingTask.JobId = jobId;
                                    dp.ParameterDispatchingTask.EstRevId = estRevID;
                                    dp.ParameterDispatchingTask.WoId = woId;

                                    dp.ParameterDispatchingTask.Subject = "Workorder approval";  //DispatchingTaskEN.NDispatchingTaskPurpose.WorkorderApproval
                                    if (!string.IsNullOrEmpty(remarks))
                                    {
                                        dp.ParameterDispatchingTask.Description = remarks.Trim();
                                    }

                                    if (dueDate != null)
                                    {
                                        dp.ParameterDispatchingTask.RequiredTime = (DateTime)dueDate;
                                    }
                                    else
                                    {
                                        dp.ParameterDispatchingTask.RequiredTime = DateTime.Now.AddDays(1);
                                    }

                                    dp.ParameterDispatchingTask.SubmitTime = DateTime.Now;

                                    switch(rush) {
                                        case "Yes":
                                            dp.ParameterDispatchingTask.Rush = true;
                                            break;
                                        default:
                                            dp.ParameterDispatchingTask.Rush = false;
                                            break;
                                    }

                                    switch (woType)
                                    {
                                        case "Production":
                                            dp.ParameterDispatchingTask.WorkorderType = 10;
                                            break;
                                        case "Service":
                                            dp.ParameterDispatchingTask.WorkorderType = 20;
                                            break;
                                        case "Site Check":
                                            dp.ParameterDispatchingTask.WorkorderType = 30;
                                            break;
                                        default:
                                            break;
                                    }

                                    dp.EnableDuplicateSubmit = true;
                                    dp.Insert();

                                    int newTaskId = SqlCommon.GetNewlyInsertedRecordID(TableName.Sales_Dispatching);
                                    CommonMethods.InsertToMISSalesForceMapping(TableName.Sales_Dispatching, sfWorkOrderID, newTaskId.ToString(), salesForceProjectID);
                                }

                            }
                        }

                        LogMethods.Log.Debug("HandleApprovalStatus:Debug:" + "Done");
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("HandleApprovalStatus:Error:" + e.Message);
            }
        }

        private void HandleWorkOrderItem(int workOrderID, string listItemID, string sfWorkOrderID)
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
                    string query = "SELECT Id, Item_Name__c, Requirement__c, Work_Order_Item_Description__c, Item_Cost__c, Quantity__c FROM Item__c where Id in (";
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
                        int itemIDTemp = CommonMethods.GetMISID(TableName.WO_Item, il.Id, sfWorkOrderID, salesForceProjectID);
                        if (itemIDTemp == 0)
                        {
                            WokrorderItemGenerateFromBlank woItem = new WokrorderItemGenerateFromBlank(workOrderID);
                            // no error
                            if (woItem.ValidationID == 0)
                            {
                                woItem.InsertItem();
                                int newWOItemID = woItem.NewWorkItemID;
                                CommonMethods.InsertToMISSalesForceMapping(TableName.WO_Item, il.Id, newWOItemID.ToString(), sfWorkOrderID, salesForceProjectID);
                                itemIDTemp = newWOItemID;
                            }
                        }

                        if (itemIDTemp != 0)
                        {
                            UpdateWorkOrderItem(il.Id, itemIDTemp, il.Item_Name__c, il.Requirement__c, il.Work_Order_Item_Description__c, il.Item_Cost__c, il.Quantity__c);
                        }
                    }

                    /* delete work order items which has been removed out of work order */
                    DeleteAllDeletedWorkOrderItems(items, sfWorkOrderID);

                    LogMethods.Log.Debug("HandleWorkOrderItem:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("HandleWorkOrderItem:Error:" + e.Message);
            }
        }

        private void DeleteAllDeletedWorkOrderItems(string[] items, string sfWorkOrderID)
        {
            try
            {
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.WO_Item, sfWorkOrderID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(items, i) == -1)
                    {
                        // get MISID
                        int itemIDTemp = CommonMethods.GetMISID(TableName.WO_Item, i, sfWorkOrderID, salesForceProjectID);
                        // get a row
                        var workOrderItem = _db.WO_Item.Where(x => x.woItemID == itemIDTemp).FirstOrDefault();
                        if (workOrderItem != null)
                        {
                            _db.WO_Item.Remove(workOrderItem);
                            _db.SaveChanges();
                        }
                        // remove MISID out of MISSalesForceMapping
                        CommonMethods.Delete(TableName.WO_Item, i, sfWorkOrderID, salesForceProjectID);
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedWorkOrderItems:Error:" + e.Message);
            }
        }

        private void UpdateWorkOrderItem(string salesforceItemID, long workOrderItemID, string itemName, string requirement, string description, double? itemCost, double? quality)
        {
            try
            {
                var workOrderItem = _db.WO_Item.Where(x => x.woItemID == workOrderItemID).FirstOrDefault();
                if (workOrderItem != null)
                {
                    workOrderItem.estItemNameText = itemName;

                    int requirementID = 10;
                    var jobType = _db.FW_JOB_TYPE.Where(x => x.JOB_TYPE.Trim() == requirement.Trim()).FirstOrDefault();
                    if (jobType != null)
                    {
                        requirementID = jobType.QUOTE_SUPPLY_TYPE;
                    }
                    else
                    {
                        LogMethods.Log.Error("UpdateWorkOrderItem:Debug:" + "Requirement of " + requirement + " doesn't exist on FW_JOB_TYPE table.");
                    }
                    workOrderItem.Requirement = requirementID;
                    workOrderItem.woDescription = description;
                    if (quality != null)
                        workOrderItem.qty = Convert.ToInt16(quality);

                    if (itemCost != null)
                    {
                        workOrderItem.qiAmount = (double)itemCost;
                    }

                    long estItemID = CommonMethods.GetMISID(TableName.EST_Item, salesforceItemID, salesForceProjectID);
                    if (estItemID != 0)
                    {
                        workOrderItem.estItemID = estItemID;
                    }

                    _db.Entry(workOrderItem).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("UpdateWorkOrderItem:Error:" + e.Message);
            }
        }

        private void UpdateWorkOrder(int workOrderID, string woNumber, string woType, string paymentMethod, double? version, string rush, string rushReason,
                        string remarks, DateTime? issueDate, DateTime? dueDate, string cloneType, string preWONumber, string siteCheckPurpose, string siteCheckPurposeAsOther, string sfWorkOrderID)
        {
            try
            {
                var workOrder = _db.Sales_JobMasterList_WO.Where(x => x.woID == workOrderID).FirstOrDefault();
                if (workOrder != null)
                {
                    switch (woType)
                    {
                        case "Production":
                            workOrder.woType = 10;
                            break;
                        case "Service":
                            workOrder.woType = 20;
                            break;
                        case "Site Check":
                            workOrder.woType = 30;
                            break;
                        default:
                            break;
                    }

                    switch (paymentMethod)
                    {
                        case "C.O.D (Invoice Attached)":
                            workOrder.PayMethods = 10;
                            break;
                        case "Invoice Mail Out By Office":
                            workOrder.PayMethods = 20;
                            break;
                        case "Installers Give Invoice To Client":
                            workOrder.PayMethods = 30;
                            break;
                        case "No Charge-Other":
                            workOrder.PayMethods = 40;
                            break;
                        case "No Charge-Mistakes":
                            workOrder.PayMethods = 41;
                            break;
                        case "No Charge-Under Warranty":
                            workOrder.PayMethods = 42;
                            break;
                        case "No Charge-Company Give Out As Gift":
                            workOrder.PayMethods = 43;
                            break;
                        case "No Charge-Sample":
                            workOrder.PayMethods = 44;
                            break;
                        case "No Charge-For Company Internal Use":
                            workOrder.PayMethods = 45;
                            break;
                        case "Decide By Installer":
                            workOrder.PayMethods = 46;
                            break;
                        case "PJ":
                            workOrder.PayMethods = 50;
                            break;
                        default:
                            break;
                    }

                    if (version != null)
                    {
                        workOrder.woRev = Convert.ToByte(version);
                    }

                    switch (rush)
                    {
                        case "Yes":
                            workOrder.rush = true;
                            workOrder.rushReason = rushReason;
                            break;
                        case "No":
                            workOrder.rush = false;
                            break;
                        default:
                            break;
                    }

                    workOrder.Remarks = remarks;

                    if (issueDate != null)
                    {
                        workOrder.issuedDate = (DateTime)issueDate;
                    }
                    if (dueDate != null)
                    {
                        workOrder.DeadLine = (DateTime)dueDate;
                    }

                    switch (cloneType)
                    {
                        case "Redo":
                            workOrder.rush = true;
                            workOrder.reDo = true;
                            workOrder.revise = false;
                            workOrder.reviseVer = null;
                            workOrder.RedoOfWoNumbers = preWONumber;
                            workOrder.WorkorderNumber = woNumber;
                            if (version != null)
                            {
                                workOrder.redoVer = Convert.ToInt16(version);
                            }
                            break;
                        case "Revise":
                            workOrder.rush = true;
                            workOrder.revise = true;
                            workOrder.reDo = false;
                            workOrder.redoVer = null;
                            workOrder.RedoOfWoNumbers = preWONumber;
                            workOrder.WorkorderNumber = preWONumber;
                            if (version != null)
                            {
                                workOrder.reviseVer = Convert.ToInt16(version);
                            }
                            break;
                        case "New":
                            workOrder.revise = false;
                            workOrder.reviseVer = null;
                            workOrder.reDo = false;
                            workOrder.redoVer = null;
                            workOrder.RedoOfWoNumbers = "";
                            workOrder.WorkorderNumber = woNumber;
                            break;
                        default:
                            break;
                    }

                    _db.Entry(workOrder).State = EntityState.Modified;
                    _db.SaveChanges();

                }

                if (woType == "Site Check")
                {
                    int siteCheckID = CommonMethods.GetMISID(TableName.WO_Sitecheck_Purpose, sfWorkOrderID, salesForceProjectID);
                    if (siteCheckID == 0)
                    {
                        InsertNewSiteCheckPurpose(workOrderID, siteCheckPurpose, siteCheckPurposeAsOther);
                        int newId = SqlCommon.GetNewlyInsertedRecordID(TableName.WO_Sitecheck_Purpose);
                        CommonMethods.InsertToMISSalesForceMapping(TableName.WO_Sitecheck_Purpose, sfWorkOrderID, newId.ToString(), salesForceProjectID);
                    }
                    else
                    {
                        UpdateSiteCheckPurpose(siteCheckID, siteCheckPurpose, siteCheckPurposeAsOther);
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("UpdateWorkOrder:Error:" + e.Message);
            }

        }

        private bool UpdateSiteCheckPurpose(int siteCheckID, string siteCheckPurpose, string siteCheckPurposeAsOther)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlUpdateString = "UPDATE [WO_Sitecheck_Purpose] SET [scPurpose1] = @scPurpose1, [scPurpose2] = @scPurpose2, [scPurpose3] = @scPurpose3, [scPurpose4] = @scPurpose4, [scPurposeOther] = @scPurposeOther WHERE [scID] = @scID";
                var UpdateCommand = new SqlCommand(SqlUpdateString, Connection);
                UpdateCommand.Parameters.AddWithValue("@scID", siteCheckID);
                switch (siteCheckPurpose)
                {
                    case "Quotation":
                        UpdateCommand.Parameters.Add("@scPurpose1", SqlDbType.Bit).Value = true;
                        UpdateCommand.Parameters.Add("@scPurpose2", SqlDbType.Bit).Value = false;
                        UpdateCommand.Parameters.Add("@scPurpose3", SqlDbType.Bit).Value = false;
                        UpdateCommand.Parameters.Add("@scPurpose4", SqlDbType.Bit).Value = false;
                        UpdateCommand.Parameters.Add("@scPurposeOther", SqlDbType.NVarChar, 500).Value = "";
                        break;
                    case "Permit":
                        UpdateCommand.Parameters.Add("@scPurpose1", SqlDbType.Bit).Value = false;
                        UpdateCommand.Parameters.Add("@scPurpose2", SqlDbType.Bit).Value = true;
                        UpdateCommand.Parameters.Add("@scPurpose3", SqlDbType.Bit).Value = false;
                        UpdateCommand.Parameters.Add("@scPurpose4", SqlDbType.Bit).Value = false;
                        UpdateCommand.Parameters.Add("@scPurposeOther", SqlDbType.NVarChar, 500).Value = "";
                        break;
                    case "Production":
                        UpdateCommand.Parameters.Add("@scPurpose1", SqlDbType.Bit).Value = false;
                        UpdateCommand.Parameters.Add("@scPurpose2", SqlDbType.Bit).Value = false;
                        UpdateCommand.Parameters.Add("@scPurpose3", SqlDbType.Bit).Value = true;
                        UpdateCommand.Parameters.Add("@scPurpose4", SqlDbType.Bit).Value = false;
                        UpdateCommand.Parameters.Add("@scPurposeOther", SqlDbType.NVarChar, 500).Value = "";
                        break;
                    default:
                        if (siteCheckPurposeAsOther != null)
                        {
                            UpdateCommand.Parameters.Add("@scPurposeOther", SqlDbType.NVarChar, 500).Value = siteCheckPurposeAsOther;
                        }
                        else
                        {
                            UpdateCommand.Parameters.Add("@scPurposeOther", SqlDbType.NVarChar, 500).Value = "";
                        }
                        UpdateCommand.Parameters.Add("@scPurpose1", SqlDbType.Bit).Value = false;
                        UpdateCommand.Parameters.Add("@scPurpose2", SqlDbType.Bit).Value = false;
                        UpdateCommand.Parameters.Add("@scPurpose3", SqlDbType.Bit).Value = false;
                        UpdateCommand.Parameters.Add("@scPurpose4", SqlDbType.Bit).Value = true;
                        break;
                }
                Connection.Open();
                UpdateCommand.ExecuteNonQuery();
                ret = true;
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("UpdateSiteCheckPurpose:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

        private void InsertNewSiteCheckPurpose(int woId, string siteCheckPurpose, string siteCheckPurposeAsOther)
        {
            using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
            {
                string InsertString =
                    "INSERT INTO [WO_Sitecheck_Purpose] ([woID], [scPurpose1], [scPurpose2], [scPurpose3], [scPurpose4], [scPurposeOther]) VALUES (@woID, @scPurpose1, @scPurpose2, @scPurpose3, @scPurpose4, @scPurposeOther)";

                // Create the command and set its properties.
                var InsertCommand = new SqlCommand(InsertString, Connection);
                try
                {
                    Connection.Open();
                    InsertCommand.Parameters.Add("@woID", SqlDbType.Int).Value = woId;
                    switch (siteCheckPurpose)
                    {
                        case "Quotation":
                            InsertCommand.Parameters.Add("@scPurpose1", SqlDbType.Bit).Value = true;
                            InsertCommand.Parameters.Add("@scPurpose2", SqlDbType.Bit).Value = false;
                            InsertCommand.Parameters.Add("@scPurpose3", SqlDbType.Bit).Value = false;
                            InsertCommand.Parameters.Add("@scPurpose4", SqlDbType.Bit).Value = false;
                            InsertCommand.Parameters.Add("@scPurposeOther", SqlDbType.NVarChar, 500).Value = "";
                            break;
                        case "Permit":
                            InsertCommand.Parameters.Add("@scPurpose2", SqlDbType.Bit).Value = true;
                            InsertCommand.Parameters.Add("@scPurpose1", SqlDbType.Bit).Value = false;
                            InsertCommand.Parameters.Add("@scPurpose3", SqlDbType.Bit).Value = false;
                            InsertCommand.Parameters.Add("@scPurpose4", SqlDbType.Bit).Value = false;
                            InsertCommand.Parameters.Add("@scPurposeOther", SqlDbType.NVarChar, 500).Value = "";
                            break;
                        case "Production":
                            InsertCommand.Parameters.Add("@scPurpose3", SqlDbType.Bit).Value = true;
                            InsertCommand.Parameters.Add("@scPurpose2", SqlDbType.Bit).Value = false;
                            InsertCommand.Parameters.Add("@scPurpose1", SqlDbType.Bit).Value = false;
                            InsertCommand.Parameters.Add("@scPurpose4", SqlDbType.Bit).Value = false;
                            InsertCommand.Parameters.Add("@scPurposeOther", SqlDbType.NVarChar, 500).Value = "";
                            break;
                        default:
                            InsertCommand.Parameters.Add("@scPurposeOther", SqlDbType.NVarChar, 500).Value = siteCheckPurposeAsOther;
                            InsertCommand.Parameters.Add("@scPurpose4", SqlDbType.Bit).Value = true;
                            InsertCommand.Parameters.Add("@scPurpose2", SqlDbType.Bit).Value = false;
                            InsertCommand.Parameters.Add("@scPurpose1", SqlDbType.Bit).Value = false;
                            InsertCommand.Parameters.Add("@scPurpose3", SqlDbType.Bit).Value = false;
                            break;
                    }
                    InsertCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    LogMethods.Log.Error("InsertNewSiteCheckPurpose:Error:" + ex.Message);
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

    }
}
