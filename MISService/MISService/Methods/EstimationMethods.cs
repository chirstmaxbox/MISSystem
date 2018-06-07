using CustomerDomain.Model;
using MISService.Method;
using MISService.Models;
using SpecDomain.BLL.EstItem;
using SpecDomain.Model;
using System;
using System.Collections.Generic;
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

        public EstimationMethods()
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
        }

        public void GetEstimation(string sfProjectID, int estRevID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query =  "SELECT Id, Name FROM Estimation__c where Project_Name__c = '" + sfProjectID + "'";

                    enterprise.QueryResult result;
                    queryClient.query(
                        header, //sessionheader
                        null, //queryoptions
                        null, //mruheader
                        null, //packageversion
                        query, out result);

                    //cast query results
                    IEnumerable<enterprise.Estimation__c> estimationList = result.records.Cast<enterprise.Estimation__c>();

                    //show results
                    foreach (var el in estimationList)
                    {
                        GetAllItems(el.Id, estRevID);
                    }
                    LogMethods.Log.Debug("GetAllCompanies:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllCompanies:Error:" + e.Message);
            }
        }


        public void GetAllItems(string sfEstimation, int estRevID) 
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Name, Category__c, Sign_Type__c, Feature_1__c, Feature_2__c, Graphic__c, Item_Name__c FROM Item__c where Estimation_Name__c = '" + sfEstimation + "'";

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
                    foreach (var il in itemList)
                    {
                        long estItemID = CommonMethods.GetMISID(TableName.EST_Item, il.Id);
                        if (estItemID == 0)
                        {
                            int productID = 0;
                            Product optionDetails = _db.Products.Where(x => x.ProductName.Trim() == il.Sign_Type__c & x.Active).FirstOrDefault();
                            if (optionDetails != null)
                            {
                                productID = optionDetails.ProductID;
                            }
                            var est = new MyEstItemCreate(estRevID, productID, il.Item_Name__c);
                            CommonMethods.InsertToMISSalesForceMapping(TableName.EST_Item, il.Id, est.EstItemID.ToString());
                            estItemID = est.EstItemID;
                        }

                        UpdateEstItem(estItemID);
                    }
                    LogMethods.Log.Debug("GetAllCompanies:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllCompanies:Error:" + e.Message);
            }
        }

        private void UpdateEstItem(long estItemID)
        {
            
        }
    }
}
