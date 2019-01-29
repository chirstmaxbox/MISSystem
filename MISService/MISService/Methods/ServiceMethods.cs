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
        private string salesForceProjectID;

        public ServiceMethods(string salesForceProjectID)
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
            this.salesForceProjectID = salesForceProjectID;
        }

        public void GetAllServices(string sfEstimation, int estRevID, enterprise.QueryResult result)
        {
            try
            {
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    if (result == null || (result != null && result.size == 0)) return;

                    IEnumerable<enterprise.Service_Cost__c> serviceList = result.records.Cast<enterprise.Service_Cost__c>();
                    var svc = new MyEstServiceCreate(estRevID);
                    List<string> services = new List<string>();
                    foreach (var sl in serviceList)
                    {
                        services.Add(sl.Id);
                        long estServiceID = CommonMethods.GetMISID(TableName.Est_Service, sl.Id, sfEstimation, salesForceProjectID);
                        int printOrder = svc.GetQsMaxPrintOrder() + 1;
                        if (estServiceID == 0)
                        {
                            // new service
                            if (sl.Service_Cost__c1 != null && sl.Service_Cost__c1 > 0)
                            {
                                svc.InsertRecord(Convert.ToInt32(sl.Service_Name__r.MIS_Service_Number__c),
                                     "$" + sl.Service_Cost__c1.ToString(),
                                     1,
                                     sl.Service_Detail__c == null ? "" : sl.Service_Detail__c,
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
                                     sl.Service_Detail__c == null ? "" : sl.Service_Detail__c,
                                     sl.Service_Name__r.Name,
                                     sl.Note__c,
                                     printOrder
                                );
                            }
                            int qs_id = SqlCommon.GetNewlyInsertedRecordID(TableName.Est_Service);
                            if (qs_id > 0)
                            {
                                CommonMethods.InsertToMISSalesForceMapping(TableName.Est_Service, sl.Id, qs_id.ToString(), sfEstimation, salesForceProjectID);
                            }
                        }
                        else
                        {
                            UpdateEstService(estServiceID, sl.Service_Cost__c1, sl.Service_Detail__c, sl.Service_Name__r.Name, Convert.ToInt16(sl.Service_Name__r.MIS_Service_Number__c), sl.Note__c);
                        }
                    }

                    DeleteAllDeletedEstimationServices(services.ToArray(), sfEstimation);
                    LogMethods.Log.Debug("GetAllServices:Debug:" + "Done");
                }

            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllServices:Error:" + e.Message);
            }
        }

        private void DeleteAllDeletedEstimationServices(string[] services, string sfEstimation)
        {
            try
            {
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.Est_Service, sfEstimation, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(services, i) == -1)
                    {
                        // get MISID
                        int serviceIDTemp = CommonMethods.GetMISID(TableName.Est_Service, i, sfEstimation, salesForceProjectID);
                        // get a row
                        var serviceItem = _db.EST_Service.Where(x => x.qsID == serviceIDTemp).FirstOrDefault();
                        if (serviceItem != null)
                        {
                            _db.EST_Service.Remove(serviceItem);
                            _db.SaveChanges();
                        }
                        // remove MISID out of MISSalesForceMapping
                        CommonMethods.Delete(TableName.Est_Service, i, sfEstimation, salesForceProjectID);
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedEstimationServices:Error:" + e.Message);
            }
        }

        private void UpdateEstService(long estServiceID, double? cost, string detail, string name, short qsServiceID, string note)
        {
            try
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

                    LogMethods.Log.Debug("UpdateEstService:Debug:" + "Done");

                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("UpdateEstService:Error:" + e.Message);
            }
        }

    }
}
