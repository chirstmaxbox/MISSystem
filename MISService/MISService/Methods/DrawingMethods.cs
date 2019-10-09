using MISService.Method;
using MISService.Models;
using MyCommon;
using SpecDomain.BLL.Task;
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
    public class DrawingMethods
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        private EndpointAddress apiAddr;
        private enterprise.SessionHeader header;
        private string salesForceProjectID;

        public DrawingMethods(string salesForceProjectID)
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
            this.salesForceProjectID = salesForceProjectID;
        }

        public void GetAllDrawings(string sfProjectID, int estRevID, int jobID, int employeeNumber)
        {
            try
            {
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Name, Version__c, Drawing_Requisition_Type__c, Drawing_Purpose__c, Is_Electronic_File_From_Client_Available__c, "
                                    + " Is_GC_Or_Designer_Drawing_Available__c, Is_Landord_Or_Mall_Criteria_Available__c, Is_Latest_Version_Q_D_Quotation_Avail__c, "
                                    + " Is_Site_Check_Photo_Available__c, Is_Site_Check_Report_Available__c, LastModifiedDate, Drawing_Hour__c, Number_Of_Drawings__c, Target_Date__c, Due_Time__c, Issue_Date_Time__c, "
                                    + " (SELECT Id, Item_Name__c, Item_Description__c, Quantity__c FROM Items__r),"
                                    + " (SELECT Status, LastActor.Name, CompletedDate FROM ProcessInstances order by CompletedDate desc limit 1)"
                                    + " FROM Drawing__c "
                                    + " WHERE Project_Name__c = '" + sfProjectID + "'" + " order by LastModifiedDate desc";

                    enterprise.QueryResult result;
                    queryClient.query(
                        header, //sessionheader
                        null, //queryoptions
                        null, //mruheader
                        null, //packageversion
                        query, out result);

                    /* if no any record, return */
                    if (result.size == 0) return;

                    IEnumerable<enterprise.Drawing__c> drawingList = result.records.Cast<enterprise.Drawing__c>();
                    /* in MIS, only one drawing */
                    int requisitionId = 0;
                    bool flag = false;
                    foreach (var dl in drawingList)
                    {
                        if (!flag)
                        {
                            /* one unique row will be inserted if it is not existent */
                            var vm = new DrawingRequisitionFormVm(jobID, estRevID);
                            vm.Initialization();

                            /* update data */
                            requisitionId = UpdateDrawing(estRevID, dl.Version__c, dl.Drawing_Requisition_Type__c, dl.Drawing_Purpose__c, dl.Is_Electronic_File_From_Client_Available__c,
                                dl.Is_GC_Or_Designer_Drawing_Available__c, dl.Is_Landord_Or_Mall_Criteria_Available__c, dl.Is_Latest_Version_Q_D_Quotation_Avail__c,
                                dl.Is_Site_Check_Photo_Available__c, dl.Is_Site_Check_Report_Available__c);

                            if (requisitionId != 0)
                            {
                                /* Salesforce can create multi-drawing request but the MIS only supports one at a time so 
                                 I will only show the latest one on the MIS system*/
                                // update SalesForceParentID in MISSalesForceMapping 
                                UpdateMISSalesForceMapping(TableName.Sales_Dispatching_DrawingRequisition_EstimationItem, dl.Id);

                                // add items to drawing
                                GetAllDrawingItems(sfProjectID, requisitionId, estRevID, dl.Id, dl.Items__r);

                                GetDrawingApprovalData(jobID, dl.ProcessInstances, dl.Version__c, employeeNumber, dl.Drawing_Hour__c, dl.Target_Date__c, requisitionId, dl.Drawing_Requisition_Type__c, dl.Id, dl.Number_Of_Drawings__c, dl.Due_Time__c, dl.Issue_Date_Time__c);
                            }
                            flag = true;
                        }
                        else
                        {
                            GetDrawingApprovalData(jobID, dl.ProcessInstances, dl.Version__c, employeeNumber, dl.Drawing_Hour__c, dl.Target_Date__c, requisitionId, dl.Drawing_Requisition_Type__c, dl.Id, dl.Number_Of_Drawings__c, dl.Due_Time__c, dl.Issue_Date_Time__c);
                        }
                    }

                }
                
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllDrawing:Error:" + e.Message);
            }
        }

        private void GetDrawingApprovalData( int jobId, enterprise.QueryResult result, double? version, int employeeNumber, double? drawHour, DateTime? dueDate, int requisitionId, string drawingType, string sfDrawingID, double? NumofDrawings, DateTime? dueTime, DateTime? issueDateTime)
        {
            try
            {
                if (version == null || result == null || (result != null && result.size == 0)) return;

                //cast query results
                IEnumerable<enterprise.ProcessInstance> processInstanceList = result.records.Cast<enterprise.ProcessInstance>();

                foreach (var el in processInstanceList)
                {
                    int taskType = 501;
                    short taskStatus = 549;
                    if (drawingType.Trim() == "Structure")
                    {
                        taskType = 551;
                        taskStatus = 599;
                    }

                    if (el.Status == "Pending")
                    {
                        int drawingID = CommonMethods.GetMISID(TableName.Sales_Dispatching, sfDrawingID, salesForceProjectID);
                        if (drawingID == 0)
                        {
                            //not exist
                            var vm = new SubmitDrawingRequestVm(requisitionId, employeeNumber);
                            if (dueDate != null)
                            {
                                if (dueTime != null)
                                {
                                    TimeZone localZone = TimeZone.CurrentTimeZone;
                                    DateTime currentUTC = localZone.ToUniversalTime(dueTime.Value);
                                    DateTime localTime = new DateTime(dueDate.Value.Year, dueDate.Value.Month, dueDate.Value.Day, currentUTC.Hour, currentUTC.Minute, 00);
                                    if (localZone.IsDaylightSavingTime(localTime))
                                    {
                                        localTime = localTime.AddHours(-1);
                                    }
                                    vm.FormatedRequiredTime = localTime.ToString("MMM dd, yyyy  hh:mm tt");
                                }
                                else
                                {
                                    vm.FormatedRequiredTime = new DateTime(dueDate.Value.Year, dueDate.Value.Month, dueDate.Value.Day, dueDate.Value.Hour, dueDate.Value.Minute, 00).ToString("MMM dd, yyyy  hh:mm tt");
                                }
                            }
                            else
                            {
                                DateTime dt1 = MyDateTime.GetDateOfAddedBusinessDays(DateTime.Today, 2);
                                DateTime dt2 = DateTime.Now.AddMinutes(2);
                                vm.FormatedRequiredTime =
                                    new DateTime(dt1.Year, dt1.Month, dt1.Day, dt2.Hour, dt2.Minute, 00).ToString("MMM dd, yyyy  hh:mm tt");
                            }
                            long taskID;
                            if (issueDateTime == null)
                            {
                                taskID = vm.Create();
                            }
                            else
                            {
                                taskID = vm.Create(issueDateTime.Value.ToLocalTime());
                            }
                            if (taskID > 0)
                            {
                                CommonMethods.InsertToMISSalesForceMapping(TableName.Sales_Dispatching, sfDrawingID, taskID.ToString(), salesForceProjectID);
                            }
                        }

                        /*
                        var sales_Dispatching = _db.Sales_Dispatching.Where(x => x.JobID == jobId && x.TaskType == taskType && x.Importance == version).FirstOrDefault();
                        if (sales_Dispatching == null)
                        {
                            var vm = new SubmitDrawingRequestVm(requisitionId, employeeNumber);
                            if (dueDate != null)
                            {
                                vm.FormatedRequiredTime = new DateTime(dueDate.Value.Year, dueDate.Value.Month, dueDate.Value.Day, dueDate.Value.Hour, dueDate.Value.Minute, 00).ToString("MMM dd, yyyy  hh:mm tt");
                            }
                            else
                            {
                                DateTime dt1 = MyDateTime.GetDateOfAddedBusinessDays(DateTime.Today, 2);
                                DateTime dt2 = DateTime.Now.AddMinutes(2);
                                vm.FormatedRequiredTime =
                                    new DateTime(dt1.Year, dt1.Month, dt1.Day, dt2.Hour, dt2.Minute, 00).ToString("MMM dd, yyyy  hh:mm tt");
                            }
                            vm.Create();
                        }
                         * */
                    }
                    else if (el.Status == "Approved")
                    {
                        int drawingID = CommonMethods.GetMISID(TableName.Sales_Dispatching, sfDrawingID, salesForceProjectID);
                        if (drawingID > 0)
                        {
                            var sales_Dispatching = _db.Sales_Dispatching.Where(x => x.TaskID == drawingID).FirstOrDefault();
                            if (sales_Dispatching != null)
                            {
                                if (drawHour != null)
                                {
                                    sales_Dispatching.WorkedHour = drawHour;
                                }

                                if (NumofDrawings != null)
                                {
                                    sales_Dispatching.NumberOfDrawing = Convert.ToInt32(NumofDrawings);
                                }

                                sales_Dispatching.Status = taskStatus;
                                if (el.CompletedDate != null)
                                {
                                    sales_Dispatching.FinishedDate = el.CompletedDate.Value.ToLocalTime();
                                }

                                _db.Entry(sales_Dispatching).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }
                    }
                    else if (el.Status == "Removed")
                    {
                        int drawingID = CommonMethods.GetMISID(TableName.Sales_Dispatching, sfDrawingID, salesForceProjectID);
                        if (drawingID > 0)
                        {
                            var sales_Dispatching = _db.Sales_Dispatching.Where(x => x.TaskID == drawingID).FirstOrDefault();
                            if (sales_Dispatching != null)
                            {
                                /* delete a row in Sales_Dispatching */
                                _db.Sales_Dispatching.Remove(sales_Dispatching);
                                _db.SaveChanges();
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetDrawingApprovalData:Error:" + e.Message);
            }
        }

        private void UpdateMISSalesForceMapping(string tableName, string salesforceParentID)
        {
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                Connection.Open();
                string SqlDelString = "UPDATE MISSalesForceMapping SET [SalesforceParentID] = @salesforceParentID WHERE ([SalesForceProjectID] = @salesForceProjectID) AND ([TableName] = @tableName)";
                var UpdateCommand = new SqlCommand(SqlDelString, Connection);
                UpdateCommand.Parameters.AddWithValue("@tableName", tableName);
                UpdateCommand.Parameters.AddWithValue("@salesforceParentID", salesforceParentID);
                UpdateCommand.Parameters.AddWithValue("@salesForceProjectID", salesForceProjectID);
                UpdateCommand.ExecuteNonQuery();
                Connection.Close();
                LogMethods.Log.Debug("UpdateMISSalesForceMapping:Debug:" + "Done");
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("UpdateMISSalesForceMapping:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }
        }

        private void GetAllDrawingItems(string sfProjectID, int drawingRequisitionID, int estRevID, string sfDrawingID, enterprise.QueryResult result)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    if (result == null || (result != null && result.size == 0)) return;

                    //cast query results
                    IEnumerable<enterprise.Item__c> itemList = result.records.Cast<enterprise.Item__c>();

                    //show results
                    var results = new List<MyLongKeyValueBool>();
                    List<string> items = new List<string>();
                    foreach (var il in itemList)
                    {
                        items.Add(il.Id);
                        int itemIDTemp = CommonMethods.GetMISID(TableName.Sales_Dispatching_DrawingRequisition_EstimationItem, il.Id, sfDrawingID, salesForceProjectID);
                        if (itemIDTemp == 0)
                        {
                            /* get item ID from EST_ITEM table */
                            int itemID = CommonMethods.GetEstimationItemID(estRevID, il.Item_Name__c);
                            if (itemID != 0)
                            {
                                var r = new MyLongKeyValueBool();
                                r.Key = itemID;
                                r.IsChecked = true;
                                r.Value1 = il.Id;
                                results.Add(r);
                            }
                        }
                        else
                        {
                            UpdateDrawingItem(itemIDTemp, il.Quantity__c, il.Item_Description__c);
                        }
                    }

                    if (results.Any())
                    {
                        var vm = new DrawingRequisitionItemVm();
                        vm.RequisitionID = drawingRequisitionID;
                        vm.AvailableEstItems = results;
                        vm.CreateRequisitionItems();
                        foreach (var ret in results)
                        {
                            if (Convert.ToInt16(ret.Value2) > 0)
                            {
                                CommonMethods.InsertToMISSalesForceMapping(TableName.Sales_Dispatching_DrawingRequisition_EstimationItem, ret.Value1, ret.Value2, sfDrawingID, salesForceProjectID);
                            }
                        }
                    }

                    // delete old data
                    DeleteAllDeletedDrawingItems(items.ToArray(), sfDrawingID);

                    LogMethods.Log.Debug("GetAllDrawingItems:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllDrawingItems:Error:" + e.Message);
            }
        }

        private void UpdateDrawingItem(int requisitionItemID, double? qty, string description)
        {
            var item = _db.Sales_Dispatching_DrawingRequisition_EstimationItem.Where(x => x.RequisitionItemID == requisitionItemID).FirstOrDefault();
            if (item != null)
            {
                if (qty != null)
                {
                    item.Qty = qty.ToString();
                }

                if (!string.IsNullOrEmpty(description))
                {
                    item.Description = description;
                }

                _db.Entry(item).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }

        private void DeleteAllDeletedDrawingItems(string[] items, string sfDrawingID)
        {
            try
            {
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.Sales_Dispatching_DrawingRequisition_EstimationItem, sfDrawingID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(items, i) == -1)
                    {
                        // get MISID
                        int itemIDTemp = CommonMethods.GetMISID(TableName.Sales_Dispatching_DrawingRequisition_EstimationItem, i, sfDrawingID, salesForceProjectID);
                        // get a row
                        var drawingItem = _db.Sales_Dispatching_DrawingRequisition_EstimationItem.Where(x => x.RequisitionItemID == itemIDTemp).FirstOrDefault();
                        if (drawingItem != null)
                        {
                            _db.Sales_Dispatching_DrawingRequisition_EstimationItem.Remove(drawingItem);
                            _db.SaveChanges();
                        }
                        // remove MISID out of MISSalesForceMapping
                        CommonMethods.Delete(TableName.Sales_Dispatching_DrawingRequisition_EstimationItem, i, sfDrawingID, salesForceProjectID);
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedDrawingItems:Error:" + e.Message);
            }
        }

        private int UpdateDrawing(int estRevID, double? version, string drawingType, string drawingPurpose, string Is_Electronic_File_From_Client_Available__c,
            string gIs_GC_Or_Designer_Drawing_Available__c, string Is_Landord_Or_Mall_Criteria_Available__c, string Is_Latest_Version_Q_D_Quotation_Avail__c,
            string Is_Site_Check_Photo_Available__c, string Is_Site_Check_Report_Available__c)
        {
            int requisitionId = 0;
            try
            {
                var DrawingRequisition = _db.Sales_Dispatching_DrawingRequisition_Estimation.FirstOrDefault(x => x.EstRevID == estRevID);
                if (DrawingRequisition != null)
                {
                    DrawingRequisition.DrawingType = drawingType;
                    switch (drawingPurpose)
                    {
                        case DrawingPurpose.Estimation_Drawing:
                            DrawingRequisition.DrawingPurpose = 102;
                            break;
                        case DrawingPurpose.Permit_Drawing:
                            DrawingRequisition.DrawingPurpose = 103;
                            break;
                        case DrawingPurpose.Workorder_Drawing:
                            DrawingRequisition.DrawingPurpose = 104;
                            break;
                        case DrawingPurpose.Concept_Design:
                            DrawingRequisition.DrawingPurpose = 105;
                            break;
                        default:
                            break;
                    }

                    if (version != null)
                    {
                        DrawingRequisition.Version = Convert.ToInt16(version);
                    }

                    bool isValid = true;

                    if (Is_Electronic_File_From_Client_Available__c != null)
                    {
                        switch (Is_Electronic_File_From_Client_Available__c)
                        {
                            case YesNo.Yes:
                                DrawingRequisition.IsElectronicFillFromClientAvailable = 1;
                                break;
                            case YesNo.No:
                                DrawingRequisition.IsElectronicFillFromClientAvailable = 2;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        isValid = false;
                    }

                    if (gIs_GC_Or_Designer_Drawing_Available__c != null)
                    {
                        switch (gIs_GC_Or_Designer_Drawing_Available__c)
                        {
                            case YesNo.Yes:
                                DrawingRequisition.IsGCorDesignerDrawingAvailable = 1;
                                break;
                            case YesNo.No:
                                DrawingRequisition.IsGCorDesignerDrawingAvailable = 2;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        isValid = false;
                    }

                    if (Is_Landord_Or_Mall_Criteria_Available__c != null)
                    {
                        switch (Is_Landord_Or_Mall_Criteria_Available__c)
                        {
                            case YesNo.Yes:
                                DrawingRequisition.IsLandordOrMallCriteriaAvailable = 1;
                                break;
                            case YesNo.No:
                                DrawingRequisition.IsLandordOrMallCriteriaAvailable = 2;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        isValid = false;
                    }

                    if (Is_Latest_Version_Q_D_Quotation_Avail__c != null)
                    {
                        switch (Is_Latest_Version_Q_D_Quotation_Avail__c)
                        {
                            case YesNo.Yes:
                                DrawingRequisition.IsQuotationAvailable = 1;
                                break;
                            case YesNo.No:
                                DrawingRequisition.IsQuotationAvailable = 2;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        isValid = false;
                    }

                    if (Is_Site_Check_Photo_Available__c != null)
                    {
                        switch (Is_Site_Check_Photo_Available__c)
                        {
                            case YesNo.Yes:
                                DrawingRequisition.IsSiteCheckPhotoAvailable = 1;
                                break;
                            case YesNo.No:
                                DrawingRequisition.IsSiteCheckPhotoAvailable = 2;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        isValid = false;
                    }

                    if (Is_Site_Check_Report_Available__c != null)
                    {
                        switch (Is_Site_Check_Report_Available__c)
                        {
                            case YesNo.Yes:
                                DrawingRequisition.IsSiteCheckReportAvailable = 1;
                                break;
                            case YesNo.No:
                                DrawingRequisition.IsSiteCheckReportAvailable = 2;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        isValid = false;
                    }

                    DrawingRequisition.IsValid = isValid;

                    _db.Entry(DrawingRequisition).State = EntityState.Modified;
                    _db.SaveChanges();

                    requisitionId = DrawingRequisition.RequisitionID;
                    LogMethods.Log.Debug("UpdateDrawing:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("UpdateDrawing:Error:" + e.Message);
            }

            return requisitionId;
        }


    }
}
