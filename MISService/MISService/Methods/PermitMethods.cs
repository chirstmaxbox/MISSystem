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

        public PermitMethods()
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
        }

        public void GetAllSignPermits(string sfProjectID, int jobID, int userEmployeeID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Number_of_Signs__c, Project_Value_Estimated__c, Project_Name__r.Client_Site_Name__r.Installing_Company_Street__c, "
                        + " Project_Name__r.Client_Site_Name__r.Installing_Company_City__c, Project_Name__r.Client_Site_Name__r.Installing_Company_Province__c, "
                        + " Project_Name__r.Client_Site_Name__r.Installing_Company_Postal_Code__c, Remarks__c, Issue_Date__c, Due_Date__c, "
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
                        /* check if the quote exists */
                        int sign_permitID = CommonMethods.GetMISID(TableName.PermitForSignPermit, sp.Id);
                        if (sign_permitID == 0)
                        {
                            CreatePermit cpa = new CreatePermit(userEmployeeID, jobID, 10, 0);
                            cpa.Create();
                            int id = cpa.NewlyInsertedID;
                            CommonMethods.InsertToMISSalesForceMapping(TableName.PermitForSignPermit, sp.Id, id.ToString());
                            sign_permitID = id;
                        }

                        if (sign_permitID != 0)
                        {
                            HandleLanlordData();



                            UpdateSignPermit(sign_permitID);
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

        private void HandleLandlord(string sfLandlordID, string addr, string city, string state, string zipCode)
        {
            int landlordID = CommonMethods.GetMISID(TableName.PermitLandlord, sfLandlordID);
            if (landlordID == 0)
            {
                // insert to PermitLandlord
                PermitLandlord pl = new PermitLandlord();
                pl.ADDR_1 = addr;
                pl.CITY = city;
                pl.STATE = state;
                pl.ZIPCODE = zipCode;

                _db.PermitLandlords.Add(pl);
                _db.SaveChanges();

                int id = SqlCommon.GetNewlyInsertedRecordID(TableName.PermitLandlord);
                CommonMethods.InsertToMISSalesForceMapping(TableName.PermitLandlord, sfLandlordID, id.ToString());
            }
            else
            {
                var item = _db.PermitLandlords.Where(x => x.ROWID == landlordID).FirstOrDefault();
            }



        }

        private void UpdateSignPermit(int sign_permitID)
        {
            var item = _db.PermitForSignPermits.Where(x => x.AppID == sign_permitID).FirstOrDefault();
            if (item != null)
            {
                _db.la
            }
        }
    }
}
