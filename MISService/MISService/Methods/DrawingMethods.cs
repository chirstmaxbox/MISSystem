using MISService.Method;
using MISService.Models;
using MyCommon;
using SpecDomain.BLL.Task;
using SpecDomain.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public DrawingMethods()
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
        }

        public void GetAllDrawing(string sfProjectID, int estRevID, int jobID)
        {
            try
            {
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Name, Drawing_Requisition_Type__c, Drawing_Purpose__c, Is_Electronic_File_From_Client_Available__c, "
                        + " Is_GC_Or_Designer_Drawing_Available__c, Is_Landord_Or_Mall_Criteria_Available__c, Is_Latest_Version_Q_D_Quotation_Avail__c, "
                        + " Is_Site_Check_Photo_Available__c, Is_Site_Check_Report_Available__c, LastModifiedDate FROM Drawing__c where Project_Name__c = '" + sfProjectID + "'" + " order by LastModifiedDate desc limit 1";

                    enterprise.QueryResult result;
                    queryClient.query(
                        header,
                        null,
                        null,
                        null,
                        query, out result);

                    IEnumerable<enterprise.Drawing__c> drawingList = result.records.Cast<enterprise.Drawing__c>();
                    /* in MIS, only one drawing */
                    foreach (var dl in drawingList)
                    {
                        /* one unique row will be inserted if it is not existent */
                        var vm = new DrawingRequisitionFormVm(jobID, estRevID);
                        vm.Initialization();

                        /* update data */
                        int requisitionId = UpdateDrawing(estRevID, dl.Drawing_Requisition_Type__c, dl.Drawing_Purpose__c, dl.Is_Electronic_File_From_Client_Available__c,
                            dl.Is_GC_Or_Designer_Drawing_Available__c, dl.Is_Landord_Or_Mall_Criteria_Available__c, dl.Is_Latest_Version_Q_D_Quotation_Avail__c,
                            dl.Is_Site_Check_Photo_Available__c, dl.Is_Site_Check_Report_Available__c);

                        if (requisitionId != 0)
                        {
                            GetAllDrawingItems(sfProjectID, requisitionId);
                        }
                    }

                }
                
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllDrawing:Error:" + e.Message);
            }
        }

        public void GetAllDrawingItems(string sfProjectID, int drawingRequisitionID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id FROM Item__c where Estimation_Name__r.Project_Name__c = '" + sfProjectID + "'";

                    enterprise.QueryResult result;
                    queryClient.query(
                        header, //sessionheader
                        null, //queryoptions
                        null, //mruheader
                        null, //packageversion
                        query, out result);

                    //cast query results
                    IEnumerable<enterprise.Item__c> itemList = result.records.Cast<enterprise.Item__c>();

                    //show results
                    var results = new List<MyLongKeyValueBool>();
                    foreach (var il in itemList)
                    {
                        int itemIDTemp = CommonMethods.GetMISID(TableName.Sales_Dispatching_DrawingRequisition_EstimationItem, il.Id);
                        if (itemIDTemp == 0)
                        {
                            /* get item ID from Mapping Table */
                            int itemID = CommonMethods.GetMISID(TableName.EST_Item, il.Id);
                            if (itemID != 0)
                            {
                                var r = new MyLongKeyValueBool();
                                r.Key = itemID;
                                results.Add(r);
                            }
                        }
                    }

                    if (results.Any())
                    {
                        var vm = new DrawingRequisitionItemVm();
                        vm.RequisitionID = drawingRequisitionID;
                        vm.AvailableEstItems = results;
                        vm.CreateRequisitionItems();
                    }
                    LogMethods.Log.Debug("GetAllDrawingItems:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllDrawingItems:Error:" + e.Message);
            }
        }

        /*
        public List<MyLongKeyValueBool> GetAllItemsOfSpecificDrawing(string sfDrawingID)
        {
            var results = new List<MyLongKeyValueBool>();
            try
            {
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id FROM Item__c where Drawing_Name__c = '" + sfDrawingID + "'";

                    enterprise.QueryResult result;
                    queryClient.query(
                        header,
                        null,
                        null,
                        null,
                        query, out result);

                    IEnumerable<enterprise.Item__c> itemList = result.records.Cast<enterprise.Item__c>();
                    foreach (var il in itemList)
                    {
                        int itemID = CommonMethods.GetMISID(TableName.EST_Item, il.Id);
                        if (itemID != 0)
                        {
                            var r = new MyLongKeyValueBool();
                            r.Key = itemID;
                            results.Add(r);
                        }
                    }
                    LogMethods.Log.Debug("GetAllItemsOfSpecificDrawing:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllItemsOfSpecificDrawing:Error:" + e.Message);
            }

            return results;
        }
        */

        public int UpdateDrawing(int estRevID, string drawingType, string drawingPurpose, string Is_Electronic_File_From_Client_Available__c,
            string gIs_GC_Or_Designer_Drawing_Available__c, string Is_Landord_Or_Mall_Criteria_Available__c, string Is_Latest_Version_Q_D_Quotation_Avail__c,
            string Is_Site_Check_Photo_Available__c, string Is_Site_Check_Report_Available__c)
        {
            int requisitionId = 0;
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

                bool isValid = true;

                if(Is_Electronic_File_From_Client_Available__c != null) {
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
            }

            return requisitionId;
        }


    }
}
