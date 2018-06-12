using MISService.Method;
using MyCommon.MyEnum;
using SalesCenterDomain.BDL.Project;
using SalesCenterDomain.BDL.Quote;
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
    public class QuoteMethods
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        private EndpointAddress apiAddr;
        private enterprise.SessionHeader header;

        public QuoteMethods()
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
        }

        public void GetAllQuote(string sfProjectID, int jobID, int estRevID, int userEmployeeID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Name FROM Quote where OpportunityId = '" + sfProjectID + "'";

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
                    IEnumerable<enterprise.Quote> quoteList = result.records.Cast<enterprise.Quote>();

                    foreach (var ql in quoteList)
                    {
                        var qt = new QuoteTitleGenerate(jobID, estRevID);
                        qt.Generate();

                        //Check status
                        int vid  = qt.ValidationErrorID;
                        if(vid == 0) {
                            var ps = new ProjectStatus(jobID);
                            ps.ChnageTo((int)NJobStatus.qProcessing, userEmployeeID);
                        }
                    }
                    LogMethods.Log.Debug("GetAllQuote:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllQuote:Error:" + e.Message);
            }
        }


    }
}
