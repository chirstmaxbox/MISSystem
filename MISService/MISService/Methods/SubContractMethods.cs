using MISService.Method;
using MISService.Models;
using ProjectDomain;
using SubContractDomain.BLL;
using SubContractDomain.Model;
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
    public class SubContractMethods
    {
        private readonly SubContractDbEntities _db = new SubContractDbEntities();
        private readonly ProjectModelDbEntities _db1 = new ProjectModelDbEntities();
        private EndpointAddress apiAddr;
        private enterprise.SessionHeader header;
        private string salesForceProjectID;

        public SubContractMethods(string salesForceProjectID)
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
            this.salesForceProjectID = salesForceProjectID;
        }

        public void GetAllSubContracts(string sfProjectID, int jobID, int userEmployeeID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Name, First_Site_Contact__c, Second_Site_Contact__c, Budget__c, Provided_By__c, "
                        + " Remarks__c, Due_Date__c, Rush__c, Requirement__c, Requirement_As_Other__c, Estimated_Shipping_Cost__c, Shipping_Items_Total_Value__c, Work_Order_Number__c "
                        + " FROM SubContract__c where Project_Name__c = '" + sfProjectID + "'";

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
                    IEnumerable<enterprise.SubContract__c> subContractList = result.records.Cast<enterprise.SubContract__c>();
                    List<string> items = new List<string>();
                    foreach (var sp in subContractList)
                    {
                        items.Add(sp.Id);
                        /* check if the sign permit exists */
                        int subContractID = CommonMethods.GetMISID(TableName.SubContract, sp.Id, salesForceProjectID);
                        if (subContractID == 0)
                        {
                            SubContractDomain.Model.SubContract sc = new SubContractDomain.Model.SubContract();
                            sc.JobID = jobID;
                            sc.RequestDate = DateTime.Now;
                            sc.RequestBy = userEmployeeID;
                            sc.StatusID = 0;
                            sc.ContactPerson1 = 0;
                            sc.ContactPerson2 = 0;
                            sc.Budget = 0;
                            sc.Remark = "";
                            sc.TargetDate = DateTime.Today;
                            sc.RequirementID = 0;
                            sc.Requirement = "";

                            SubContractCreate scCreate = new SubContractCreate(userEmployeeID);
                            scCreate.Create(sc);
                            subContractID = scCreate.NewlyInsertedID;
                            CommonMethods.InsertToMISSalesForceMapping(TableName.SubContract, sp.Id, subContractID.ToString(), salesForceProjectID);
                        }

                        if (subContractID != 0)
                        {
                            UpdateSubContract(subContractID, salesForceProjectID, sp.First_Site_Contact__c, sp.Second_Site_Contact__c, sp.Budget__c, sp.Provided_By__c, 
                                    sp.Remarks__c, sp.Due_Date__c, sp.Rush__c, sp.Requirement__c, sp.Requirement_As_Other__c, sp.Estimated_Shipping_Cost__c, sp.Shipping_Items_Total_Value__c, sp.Work_Order_Number__c);
                        }
                    }

                    DeleteAllDeletedSubContracts(items.ToArray());

                    LogMethods.Log.Debug("GetAllSubContracts:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllSubContracts:Error:" + e.Message);
            }
        }

        private void DeleteAllDeletedSubContracts(string[] items)
        {
            try
            {
                List<string> ids = CommonMethods.GetAllSalesForceIDWithoutParent(TableName.SubContract, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(items, i) == -1)
                    {
                        // get MISID
                        int itemIDTemp = CommonMethods.GetMISID(TableName.SubContract, i, salesForceProjectID);
                        // get a row
                        var estItem = _db.SubContracts.Where(x => x.SubcontractID == itemIDTemp).FirstOrDefault();
                        if (estItem != null)
                        {
                            _db.SubContracts.Remove(estItem);
                            _db.SaveChanges();
                        }
                        // remove MISID out of MISSalesForceMapping
                        CommonMethods.Delete(TableName.SubContract, i, salesForceProjectID);
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedSubContracts:Error:" + e.Message);
            }
        }

        private void UpdateSubContract(int subContractID, string sfProjectID, string firstSiteContact, string secondSiteContact, double? budget, string providedBy,
                     string remarks, DateTime? dueDate, string rush, string requirement, string requirementAsOther, double? estimatedShippingCost, double? itemValue, string workOrderList)
        {
            try
            {
                var subContract = _db.SubContracts.Where(x => x.SubcontractID == subContractID).FirstOrDefault();
                if (subContract != null)
                {
                    if (firstSiteContact != null)
                    {
                        int val = CommonMethods.GetMISID(TableName.Customer_Contact, firstSiteContact, sfProjectID);
                        if (val != 0)
                        {
                            subContract.ContactPerson1 = val;
                        }
                    }

                    if (secondSiteContact != null)
                    {
                        int val = CommonMethods.GetMISID(TableName.Customer_Contact, secondSiteContact, sfProjectID);
                        if (val != 0)
                        {
                            subContract.ContactPerson2 = val;
                        }
                    }

                    if (budget != null)
                    {
                        subContract.Budget = (double)budget;
                    }

                    if (providedBy != null)
                    {
                        subContract.BudgetProvideBy = providedBy;
                    }

                    if (remarks != null)
                    {
                        subContract.Remark = remarks;
                    }

                    if (dueDate != null)
                    {
                        subContract.TargetDate = (DateTime)dueDate;
                    }

                    switch (rush)
                    {
                        case "Yes":
                            subContract.IsRush = true;
                            break;
                        case "No":
                            subContract.IsRush = false;
                            break;
                        default:
                            break;
                    }

                    switch (requirement)
                    {
                        case "Installation":
                            subContract.RequirementID = 5;
                            subContract.Requirement = requirement;
                            break;
                        case "Site Check":
                            subContract.RequirementID = 10;
                            subContract.Requirement = requirement;
                            break;
                        case "Service":
                            subContract.RequirementID = 15;
                            subContract.Requirement = requirement;
                            break;
                        case "Electrical Hook Up":
                            subContract.RequirementID = 20;
                            subContract.Requirement = requirement;
                            break;
                        case "Other":
                            subContract.RequirementID = 65531;
                            if (requirementAsOther != null)
                            {
                                subContract.Requirement = requirementAsOther;
                            }
                            else
                            {
                                subContract.Requirement = "";
                            }
                            break;
                        default:
                            break;
                    }

                    if (estimatedShippingCost != null)
                    {
                        subContract.EstimatedShippingCost = estimatedShippingCost;
                    }

                    if (itemValue != null)
                    {
                        subContract.ItemValue = itemValue;
                    }

                    _db.Entry(subContract).State = EntityState.Modified;
                    _db.SaveChanges();
                }

                List<int> processedItems = new List<int>();
                if (!string.IsNullOrEmpty(workOrderList))
                {
                    char[] delimiters = new char[] { '\n', ' ', ',' };
                    string[] items = workOrderList.Split(delimiters);
                    if (items.Length != 0)
                    {
                        foreach (string e in items)
                        {
                            var workOrder = _db1.Sales_JobMasterList_WO.Where(x => x.WorkorderNumber == e).FirstOrDefault();
                            if (workOrder != null)
                            {
                                int rowId = 0;
                                var subContractWO = _db.SubcontractWorkorders.Where(x => x.SubcontractID == subContractID && x.WorkorderID == workOrder.woID).FirstOrDefault();
                                if (subContractWO == null)
                                {
                                    // not exist, add one row
                                    SubcontractWorkorder scWO = new SubcontractWorkorder();
                                    scWO.SubcontractID = subContractID;
                                    scWO.WorkorderID = workOrder.woID;

                                    _db.SubcontractWorkorders.Add(scWO);
                                    _db.SaveChanges();

                                    rowId = scWO.RowID;
                                }
                                else
                                {
                                    rowId = subContractWO.RowID;
                                }
                                /* add it to know that it still exists */
                                processedItems.Add(rowId);
                            }
                        }
                    }
                }

                /* delete all items which are not in the processedItems list */
                var records = _db.SubcontractWorkorders.Where(x => x.SubcontractID == subContractID).ToList();
                if (records.Any())
                {
                    foreach (var r in records)
                    {
                        if (processedItems.IndexOf(r.RowID) == -1)
                        {
                            //delete it
                            _db.SubcontractWorkorders.Remove(r);
                        }
                    }
                    _db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("UpdateSubContract:Error:" + e.Message);
            }
        }
    }
}
