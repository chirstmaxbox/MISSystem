using MISService.Method;
using MISService.Models;
using ProjectDomain;
using SalesCenterDomain.BDL.Workorder;
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
    public class WorkOrderMethods
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private EndpointAddress apiAddr;
        private enterprise.SessionHeader header;

        public WorkOrderMethods()
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
        }

        public void GetAllWorkOrders(string sfProjectID, int jobID, int estRevID, int userEmployeeID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Name, Work_Order_Type__c, Payment_Method__c, Version__c, Rush__c, Rush_Reason__c, Remarks__c, "
                        + " Issue_Date__c, Due_Date__c, Clone_Type__c, Previous_Work_Order_Number__c, List_Item_Name__c "
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
                        int workOrderID = CommonMethods.GetMISID(TableName.Sales_JobMasterList_WO, ql.Id);
                        if (workOrderID == 0)
                        {
                            // not exist
                            WorkorderGenerateFromEstimation gw = new WorkorderGenerateFromEstimation(jobID, estRevID);
                            gw.CreateNew();
                            workOrderID = gw.WoID;
                            CommonMethods.InsertToMISSalesForceMapping(TableName.Sales_JobMasterList_WO, ql.Id, workOrderID.ToString());
                        }

                        if (workOrderID != 0)
                        {
                            UpdateWorkOrder(workOrderID, ql.Name, ql.Work_Order_Type__c, ql.Payment_Method__c, ql.Version__c, ql.Rush__c, ql.Rush_Reason__c,
                                ql.Remarks__c, ql.Issue_Date__c, ql.Due_Date__c, ql.Clone_Type__c, ql.Previous_Work_Order_Number__c);

                            // generate work order items
                            GenerateWorkOrderItem(workOrderID, ql.List_Item_Name__c, ql.Id);
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

        private void GenerateWorkOrderItem(int workOrderID, string listItemID, string sfWorkOrderID)
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
                        int itemIDTemp = CommonMethods.GetMISID(TableName.WO_Item, il.Id, sfWorkOrderID);
                        if (itemIDTemp == 0)
                        {
                            WokrorderItemGenerateFromBlank woItem = new WokrorderItemGenerateFromBlank(workOrderID);
                            // no error
                            if (woItem.ValidationID == 0)
                            {
                                woItem.InsertItem();
                                int newWOItemID = woItem.NewWorkItemID;
                                CommonMethods.InsertToMISSalesForceMapping(TableName.WO_Item, il.Id, newWOItemID.ToString(), sfWorkOrderID);
                            }
                        }
                        else
                        {
                            UpdateWorkOrderItem(itemIDTemp, il.Item_Name__c, il.Requirement__c, il.Work_Order_Item_Description__c, il.Item_Cost__c, il.Quantity__c);
                        }
                    }

                    LogMethods.Log.Debug("GenerateWorkOrderItem:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GenerateWorkOrderItem:Error:" + e.Message);
            }
        }

        private void UpdateWorkOrderItem(long workOrderItemID, string itemName, string requirement, string description, double? itemCost, double? quality)
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

                _db.Entry(workOrderItem).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }

        private void UpdateWorkOrder(int workOrderID, string woNumber, string woType, string paymentMethod, double? version, string rush, string rushReason,
                        string remarks, DateTime? issueDate, DateTime? dueDate, string cloneType, string preWONumber)
        {
            var workOrder = _db.Sales_JobMasterList_WO.Where(x => x.woID == workOrderID).FirstOrDefault();
            if (workOrder != null)
            {
                workOrder.WorkorderNumber = woNumber;

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
                        break;
                    default:
                        break;
                }

                _db.Entry(workOrder).State = EntityState.Modified;
                _db.SaveChanges();
            }

        }
    }
}
