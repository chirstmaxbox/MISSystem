using MISService.Method;
using MISService.Models;
using PermitDomain.BLL;
using PermitDomain.Model;
using ProjectDomain;
using SalesCenterDomain.BDL;
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
    public class PermitMethods
    {
        private readonly PermitDbEntities _db = new PermitDbEntities();
        private EndpointAddress apiAddr;
        private enterprise.SessionHeader header;
        private string salesForceProjectID;

        public PermitMethods(string salesForceProjectID)
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
            this.salesForceProjectID = salesForceProjectID;
        }

        public void GetAllSignPermits(string sfProjectID, int jobID, int userEmployeeID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Number_of_Signs__c, Project_Value_Estimated__c, "
                        + " Remarks__c, Issue_Date__c, Due_Date__c, "
                        + " LandLord_Name__c, LandLord_Name__r.Name, LandLord_Phone_Number__c, LandLord_Name__r.Street__c, LandLord_Name__r.City__c, "
                        + " LandLord_Name__r.Province__c, LandLord_Name__r.Postal_Code__c "
                        + " FROM Sign_Permit__c where Project_Name__c = '" + sfProjectID + "'";

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
                    IEnumerable<enterprise.Sign_Permit__c> signPermitList = result.records.Cast<enterprise.Sign_Permit__c>();

                    foreach (var sp in signPermitList)
                    {
                        /* check if the sign permit exists */
                        int sign_permitID = CommonMethods.GetMISID(TableName.PermitForSignPermit, sp.Id, salesForceProjectID);
                        if (sign_permitID == 0)
                        {
                            CreatePermit cpa = new CreatePermit(userEmployeeID, jobID, 10, 0);
                            cpa.Create();
                            int id = cpa.NewlyInsertedID;
                            CommonMethods.InsertToMISSalesForceMapping(TableName.PermitForSignPermit, sp.Id, id.ToString(), salesForceProjectID);
                            sign_permitID = id;
                        }

                        if (sign_permitID != 0)
                        {
                            HandleLandlord(sp.Id, sp.LandLord_Name__r.Street__c, sp.LandLord_Name__r.City__c, sp.LandLord_Name__r.Province__c, sp.LandLord_Name__r.Postal_Code__c,
                                sp.LandLord_Name__r.Name, sp.LandLord_Phone_Number__c);

                            UpdateSignPermit(sp.Id, sign_permitID, sp.Number_of_Signs__c, sp.Project_Value_Estimated__c, sp.Issue_Date__c, sp.Due_Date__c, sp.Remarks__c);
                        }

                    }
                    LogMethods.Log.Debug("GetAllSignPermits:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllSignPermits:Error:" + e.Message);
            }
        }

        private void HandleLandlord(string sfLandlordID, string addr, string city, string state, string zipCode, string landlordName, string phone)
        {
            try
            {
                int landlordID = CommonMethods.GetMISID(TableName.PermitLandlord, sfLandlordID, salesForceProjectID);
                if (landlordID == 0)
                {
                    // insert to PermitLandlord
                    PermitLandlord pl = new PermitLandlord();
                    pl.NAME = "New Location";
                    pl.ADDR_1 = addr;
                    pl.CITY = city;
                    pl.STATE = state;
                    pl.ZIPCODE = zipCode;
                    pl.Active = true;

                    _db.PermitLandlords.Add(pl);
                    _db.SaveChanges();

                    int id = SqlCommon.GetNewlyInsertedRecordID(TableName.PermitLandlord);
                    landlordID = id;
                    CommonMethods.InsertToMISSalesForceMapping(TableName.PermitLandlord, sfLandlordID, id.ToString(), salesForceProjectID);
                }
                else
                {
                    var item = _db.PermitLandlords.Where(x => x.ROWID == landlordID).FirstOrDefault();
                    if (item != null)
                    {
                        item.ADDR_1 = addr;
                        item.CITY = city;
                        item.STATE = state;
                        item.ZIPCODE = zipCode;

                        _db.Entry(item).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }

                /* landlord contact */
                int landlordContactID = CommonMethods.GetMISID(TableName.PermitLandlordContact, sfLandlordID, salesForceProjectID);
                if (landlordContactID == 0)
                {
                    /* add landloard contact */
                    PermitLandlordContact plc = new PermitLandlordContact();
                    plc.ROWID = landlordID;
                    plc.CONTACT_FIRST_NAME = landlordName;
                    plc.CONTACT_PHONE = phone;

                    _db.PermitLandlordContacts.Add(plc);
                    _db.SaveChanges();

                    int id = SqlCommon.GetNewlyInsertedRecordID(TableName.PermitLandlordContact);
                    CommonMethods.InsertToMISSalesForceMapping(TableName.PermitLandlordContact, sfLandlordID, id.ToString(), salesForceProjectID);
                }
                else
                {
                    var item = _db.PermitLandlordContacts.Where(x => x.CONTACT_ID == landlordContactID).FirstOrDefault();
                    if (item != null)
                    {
                        item.CONTACT_FIRST_NAME = landlordName;
                        item.CONTACT_PHONE = phone;

                        _db.Entry(item).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("HandleLandLord:Error:" + e.Message);
            }
        }

        private void UpdateSignPermit(string sfLandlordID, int sign_permitID, double? numOfSigns, double? proValueEstimated, DateTime? issueDate, DateTime? dueDate, string remarks)
        {
            try
            {
                int landlordID = CommonMethods.GetMISID(TableName.PermitLandlord, sfLandlordID, salesForceProjectID);
                int landlordContactID = CommonMethods.GetMISID(TableName.PermitLandlordContact, sfLandlordID, salesForceProjectID);

                var item = _db.PermitForSignPermits.Where(x => x.AppID == sign_permitID).FirstOrDefault();
                if (item != null)
                {
                    if (numOfSigns != null)
                    {
                        item.NumberOfSigns = numOfSigns;
                    }
                    if (proValueEstimated != null)
                    {
                        item.ProjectValueEstimated = proValueEstimated;
                    }
                    if (landlordID != 0)
                    {
                        item.LandlordID = landlordID;
                    }
                    if (landlordContactID != 0)
                    {
                        item.LandlordContactID = landlordContactID;
                    }

                    _db.Entry(item).State = EntityState.Modified;
                    _db.SaveChanges();

                    var baseItem = _db.PermitBases.Where(x => x.BaseAppID == item.BaseAppID).FirstOrDefault();
                    if (baseItem != null)
                    {
                        if (issueDate != null)
                        {
                            baseItem.RequestDate = (DateTime)issueDate;
                        }
                        if (dueDate != null)
                        {
                            baseItem.Deadline = (DateTime)dueDate;
                        }
                        baseItem.Remark = remarks;

                        _db.Entry(baseItem).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("UpdateSignPermit:Error:" + e.Message);
            }
        }

        public void GetAllHoistingPermits(string sfProjectID, int jobID, int userEmployeeID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Occupation_Start_Time__c, Occupation_End_Time__c, Issue_Date__c, Type_Of_Truck__c, "
                        + " Truck_Weight__c, Foreman_Name__c, Foreman_Phone__c, Remarks__c "
                        + " FROM Hoisting_Permit__c where Project_Name__c = '" + sfProjectID + "'";

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
                    IEnumerable<enterprise.Hoisting_Permit__c> hoistingPermitList = result.records.Cast<enterprise.Hoisting_Permit__c>();

                    foreach (var sp in hoistingPermitList)
                    {
                        int hoisting_permitID = CommonMethods.GetMISID(TableName.PermitForHoisting, sp.Id, salesForceProjectID);
                        if (hoisting_permitID == 0)
                        {
                            CreatePermit cpa = new CreatePermit(userEmployeeID, jobID, 30, 0);
                            cpa.Create();
                            int id = cpa.NewlyInsertedID;
                            CommonMethods.InsertToMISSalesForceMapping(TableName.PermitForHoisting, sp.Id, id.ToString(), salesForceProjectID);
                            hoisting_permitID = id;
                        }

                        if (hoisting_permitID != 0)
                        {
                            UpdateHoistingPermit(hoisting_permitID, sp.Issue_Date__c, sp.Occupation_Start_Time__c, sp.Occupation_End_Time__c, sp.Type_Of_Truck__c,
                                sp.Truck_Weight__c, sp.Foreman_Name__c, sp.Foreman_Phone__c, sp.Remarks__c);
                        }

                    }
                    LogMethods.Log.Debug("GetAllHoistingPermits:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllHoistingPermits:Error:" + e.Message);
            }
        }

        private void UpdateHoistingPermit(int hoist_PermitID, DateTime? issueDate, DateTime? startTime, DateTime? endTime, string truckType, string weight,
            string foremanName, string foremanPhone, string remark)
        {
            try
            {
                var item = _db.PermitForHoistings.Where(x => x.AppID == hoist_PermitID).FirstOrDefault();
                if (item != null)
                {
                    if (startTime != null)
                    {
                        var localTime = startTime.Value.ToLocalTime();
                        item.OccupationTimeStart = localTime.ToString("hh:mm tt");
                    }
                    if (endTime != null)
                    {
                        var localTime = endTime.Value.ToLocalTime();
                        item.OccupationTimeEnd = localTime.ToString("hh:mm tt");
                    }

                    switch (truckType)
                    {
                        case "Boom Truck 40'":
                            item.TypeOfTruck = 3;
                            break;
                        case "Boom Truck 50'":
                            item.TypeOfTruck = 4;
                            break;
                        case "Boom Truck 85'":
                            item.TypeOfTruck = 5;
                            break;
                        case "Lader Van":
                            item.TypeOfTruck = 2;
                            break;
                        case "Pickup Truck":
                            item.TypeOfTruck = 1;
                            break;
                        case "Scissor Lift Only":
                            item.TypeOfTruck = 6;
                            break;
                        default:
                            item.TypeOfTruck = 0;
                            break;
                    }

                    item.Tonnage = weight;
                    item.ForemanName = foremanName;
                    item.ForemanPhone = foremanPhone;

                    _db.Entry(item).State = EntityState.Modified;
                    _db.SaveChanges();

                    var baseItem = _db.PermitBases.Where(x => x.BaseAppID == item.BaseAppID).FirstOrDefault();
                    if (baseItem != null)
                    {
                        if (issueDate != null)
                        {
                            if (issueDate != null)
                            {
                                baseItem.RequestDate = (DateTime)issueDate;
                            }
                        }

                        if (endTime != null)
                        {
                            baseItem.Deadline = endTime.Value.ToLocalTime();
                        }

                        baseItem.Remark = remark;

                        _db.Entry(baseItem).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("UpdateHoistingPermit:Error:" + e.Message);
            }
        }


        public void GetAllStakeOutPermits(string sfProjectID, int jobID, int userEmployeeID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Stick_Position_Radius__c, Dept_Of_Holes__c, Issue_Date__c, Due_Date__c, Remarks__c "
                        + " FROM StakeOut_Permit__c where Project_Name__c = '" + sfProjectID + "'";

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
                    IEnumerable<enterprise.StakeOut_Permit__c> stakeoutPermitList = result.records.Cast<enterprise.StakeOut_Permit__c>();

                    foreach (var sp in stakeoutPermitList)
                    {
                        int stakeout_permitID = CommonMethods.GetMISID(TableName.PermitForStakeout, sp.Id, salesForceProjectID);
                        if (stakeout_permitID == 0)
                        {
                            CreatePermit cpa = new CreatePermit(userEmployeeID, jobID, 20, 0);
                            cpa.Create();
                            int id = cpa.NewlyInsertedID;
                            CommonMethods.InsertToMISSalesForceMapping(TableName.PermitForStakeout, sp.Id, id.ToString(), salesForceProjectID);
                            stakeout_permitID = id;
                        }

                        if (stakeout_permitID != 0)
                        {
                            UpdateStakeOutPermit(stakeout_permitID, sp.Stick_Position_Radius__c, sp.Dept_Of_Holes__c, sp.Issue_Date__c,
                                sp.Due_Date__c, sp.Remarks__c);
                        }

                    }
                    LogMethods.Log.Debug("GetAllStakeOutPermits:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllStakeOutPermits:Error:" + e.Message);
            }
        }

        private void UpdateStakeOutPermit(int stakeout_permitID, string Stick_Position_Radius, string Dept_Of_Holes, DateTime? issueDate, 
            DateTime? dueDate, string remark)
        {
            try
            {
                var item = _db.PermitForStakeouts.Where(x => x.AppID == stakeout_permitID).FirstOrDefault();
                if (item != null)
                {
                    item.DeptOfHoles = Dept_Of_Holes;
                    item.WayofPointLocation = Stick_Position_Radius;

                    _db.Entry(item).State = EntityState.Modified;
                    _db.SaveChanges();

                    var baseItem = _db.PermitBases.Where(x => x.BaseAppID == item.BaseAppID).FirstOrDefault();
                    if (baseItem != null)
                    {
                        if (issueDate != null)
                        {
                            baseItem.RequestDate = (DateTime)issueDate;
                        }
                        if (dueDate != null)
                        {
                            baseItem.Deadline = (DateTime)dueDate;
                        }
                        baseItem.Remark = remark;

                        _db.Entry(baseItem).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("UpdateStakeOutPermit:Error:" + e.Message);
            }
        }

    }
}
