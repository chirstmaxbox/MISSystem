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
                    string query = "SELECT Id, Name, (select Id, Title, TextPreview from AttachedContentNotes), RecordType.Name, Work_Order_Type__c, Payment_Method__c, Version__c, Rush__c, Rush_Reason__c, Remarks__c, "
                        + " Issue_Date__c, Due_Date__c, Clone_Type__c, Previous_Work_Order_Number__r.Name, Site_Check_Purpose__c, Site_Check_Purpose_As_Other__c, Amount__c, Previous_Work_Order_Number__r.Clone_Type__c, Previous_Work_Order_Number__r.Version__c,"
                        + " (SELECT Status, LastActor.Name, CompletedDate FROM ProcessInstances order by CompletedDate desc limit 1),"
                        + " (SELECT Id, Item_Name__c, Item_Order__c, Requirement__c, Item_Description__c, Item_Cost__c, Quantity__c, Item_Link__c FROM Items__r),"
                        + " (SELECT Id, Category__c, Final_Instruction__c, Instruction__c FROM WorkShop_Instructions__r),"
                        + " (SELECT Id, Category__c, Final_Instruction__c, Instruction__c FROM Installer_Instructions__r),"
                        + " (SELECT Id, Check_List_Item__c, Content__c, Content_For_Check_List_Item_As_Others__c FROM Production_Check_List__r),"
                        + " (SELECT Id, Category__c, Final_Instruction__c, Instruction__c FROM Servicer_Instructions__r),"
                        + " (SELECT Id, Check_List_Item__c, Content__c, Content_For_Check_List_Item_As_Others__c FROM Service_Check_Lists__r)"
                        + " FROM Work_Order__c "
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
                            if (workOrderID > 0)
                            {
                                CommonMethods.InsertToMISSalesForceMapping(TableName.Sales_JobMasterList_WO, ql.Id, workOrderID.ToString(), salesForceProjectID);
                            }
                        }

                        if (workOrderID != 0)
                        {
                            UpdateWorkOrder(workOrderID, ql.Name, ql.RecordType.Name, ql.Payment_Method__c, ql.Version__c, ql.Rush__c, ql.Rush_Reason__c,
                                ql.Remarks__c, ql.Issue_Date__c, ql.Due_Date__c, ql.Clone_Type__c, ql.Previous_Work_Order_Number__r, ql.Site_Check_Purpose__c, ql.Site_Check_Purpose_As_Other__c, ql.Id, ql.Amount__c);

                            // generate work order items
                            HandleWorkOrderItem(workOrderID, estRevID, ql.Id, ql.Items__r);

                            switch (ql.RecordType.Name)
                            {
                                case "Production":
                                    ProductionWOMethods pm = new ProductionWOMethods(salesForceProjectID);
                                    pm.GetAllWorkShopInstructions(workOrderID, ql.Id, ql.WorkShop_Instructions__r);
                                    pm.GetAllInstallerInstructions(workOrderID, ql.Id, ql.Installer_Instructions__r);
                                    pm.GetAllCheckLists(workOrderID, ql.Id, ql.Production_Check_List__r);
                                    pm.GetAllNotes(workOrderID, ql.AttachedContentNotes, ql.Id);
                                    break;
                                case "Service":
                                    ServiceWOMethods sm = new ServiceWOMethods(salesForceProjectID);
                                    sm.GetAllWorkShopInstructions(workOrderID, ql.Id, ql.WorkShop_Instructions__r);
                                    sm.GetAllServicerInstructions(workOrderID, ql.Id, ql.Servicer_Instructions__r);
                                    sm.GetAllCheckLists(workOrderID, ql.Id, ql.Service_Check_Lists__r);
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
                            HandleApprovalStatus(ql.Id, jobID, estRevID, workOrderID, userEmployeeID, ql.Remarks__c, ql.Due_Date__c, ql.Rush__c, ql.RecordType.Name, ql.ProcessInstances, ql.Version__c);

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

        private void HandleApprovalStatus(string sfWorkOrderID, int jobId, int estRevID, int woId, int userEmployeeID, string remarks, DateTime? dueDate, string rush, string woType, enterprise.QueryResult result, double? version)
        {
            try
            {
                if (version == null) return;
                short ver = Convert.ToInt16(version);

                var sales_Dispatching = _db.Sales_Dispatching.Where(x => x.JobID == jobId && x.TaskType == 720 && x.WoID == woId && x.Importance == ver).FirstOrDefault();
                if (sales_Dispatching == null)
                {
                    //create service client to call API endpoint
                    using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                    {
                        if (result == null || (result != null && result.size == 0)) return;

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

                                switch (rush)
                                {
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
                                dp.Insert(ver);
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

        private void HandleWorkOrderItem(int workOrderID, int estRevID, string sfWorkOrderID, enterprise.QueryResult result)
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
                        int itemIDTemp = CommonMethods.GetMISID(TableName.WO_Item, il.Id, sfWorkOrderID, salesForceProjectID);
                        if (itemIDTemp == 0)
                        {
                            WokrorderItemGenerateFromBlank woItem = new WokrorderItemGenerateFromBlank(workOrderID);
                            // no error
                            if (woItem.ValidationID == 0)
                            {
                                woItem.InsertItem();
                                int newWOItemID = woItem.NewWorkItemID;
                                if (newWOItemID > 0)
                                {
                                    CommonMethods.InsertToMISSalesForceMapping(TableName.WO_Item, il.Id, newWOItemID.ToString(), sfWorkOrderID, salesForceProjectID);
                                }
                                itemIDTemp = newWOItemID;
                            }
                        }

                        if (itemIDTemp != 0)
                        {
                            UpdateWorkOrderItem(estRevID, il.Id, itemIDTemp, il.Item_Name__c, il.Requirement__c, il.Item_Description__c, il.Item_Cost__c, il.Quantity__c, il.Item_Order__c, il.Item_Link__c);
                        }
                    }

                    /* delete work order items which has been removed out of work order */
                    DeleteAllDeletedWorkOrderItems(items.ToArray(), sfWorkOrderID);

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

        private void UpdateWorkOrderItem(int estRevID, string salesforceItemID, long workOrderItemID, string itemName, string requirement, string description, double? itemCost, double? quality, double? itemOrder, string itemLink)
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
                        requirementID = jobType.TYPE_ID;
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

                    long estItemID = CommonMethods.GetEstimationItemID(estRevID, itemName);
                    if (estItemID != 0)
                    {
                        workOrderItem.estItemID = estItemID;
                    }

                    if (itemOrder != null)
                    {
                        workOrderItem.woPrintOrder = Convert.ToInt16(itemOrder);
                    }

                    _db.Entry(workOrderItem).State = EntityState.Modified;
                    _db.SaveChanges();


                    int drawingID = GetItemLink(workOrderItemID);
                    if (drawingID == 0)
                    {
                        // insert
                        if (!string.IsNullOrEmpty(itemLink))
                        {
                            InsertItemLink(workOrderItemID, itemLink);
                        }
                    }
                    else
                    {
                        //update
                        UpdateItemLink(drawingID, itemLink);
                    }

                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("UpdateWorkOrderItem:Error:" + e.Message);
            }
        }

        private void UpdateItemLink(long drawingID, string itemLink)
        {
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlUpdateString = "UPDATE WO_Item_Drawing SET DrawingHyperlink = @DrawingHyperlink WHERE (DrawingID = @DrawingID)";
                var UpdateCommand = new SqlCommand(SqlUpdateString, Connection);
                UpdateCommand.Parameters.AddWithValue("@DrawingID", drawingID);
                if (itemLink != null)
                {
                    UpdateCommand.Parameters.AddWithValue("@DrawingHyperlink", itemLink);
                }
                else
                {
                    UpdateCommand.Parameters.AddWithValue("@DrawingHyperlink", "");
                }
                Connection.Open();
                UpdateCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("UpdateItemLink:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }
        }

        private void InsertItemLink(long workOrderItemID, string itemLink)
        {
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlSelectString = "INSERT INTO [WO_Item_Drawing] ([ParentID], [DrawingType], [DrawingPurpose], [DrawingName], [DrawingHyperlink], [IsFinalDrawing], [Note]) VALUES (@ParentID, @DrawingType, @DrawingPurpose, @DrawingName, @DrawingHyperlink, @IsFinalDrawing, @Note)";
                var InsertCommand = new SqlCommand(SqlSelectString, Connection);
                InsertCommand.Parameters.AddWithValue("@ParentID", workOrderItemID);
                InsertCommand.Parameters.AddWithValue("@DrawingType", "Customer File");
                InsertCommand.Parameters.AddWithValue("@DrawingPurpose", 1);
                InsertCommand.Parameters.AddWithValue("@DrawingName", "");
                InsertCommand.Parameters.AddWithValue("@DrawingHyperlink", itemLink);
                InsertCommand.Parameters.AddWithValue("@IsFinalDrawing", 0);
                InsertCommand.Parameters.AddWithValue("@Note", "");
                Connection.Open();
                InsertCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("InsertItemLink:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }
        }

        private int GetItemLink(long workOrderItemID)
        {
            /* Check if item link is being existed */
            int drawingID = 0;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlSelectString = "SELECT DrawingID FROM [WO_Item_Drawing] WHERE ([ParentID] = @ParentID)";
                var SelectCommand = new SqlCommand(SqlSelectString, Connection);
                SelectCommand.Parameters.AddWithValue("@ParentID", workOrderItemID);
                Connection.Open();
                using (SqlDataReader dr = SelectCommand.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        drawingID = Convert.ToInt32(dr[0].ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("GetItemLink:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return drawingID;
        }

        private void UpdateWorkOrder(int workOrderID, string woNumber, string woType, string paymentMethod, double? version, string rush, string rushReason,
                        string remarks, DateTime? issueDate, DateTime? dueDate, string cloneType, enterprise.Work_Order__c preWONumber, string siteCheckPurpose, string siteCheckPurposeAsOther, string sfWorkOrderID, double? amount)
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

                    if (amount != null)
                    {
                        workOrder.WorkorderAmount = Convert.ToDouble(amount);
                    }

                    switch (cloneType)
                    {
                        case "Redo":
                            workOrder.rush = true;
                            workOrder.reDo = true;

                            if (preWONumber != null)
                            {
                                if (preWONumber.Clone_Type__c != "New" && preWONumber.Clone_Type__c != cloneType)
                                {
                                    workOrder.revise = true;
                                    workOrder.reviseVer = preWONumber.Version__c != null ? Convert.ToInt16(preWONumber.Version__c) : (short)1;
                                }
                                else
                                {
                                    workOrder.revise = false;
                                    workOrder.reviseVer = null;
                                }
                            }
                            else
                            {
                                workOrder.revise = false;
                                workOrder.reviseVer = null;
                            }
                            
                            workOrder.RedoOfWoNumbers = preWONumber != null ? preWONumber.Name : "";
                            workOrder.WorkorderNumber = woNumber;
                            if (version != null)
                            {
                                workOrder.redoVer = Convert.ToInt16(version);
                            }
                            break;
                        case "Revise":
                            workOrder.rush = true;
                            workOrder.revise = true;

                            if (preWONumber != null)
                            {
                                if (preWONumber.Clone_Type__c != "New" && preWONumber.Clone_Type__c != cloneType)
                                {
                                    workOrder.reDo = true;
                                    workOrder.redoVer = preWONumber.Version__c != null ? Convert.ToInt16(preWONumber.Version__c) : (short)1;
                                }
                                else
                                {
                                    workOrder.reDo = false;
                                    workOrder.redoVer = null;
                                }
                            }
                            else
                            {
                                workOrder.reDo = false;
                                workOrder.redoVer = null;
                            }

                            workOrder.RedoOfWoNumbers = preWONumber != null ? preWONumber.Name : "";
                            workOrder.WorkorderNumber = preWONumber != null ? preWONumber.Name : "";
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
                        if (newId > 0)
                        {
                            CommonMethods.InsertToMISSalesForceMapping(TableName.WO_Sitecheck_Purpose, sfWorkOrderID, newId.ToString(), salesForceProjectID);
                        }
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
