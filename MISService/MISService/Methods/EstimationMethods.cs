﻿using CustomerDomain.Model;
using MISService.Method;
using MISService.Models;
using MyCommon;
using SpecDomain.BLL.EstItem;
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

    public class EstimationMethods
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        private EndpointAddress apiAddr;
        private enterprise.SessionHeader header;
        private string salesForceProjectID;

        public EstimationMethods(string salesForceProjectID)
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
            this.salesForceProjectID = salesForceProjectID;
        }

        public void GetEstimation(string sfProjectID, int estRevID, int jobID, int employeeNumber)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Name, Cost__c, Remarks__c, Version__c, Temp_Estimation_Hour__c, Target_Date__c, Due_Time__c, Issue_Date_Time__c, "
                                        + " (SELECT Status, LastActor.Name, CompletedDate FROM ProcessInstances order by CompletedDate desc limit 1),"
                                        + " (SELECT Id, Name, Item_Order__c, Category__c, Sign_Type__c, Feature_1__c, Feature_2__c, Graphic__c, Item_Name__c, Previous_Estimation_Available__c, Sale_Requirement__c, Estimator_Description__c, Position__c, Requirement__c, Quantity__c, Item_Cost__c, Height_Feet__c, Height_Feet1_s__c, Height_Feet2_s__c, Height_Feet3_s__c, Height_Inches__c, Height_Inches1__c, Height_Inches2__c, Height_Inches3__c, Width_Feet_s__c, Width_Inches__c, Thickness_Feet_s__c, Thickness_Feet1_s__c, Thickness_Feet2_s__c, Thickness_Feet3_s__c, Thickness_Inches__c, Thickness_Inches1__c, Thickness_Inches2__c, Thickness_Inches3__c, PC_s__c, PC1_s__c, PC2_s__c, PC3_s__c, Item_Option__c FROM Items__r),"
                                        + " (SELECT Id, Service_Name__r.Name, Service_Detail__c, Service_Cost__c, Note__c, Service_Name__r.MIS_Service_Number__c FROM Service_Costs__r) "
                                        + " FROM Estimation__c "
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

                    if (result == null || (result != null && result.size == 0)) return;
                    //cast query results
                    IEnumerable<enterprise.Estimation__c> estimationList = result.records.Cast<enterprise.Estimation__c>();

                    //show results
                    foreach (var el in estimationList)
                    {
                        /* item */
                        GetAllItems(el.Id, estRevID, el.Items__r);

                        /* services */
                        ServiceMethods sm = new ServiceMethods(salesForceProjectID);
                        sm.GetAllServices(el.Id, estRevID, el.Service_Costs__r);

                        UpdateEstimation(estRevID, el.Cost__c, el.Remarks__c, el.Version__c);

                        GetEstimationApprovalData(el.Id, jobID, estRevID, el.ProcessInstances, el.Version__c, employeeNumber, el.Temp_Estimation_Hour__c, el.Target_Date__c, el.Due_Time__c, el.Issue_Date_Time__c);
                    }
                    LogMethods.Log.Debug("GetEstimation:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetEstimation:Error:" + e.Message);
            }
        }
        
        private void GetEstimationApprovalData(string sfEstimaitonID, int jobId, int estRevID, enterprise.QueryResult result, double? version, int employeeNumber, double? estHour, DateTime? dueDate, DateTime? dueTime, DateTime? issueDateTime )
        {
            try
            {
                if (version == null || result == null || (result != null && result.size == 0)) return;

                //cast query results
                IEnumerable<enterprise.ProcessInstance> processInstanceList = result.records.Cast<enterprise.ProcessInstance>();

                foreach (var el in processInstanceList)
                {
                    if (el.Status == "Pending")
                    {
                        var sales_Dispatching = _db.Sales_Dispatching.Where(x => x.JobID == jobId && x.TaskType == 201 && x.Importance == version).FirstOrDefault();
                        if (sales_Dispatching == null)
                        {
                            SubmitEstimationRequestVm vm = new SubmitEstimationRequestVm();
                            vm.JobID = jobId;
                            vm.EstRevID = estRevID;
                            vm.SubmitBy = employeeNumber;
                            vm.EstimatorID = 8; //mr Fan

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

                            if (issueDateTime != null)
                            {
                                vm.Create(issueDateTime.Value.ToLocalTime());
                            }
                            else
                                vm.Create();
                            vm.OnEstimationSubmittedWithoutChangingEstVer();
                        }
                    }
                    else if (el.Status == "Approved")
                    {
                        var sales_Dispatching = _db.Sales_Dispatching.Where(x => x.JobID == jobId && x.TaskType == 201 && x.Importance == version).FirstOrDefault();
                        if (sales_Dispatching != null)
                        {
                            if (estHour != null)
                            {
                                sales_Dispatching.WorkedHour = estHour;
                            }
                            sales_Dispatching.Status = 249;
                            if (el.CompletedDate != null)
                            {
                                sales_Dispatching.FinishedDate = el.CompletedDate.Value.ToLocalTime();
                            }

                            _db.Entry(sales_Dispatching).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                    }
                    else if (el.Status == "Removed")
                    {
                        var sales_Dispatching = _db.Sales_Dispatching.Where(x => x.JobID == jobId && x.TaskType == 201 && x.Importance == version + 1).FirstOrDefault();
                        if (sales_Dispatching != null)
                        {
                            /* delete a row in Sales_Dispatching */
                            _db.Sales_Dispatching.Remove(sales_Dispatching);
                            _db.SaveChanges();
                        }
                    }
                }

            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetEstimationApprovalData:Error:" + e.Message);
            }
        }
     
        private void UpdateEstimation(int estRevID, double? cost, string remarks, double? version)
        {
            try
            {
                var est = _db.Sales_JobMasterList_EstRev.Where(x => x.EstRevID == estRevID).FirstOrDefault();
                if (est != null)
                {
                    if (cost != null)
                    {
                        est.erAmount = Convert.ToDecimal(cost);
                    }

                    if (!string.IsNullOrEmpty(remarks))
                    {
                        est.Remark = remarks;
                    }

                    if (version != null)
                    {
                        est.erRev = Convert.ToByte(version);
                    }

                    _db.Entry(est).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("UpdateEstimation:Error:" + e.Message);
            }
        }

        private void GetAllItems(string sfEstimation, int estRevID, enterprise.QueryResult result) 
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
                    List<string> items = new List<string>();
                    foreach (var il in itemList)
                    {
                        items.Add(il.Id);
                        long estItemID = CommonMethods.GetMISID(TableName.EST_Item, il.Id, sfEstimation, salesForceProjectID);
                        if (estItemID == 0)
                        {
                            int productID = 0;
                            Product optionDetails = _db.Products.Where(x => x.ProductName.Trim() == il.Sign_Type__c & x.Active).FirstOrDefault();
                            if (optionDetails != null)
                            {
                                productID = optionDetails.ProductID;
                            }
                            var est = new MyEstItemCreate(estRevID, productID, il.Item_Name__c);
                            if (est != null && est.EstItemID > 0)
                            {
                                CommonMethods.InsertToMISSalesForceMapping(TableName.EST_Item, il.Id, est.EstItemID.ToString(), sfEstimation, salesForceProjectID);
                            }
                            estItemID = est.EstItemID;
                        }

                        UpdateEstItem(estItemID, il.Item_Name__c, il.Item_Order__c, il.Sign_Type__c, il.Previous_Estimation_Available__c, il.Sale_Requirement__c,  
                            il.Estimator_Description__c, il.Position__c, il.Requirement__c, il.Quantity__c, il.Item_Cost__c, il.Item_Option__c );

                        UpdateEstItemSize(estItemID, il.Height_Feet__c, il.Height_Feet1_s__c, il.Height_Feet2_s__c, il.Height_Feet3_s__c,
                             il.Height_Inches__c, il.Height_Inches1__c, il.Height_Inches2__c, il.Height_Inches3__c, il.Width_Feet_s__c, il.Width_Inches__c,
                             il.Thickness_Feet_s__c, il.Thickness_Feet1_s__c, il.Thickness_Feet2_s__c, il.Thickness_Feet3_s__c,
                             il.Thickness_Inches__c, il.Thickness_Inches1__c, il.Thickness_Inches2__c, il.Thickness_Inches3__c,
                             il.PC_s__c, il.PC1_s__c, il.PC2_s__c, il.PC3_s__c);
                    }

                    /* delete old items */
                    DeleteAllDeletedEstimationItems(items.ToArray(), sfEstimation);

                    LogMethods.Log.Debug("GetAllItems:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllItems:Error:" + e.Message);
            }
        }

        private void DeleteAllDeletedEstimationItems(string[] items, string sfEstimation)
        {
            try
            {
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.EST_Item, sfEstimation, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(items, i) == -1)
                    {
                        // get MISID
                        int itemIDTemp = CommonMethods.GetMISID(TableName.EST_Item, i, sfEstimation, salesForceProjectID);
                        // get a row
                        var estItem = _db.EST_Item.Where(x => x.EstItemID == itemIDTemp).FirstOrDefault();
                        if (estItem != null)
                        {
                            estItem.EstRevID = 0;
                            _db.Entry(estItem).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                        // remove MISID out of MISSalesForceMapping
                        CommonMethods.Delete(TableName.EST_Item, i, sfEstimation, salesForceProjectID);
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedEstimationItems:Error:" + e.Message);
            }
        }

        private void UpdateEstItemSize(long estItemID, string Height_Feet__c, string Height_Feet1_s__c, string Height_Feet2_s__c, string Height_Feet3_s__c,
            string Height_Inches__c, string Height_Inches1__c, string Height_Inches2__c, string Height_Inches3__c, string Width_Feet_s__c, string Width_Inches__c,
            string Thickness_Feet_s__c, string Thickness_Feet1_s__c, string Thickness_Feet2_s__c, string Thickness_Feet3_s__c,
            string Thickness_Inches__c, string Thickness_Inches1__c, string Thickness_Inches2__c, string Thickness_Inches3__c,
            string PC_s__c, string PC1_s__c, string PC2_s__c, string PC3_s__c)
        {
            try
            {
                List<EST_Item_Specification_Size> est_Item_Specification_SizeList = _db.EST_Item_Specification_Size.Where(x => x.EstItemID == estItemID).ToList();
                int i = 0;
                if (est_Item_Specification_SizeList.Any())
                {
                    foreach (var e in est_Item_Specification_SizeList)
                    {
                        switch (i)
                        {
                            case 0:
                                e.WidthFeet = Width_Feet_s__c == null ? 0 : Convert.ToInt32(Width_Feet_s__c);
                                e.WidthInch = Width_Inches__c == null ? "" : Width_Inches__c;

                                e.HeightFeet = Height_Feet__c == null ? 0 : Convert.ToInt32(Height_Feet__c);
                                e.HeightInch = Height_Inches__c == null ? "" : Height_Inches__c;

                                e.ThicknessFeet = Thickness_Feet_s__c == null ? 0 : Convert.ToInt32(Thickness_Feet_s__c);
                                e.ThicknessInch = Thickness_Inches__c == null ? "" : Thickness_Inches__c;

                                e.Pc = PC_s__c == null ? 0 : Convert.ToInt32(PC_s__c);
                                break;
                            case 1:
                                e.HeightFeet = Height_Feet1_s__c == null ? 0 : Convert.ToInt32(Height_Feet1_s__c);
                                e.HeightInch = Height_Inches1__c == null ? "" : Height_Inches1__c;

                                e.ThicknessFeet = Thickness_Feet1_s__c == null ? 0 : Convert.ToInt32(Thickness_Feet1_s__c);
                                e.ThicknessInch = Thickness_Inches1__c == null ? "" : Thickness_Inches1__c;

                                e.Pc = PC1_s__c == null ? 0 : Convert.ToInt32(PC1_s__c);
                                break;
                            case 2:
                                e.HeightFeet = Height_Feet2_s__c == null ? 0 : Convert.ToInt32(Height_Feet2_s__c);
                                e.HeightInch = Height_Inches2__c == null ? "" : Height_Inches2__c;

                                e.ThicknessFeet = Thickness_Feet2_s__c == null ? 0 : Convert.ToInt32(Thickness_Feet2_s__c);
                                e.ThicknessInch = Thickness_Inches2__c == null ? "" : Thickness_Inches2__c;

                                e.Pc = PC2_s__c == null ? 0 : Convert.ToInt32(PC2_s__c);
                                break;
                            case 3:
                                e.HeightFeet = Height_Feet3_s__c == null ? 0 : Convert.ToInt32(Height_Feet3_s__c);
                                e.HeightInch = Height_Inches3__c == null ? "" : Height_Inches3__c;

                                e.ThicknessFeet = Thickness_Feet3_s__c == null ? 0 : Convert.ToInt32(Thickness_Feet3_s__c);
                                e.ThicknessInch = Thickness_Inches3__c == null ? "" : Thickness_Inches3__c;

                                e.Pc = PC3_s__c == null ? 0 : Convert.ToInt32(PC3_s__c);
                                break;
                            default:
                                break;
                        }
                        _db.Entry(e).State = EntityState.Modified;
                        i++;
                    }
                    _db.SaveChanges();
                }
                LogMethods.Log.Debug("UpdateEstItemSize:Debug:" + "Done");
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("UpdateEstItemSize:Error:" + e.Message);
            }
        }

        private void UpdateEstItem(long estItemID, string itemName, double? itemOrder, string signType, string previousEstimation, string saleRequirement,
            string EstimatorDesc, string position, string requirement, double? quality, double? itemCost, double? itemOption)
        {
            try
            {
                var item = _db.EST_Item.Find(estItemID);
                if (item != null)
                {
                    switch (previousEstimation)
                    {
                        case YesNo.Yes:
                            item.IsPreviousEstimationAvailable = 1;
                            break;
                        case YesNo.No:
                            item.IsPreviousEstimationAvailable = 2;
                            break;
                        default:
                            item.IsPreviousEstimationAvailable = 0;
                            break;
                    }

                    item.ProductName = itemName;
                    if (itemOrder != null)
                    {
                        item.EstItemNo = Convert.ToInt16(itemOrder);
                    }

                    if (itemOption != null)
                    {
                        item.ItemOption = Convert.ToInt16(itemOption);
                    }
                    /* product ID */
                    int productID = 0;
                    Product optionDetails = _db.Products.Where(x => x.ProductName.Trim() == signType & x.Active).FirstOrDefault();
                    if (optionDetails != null)
                    {
                        productID = optionDetails.ProductID;
                    }
                    item.ProductID = productID;
                    item.SalesDescription = saleRequirement;
                    item.Description = EstimatorDesc;

                    switch (position)
                    {
                        case ItemPosition.Indoor:
                            item.PositionID = 10;
                            break;
                        case ItemPosition.Outdoor:
                            item.PositionID = 20;
                            break;
                        default:
                            item.PositionID = 0;
                            break;
                    }

                    int requirementID = 10;
                    FW_JOB_TYPE jobType = _db.FW_JOB_TYPE.Where(x => x.JOB_TYPE.Trim() == requirement.Trim()).FirstOrDefault();
                    if (jobType != null)
                    {
                        requirementID = jobType.TYPE_ID;
                    }
                    else
                    {
                        LogMethods.Log.Warn("UpdateEstItem:Warn:" + "Requirement of " + requirement + " doesn't exist on FW_JOB_TYPE table.");
                    }
                    item.RequirementID = requirementID;

                    if (quality != null)
                    {
                        item.Qty = Convert.ToInt32(quality);
                    }
                    else
                    {
                        item.Qty = 1;
                    }

                    item.PriceA = itemCost;
                    item.IsValidated = true;

                    _db.Entry(item).State = EntityState.Modified;
                    _db.SaveChanges();

                    LogMethods.Log.Debug("UpdateEstItem:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("UpdateEstItem:Error:" + e.Message);
            }

        }

    }
}
