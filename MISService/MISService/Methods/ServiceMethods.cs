using MISService.Method;
using MISService.Models;
using SalesCenterDomain.BDL;
using SpecDomain.BLL.Service;
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
    public class ServiceMethods
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        private EndpointAddress apiAddr;
        private enterprise.SessionHeader header;

        public ServiceMethods()
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
        }

        public void GetAllServices(string sfEstimation, int estRevID)
        {
            try
            {
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Service_Name__r.Name, Detail__c, Service_Cost__c, Note__c, Service_Name__r.MIS_Service_Number__c FROM Service_Cost__c where Estimation_Name__c = '" + sfEstimation + "'";

                    enterprise.QueryResult result;
                    queryClient.query(
                        header,
                        null,
                        null,
                        null,
                        query, out result);

                    /* if no any record, return */
                    if (result.size == 0) return;

                    IEnumerable<enterprise.Service_Cost__c> serviceList = result.records.Cast<enterprise.Service_Cost__c>();
                    var svc = new MyEstServiceCreate(estRevID);
                    foreach (var sl in serviceList)
                    {
                        long estServiceID = CommonMethods.GetMISID(TableName.Est_Service, sl.Id);
                        int printOrder = svc.GetQsMaxPrintOrder() + 1;
                        if (estServiceID == 0)
                        {
                            // new service
                            if (sl.Service_Cost__c1 != null && sl.Service_Cost__c1 > 0)
                            {
                                svc.InsertRecord(Convert.ToInt32(sl.Service_Name__r.MIS_Service_Number__c),
                                     "$" + sl.Service_Cost__c1.ToString(),
                                     1,
                                     sl.Detail__c == null ? "" : sl.Detail__c,
                                     sl.Service_Name__r.Name,
                                     "$" + sl.Service_Cost__c1.ToString(),
                                     printOrder
                                );
                            }
                            else
                            {
                                svc.InsertRecord(Convert.ToInt32(sl.Service_Name__r.MIS_Service_Number__c),
                                     sl.Note__c,
                                     1,
                                     sl.Detail__c == null ? "" : sl.Detail__c,
                                     sl.Service_Name__r.Name,
                                     sl.Note__c,
                                     printOrder
                                );
                            }
                            int qs_id = SqlCommon.GetNewlyInsertedRecordID(TableName.Est_Service);
                            CommonMethods.InsertToMISSalesForceMapping(TableName.Est_Service, sl.Id, qs_id.ToString());
                        }
                        else
                        {
                            UpdateEstService(estServiceID, sl.Service_Cost__c1, sl.Detail__c, sl.Service_Name__r.Name, Convert.ToInt16(sl.Service_Name__r.MIS_Service_Number__c), sl.Note__c);
                        }

                        LogMethods.Log.Debug("GetAllServices:Debug:" + "Done");
                    }
                }

            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllServices:Error:" + e.Message);
            }
        }

        private void UpdateEstService(long estServiceID, double? cost, string detail, string name, short qsServiceID, string note)
        {
            var service = _db.EST_Service.Find(estServiceID);
            if (service != null)
            {
                if (cost != null && cost > 0)
                {
                    service.qsAmount = "$" + cost.ToString();
                    service.qsAmountText = "$" + cost.ToString();
                }
                else
                {
                    service.qsAmount = note;
                    service.qsAmountText = note;
                }

                service.qsTitle = name;
                service.qsDescription = (detail == null ? "" : detail);
                service.qsServiceID = qsServiceID;

                _db.Entry(service).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }

    }
}
