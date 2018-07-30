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

        public void GetAllDrawings(string sfProjectID, int estRevID, int jobID, enterprise.QueryResult result)
        {
            try
            {
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    if (result == null || (result != null && result.size == 0)) return;

                    IEnumerable<enterprise.Drawing__c> drawingList = result.records.Cast<enterprise.Drawing__c>();
                    /* in MIS, only one drawing */
                    foreach (var dl in drawingList)
                    {
                        /* one unique row will be inserted if it is not existent */
                        var vm = new DrawingRequisitionFormVm(jobID, estRevID);
                        vm.Initialization();

                        /* update data */
                        int requisitionId = UpdateDrawing(estRevID, dl.Version__c, dl.Drawing_Requisition_Type__c, dl.Drawing_Purpose__c, dl.Is_Electronic_File_From_Client_Available__c,
                            dl.Is_GC_Or_Designer_Drawing_Available__c, dl.Is_Landord_Or_Mall_Criteria_Available__c, dl.Is_Latest_Version_Q_D_Quotation_Avail__c,
                            dl.Is_Site_Check_Photo_Available__c, dl.Is_Site_Check_Report_Available__c);

                        if (requisitionId != 0)
                        {
                            /* Salesforce can create multi-drawing request but the MIS only supports one at a time so 
                             I will only show the latest one on the MIS system*/
                            // update SalesForceParentID in MISSalesForceMapping 
                            UpdateMISSalesForceMapping(TableName.Sales_Dispatching_DrawingRequisition_EstimationItem, dl.Id);

                            // add items to drawing
                            GetAllDrawingItems(sfProjectID, requisitionId, estRevID, dl.Id);
                        }
                    }

                }
                
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllDrawing:Error:" + e.Message);
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

        private void GetAllDrawingItems(string sfProjectID, int drawingRequisitionID, int estRevID, string sfDrawingID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Item_Name__c, Item_Description__c, Quantity__c FROM Item__c where Drawing_Name__c = '" + sfDrawingID + "'";

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
