using MISService.Method;
using MISService.Models;
using ProjectDomain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using enterprise = MISService.SfdcReference;

namespace MISService.Methods
{
    public class ProductionWOMethods
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private EndpointAddress apiAddr;
        private enterprise.SessionHeader header;
        private string salesForceProjectID;

        public ProductionWOMethods(string salesForceProjectID)
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
            this.salesForceProjectID = salesForceProjectID;
        }

        public void GetAllWorkShopInstruction(int woId, string sfWorkOrderID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Category__c, Final_Instruction__c, Instruction__c "
                        + " FROM WorkShop_Instruction__c where Work_Order_Name__c = '" + sfWorkOrderID + "'";

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
                    IEnumerable<enterprise.WorkShop_Instruction__c> workShopList = result.records.Cast<enterprise.WorkShop_Instruction__c>();

                    foreach (var wl in workShopList)
                    {
                        /* check if the work order exists */
                        int workShopID = CommonMethods.GetMISID(TableName.WO_Instruction_DataTable, wl.Id, sfWorkOrderID, salesForceProjectID);
                        if (workShopID == 0)
                        {

                        }

                    }
                    LogMethods.Log.Debug("GetAllWorkShopInstruction:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllWorkShopInstruction:Error:" + e.Message);
            }
        }

        public bool InsertWorkShopInstruction(int woID, string category, string content, string finalContent)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlSelectString = "INSERT INTO WO_Instruction_Datatable(woID, InstructionID, Contents, recordType) VALUES (@woID, @InstructionID, @Contents, @recordType)";
                var InsertCommand = new SqlCommand(SqlSelectString, Connection);
                InsertCommand.Parameters.AddWithValue("@woID", woID);
                InsertCommand.Parameters.AddWithValue("@InstructionID", GetInstructionID(category, content));
                InsertCommand.Parameters.AddWithValue("@Contents", finalContent);
                InsertCommand.Parameters.AddWithValue("@recordType", 1); //1 for workship instruction, 0 for Instruction for Installer and Site...
                Connection.Open();
                InsertCommand.ExecuteNonQuery();
                ret = true;
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("InsertToMISSalesForceMapping:Crash:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

        public int GetInstructionID(string category, string content)
        {
            int id = 0;
            switch(category) {
                case "Material":
                    switch (content)
                    {
                        case "Long Time Material to order immediately: Material and Lead-time: < insert your content here >":
                            id = 1076;
                            break;
                        default:
                            id = 1077;
                            break;
                    }
                    break;
                case "Others":
                    id = 1100;
                    break;
                case "Product Specific":
                    switch (content)
                    {
                        case "Carrier box to be determined":
                            id = 1078;
                            break;
                        case "Length of rod to be confirmed":
                            id = 1088;
                            break;
                        case "Shipping: There is an existing plastic panel, supply vinyl graphic only":
                            id = 1083;
                            break;
                        case "Supply and Install: Take down and bring back the plastic panel to apply the vinyl graphic at FS shop. Install the panel back to the sign upon completion":
                            id = 1084;
                            break;
                        case "Supply and Install: There is existing plastic panel. FS install vinyl graphic on-site":
                            id = 1085;
                            break;
                        case "The finished product to be installed on the existing sign":
                            id = 1079;
                            break;
                        case "Step-1: The panel to be supplied by client; Client will deliver to FS shop on < insert your content here >. Step 2: FS subject to install the sign/letters on the panel. Step 3: Client visit FS to pick up the finished panel":
                            id = 1102;
                            break;
                        case "Step-1: The panel to be supplied by client; Client will deliver to FS shop on < insert your content here >. Step 2: FS subject to install the sign/letters on the panel. Step 3: FS to install the panel on-site":
                            id = 1102;
                            break;
                        case "There is existing plastic panel. FS install vinyl graphic on-site":
                            id = 1082;
                            break;
                        case "Threaded rod installation is required. Client will close ceiling on < insert your content here >. Please install threaded rod prior to ceiling completion":
                            id = 1086;
                            break;
                        default:
                            id = 1087;
                            break;
                    }
                    break;
                case "Upon Job Completion":
                    switch (content)
                    {
                        case "Please contact Sales Team upon fabrication completion":
                            id = 1089;
                            break;
                        case "Please contact Sales Team upon sample completion":
                            id = 1093;
                            break;
                        case "Please pack and contact Sales Team when product is ready for pick up by client":
                            id = 1090;
                            break;
                        case "Please securely crate/pack the order for shipping. Please contact Purchasing Department when ready for shipment":
                            id = 1096;
                            break;
                        case "Please securely crate/pack the order for shipping. Please contact Sales Team when ready":
                            id = 1094;
                            break;
                        case "Please ship this work order with W#< insert your content here >":
                            id = 1097;
                            break;
                        case "Product review is required by client. Please contact Sales Team when it is ready for product review":
                            id = 1091;
                            break;
                        default:
                            id = 1092;
                            break;
                    }
                    break;
                case "Workmanship":
                    switch (content)
                    {
                        case "Back of the sign is visible by public, must be finished in good shape":
                            id = 1098;
                            break;
                        default:
                            id = 1099;
                            break;
                    }
                    break;
                default:
                    break;

            }

            return id;
            
        }

    }
}
