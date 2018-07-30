using MISService.Method;
using MISService.Models;
using ProjectDomain;
using SalesCenterDomain.BDL;
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

        public void GetAllWorkShopInstructions(int woId, string sfWorkOrderID)
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
                    List<string> items = new List<string>();
                    foreach (var wl in workShopList)
                    {
                        items.Add(wl.Id);
                        /* check if the work order exists */
                        int workShopID = CommonMethods.GetMISID(TableName.WO_Instruction_DataTable_PW, wl.Id, sfWorkOrderID, salesForceProjectID);
                        if (workShopID == 0)
                        {
                            InsertWorkShopInstruction(woId, wl.Category__c, wl.Instruction__c, wl.Final_Instruction__c);
                            int newId = SqlCommon.GetNewlyInsertedRecordID(TableName.WO_Instruction_DataTable);
                            if (newId > 0)
                            {
                                CommonMethods.InsertToMISSalesForceMapping(TableName.WO_Instruction_DataTable_PW, wl.Id, newId.ToString(), sfWorkOrderID, salesForceProjectID);
                            }
                        }
                        else
                        {
                            UpdateWorkShopInstruction(workShopID, wl.Category__c, wl.Instruction__c, wl.Final_Instruction__c);
                        }
                    }

                    DeleteAllDeletedWorkShopInstructions(items.ToArray(), sfWorkOrderID);
                    LogMethods.Log.Debug("GetAllWorkShopInstruction:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllWorkShopInstruction:Error:" + e.Message);
            }
        }

        private int DeleteWorkShopInstruction(int dId)
        {
            int row = 0;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                Connection.Open();
                string SqlDelString = "DELETE FROM WO_Instruction_DataTable WHERE ([dID] = @dID) and ([recordType] = @recordType)";
                var DelCommand = new SqlCommand(SqlDelString, Connection);
                DelCommand.Parameters.AddWithValue("@dID", dId);
                DelCommand.Parameters.AddWithValue("@recordType", 1);
                row = DelCommand.ExecuteNonQuery();
                Connection.Close();
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("DeleteWorkShopInstruction:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return row;
        }

        private void DeleteAllDeletedWorkShopInstructions(string[] items, string sfWorkOrderID)
        {
            try
            {
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.WO_Instruction_DataTable_PW, sfWorkOrderID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(items, i) == -1)
                    {
                        // get MISID
                        int itemIDTemp = CommonMethods.GetMISID(TableName.WO_Instruction_DataTable_PW, i, sfWorkOrderID, salesForceProjectID);
                        // get a row
                        if (DeleteWorkShopInstruction(itemIDTemp) > 0)
                        {
                            // remove MISID out of MISSalesForceMapping
                            CommonMethods.Delete(TableName.WO_Instruction_DataTable_PW, i, sfWorkOrderID, salesForceProjectID);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedWorkShopInstructions:Error:" + e.Message);
            }
        }

        private bool InsertWorkShopInstruction(int woID, string category, string content, string finalContent)
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
                LogMethods.Log.Error("InsertWorkShopInstruction:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

        private bool UpdateWorkShopInstruction(int dID, string category, string content, string finalContent)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlUpdateString = "UPDATE [WO_Instruction_Datatable] SET  [Contents] = @Contents, [InstructionID] = @InstructionID WHERE [dID] = @dID";
                var UpdateCommand = new SqlCommand(SqlUpdateString, Connection);
                UpdateCommand.Parameters.AddWithValue("@dID", dID);
                UpdateCommand.Parameters.AddWithValue("@InstructionID", GetInstructionID(category, content));
                UpdateCommand.Parameters.AddWithValue("@Contents", finalContent);
                Connection.Open();
                UpdateCommand.ExecuteNonQuery();
                ret = true;
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("UpdateWorkShopInstruction:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

        private int GetInstructionID(string category, string content)
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
                default:
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
            }

            return id;
        }

        public void GetAllInstallerInstructions(int woId, string sfWorkOrderID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Category__c, Final_Instruction__c, Instruction__c "
                        + " FROM Installer_Instruction__c where Work_Order_Name__c = '" + sfWorkOrderID + "'";

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
                    IEnumerable<enterprise.Installer_Instruction__c> instInstructionList = result.records.Cast<enterprise.Installer_Instruction__c>();
                    List<string> items = new List<string>();
                    foreach (var wl in instInstructionList)
                    {
                        items.Add(wl.Id);
                        /* check if the work order exists */
                        int instInstructionID = CommonMethods.GetMISID(TableName.WO_Instruction_DataTable_PI, wl.Id, sfWorkOrderID, salesForceProjectID);
                        if (instInstructionID == 0)
                        {
                            InsertInstallerInstruction(woId, wl.Category__c, wl.Instruction__c, wl.Final_Instruction__c);
                            int newId = SqlCommon.GetNewlyInsertedRecordID(TableName.WO_Instruction_DataTable);
                            if (newId > 0)
                            {
                                CommonMethods.InsertToMISSalesForceMapping(TableName.WO_Instruction_DataTable_PI, wl.Id, newId.ToString(), sfWorkOrderID, salesForceProjectID);
                            }
                        }
                        else
                        {
                            UpdateInstallerInstruction(instInstructionID, wl.Category__c, wl.Instruction__c, wl.Final_Instruction__c);
                        }
                    }

                    DeleteAllDeletedInstallerInstructions(items.ToArray(), sfWorkOrderID);
                    LogMethods.Log.Debug("GetAllInstallerInstructions:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllInstallerInstructions:Error:" + e.Message);
            }
        }

        private int DeleteInstallerInstruction(int dId)
        {
            int row = 0;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                Connection.Open();
                string SqlDelString = "DELETE FROM WO_Instruction_DataTable WHERE ([dID] = @dID) and ([recordType] = @recordType)";
                var DelCommand = new SqlCommand(SqlDelString, Connection);
                DelCommand.Parameters.AddWithValue("@dID", dId);
                DelCommand.Parameters.AddWithValue("@recordType", 0);
                row = DelCommand.ExecuteNonQuery();
                Connection.Close();
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("DeleteInstallerInstruction:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return row;
        }

        private void DeleteAllDeletedInstallerInstructions(string[] items, string sfWorkOrderID)
        {
            try
            {
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.WO_Instruction_DataTable_PI, sfWorkOrderID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(items, i) == -1)
                    {
                        // get MISID
                        int itemIDTemp = CommonMethods.GetMISID(TableName.WO_Instruction_DataTable_PI, i, sfWorkOrderID, salesForceProjectID);
                        // get a row
                        if (DeleteInstallerInstruction(itemIDTemp) > 0)
                        {
                            // remove MISID out of MISSalesForceMapping
                            CommonMethods.Delete(TableName.WO_Instruction_DataTable_PI, i, sfWorkOrderID, salesForceProjectID);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedInstallerInstructions:Error:" + e.Message);
            }
        }


        private bool UpdateInstallerInstruction(int dID, string category, string content, string finalContent)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlUpdateString = "UPDATE [WO_Instruction_Datatable] SET  [Contents] = @Contents, [InstructionID] = @InstructionID WHERE [dID] = @dID";
                var UpdateCommand = new SqlCommand(SqlUpdateString, Connection);
                UpdateCommand.Parameters.AddWithValue("@dID", dID);
                UpdateCommand.Parameters.AddWithValue("@InstructionID", GetInstallerInstructionID(category, content));
                UpdateCommand.Parameters.AddWithValue("@Contents", finalContent);
                Connection.Open();
                UpdateCommand.ExecuteNonQuery();
                ret = true;
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("UpdateInstallerInstruction:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

        private bool InsertInstallerInstruction(int woID, string category, string content, string finalContent)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlSelectString = "INSERT INTO WO_Instruction_Datatable(woID, InstructionID, Contents, recordType) VALUES (@woID, @InstructionID, @Contents, @recordType)";
                var InsertCommand = new SqlCommand(SqlSelectString, Connection);
                InsertCommand.Parameters.AddWithValue("@woID", woID);
                InsertCommand.Parameters.AddWithValue("@InstructionID", GetInstallerInstructionID(category, content));
                InsertCommand.Parameters.AddWithValue("@Contents", finalContent);
                InsertCommand.Parameters.AddWithValue("@recordType", 0); //0 for Instruction for Installer and Site-Check inspector.
                Connection.Open();
                InsertCommand.ExecuteNonQuery();
                ret = true;
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("InsertInstallerInstruction:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

        private int GetInstallerInstructionID(string category, string content)
        {
            int id = 0;
            switch (category)
            {
                case "Additional Drawing":
                    id = 1023;
                    break;
                case "Additional Details":
                    id = 1032;
                    break;
                case "C.O.D":
                    id = 1003;
                    break;
                case "Confirm Appoint":
                    id = 1002;
                    break;
                case "Hoarding":
                    switch (content)
                    {
                        case "Hoarding is up: Key Code is < insert your content here > to get access into store. No need to meet with client":
                            id = 1005;
                            break;
                        case "Hoarding is up: Meet with client to get access into the store":
                            id = 1004;
                            break;
                        case "Hoarding is up: No special access required to get inside the store":
                            id = 1006;
                            break;
                        case "Hoarding is up: < insert your content here >":
                            id = 1007;
                            break;
                        default:
                            id = 1008;
                            break;
                    }
                    break;
                case "Install Only":
                    id = 1009;
                    break;
                case "Installation Method":
                    switch (content)
                    {
                        case "Mount on existing sign":
                            id = 1015;
                            break;
                        default:
                            id = 1014;
                            break;
                    }
                    break;
                case "Installation Time":
                    id = 1013;
                    break;
                case "Mall":
                    switch (content)
                    {
                        case "Install after mall hour - mall arrangement made, sign in with security before work. Call site supervisor upon arrival and leaving site":
                            id = 1011;
                            break;
                        case "Install before mall hour - mall arrangement made, sign in with security before work. Call site supervisor upon arrival and leaving site":
                            id = 1010;
                            break;
                        default:
                            id = 1012;
                            break;
                    }
                    break;
                case "Other":
                    id = 1033;
                    break;
                case "Pin Point":
                    switch (content)
                    {
                        case "Pin Point Location - AE to go with installer to pin point position of sign":
                            id = 1000;
                            break;
                        default:
                            id = 1001;
                            break;
                    }
                    break;
                case "Pylon Sign":
                    switch (content)
                    {
                        case "Pylon Sign - Hole Inspection - meet inspector on site at < insert your content here >":
                            id = 1017;
                            break;
                        default:
                            id = 1016;
                            break;
                    }
                    break;
                case "Relocation":
                    id = 1067;
                    break;
                case "Servicing":
                    switch (content)
                    {
                        case "Servicing - After installation completed, fix & repair the broken sign: < insert your content here >":
                            id = 1021;
                            break;
                        case "Servicing - After installation completed, fix & repair the broken sign: Replace burnt out lamp":
                            id = 1020;
                            break;
                        default:
                            id = 1019;
                            break;
                    }
                    break;
                case "Special Equipment":
                    id = 1022;
                    break;
                case "Take Down":
                    switch (content)
                    {
                        case "Patch Hole":
                            id = 1068;
                            break;
                        case "Take down & deliver to < insert your content here >":
                            id = 1074;
                            break;
                        case "Take down & repair":
                            id = 1069;
                            break;
                        case "Take down in good shape & bring back shop for repair":
                            id = 1073;
                            break;
                        case "Take down in good shape & store in FS workshop (If Requirement used: TD & Bring back to shop for stock)":
                            id = 1072;
                            break;
                        case "Take down, take picture, & leave sign at the site with client":
                            id = 1070;
                            break;
                        default:
                            id = 1071;
                            break;
                    }
                    break;
                default:
                    id = 1031;
                    break;
            }

            return id;
        }

        public void GetAllCheckLists(int woId, string sfWorkOrderID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Check_List_Item__c, Content__c, Content_For_Check_List_Item_As_Others__c "
                        + " FROM Production_Check_List__c where Work_Order_Name__c = '" + sfWorkOrderID + "'";

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
                    IEnumerable<enterprise.Production_Check_List__c> checkList = result.records.Cast<enterprise.Production_Check_List__c>();
                    List<string> items = new List<string>();
                    foreach (var wl in checkList)
                    {
                        items.Add(wl.Id);
                        int checkListID = CommonMethods.GetMISID(TableName.WO_WORKORDER_CHECKLIST_DATATABLE_PC, wl.Id, sfWorkOrderID, salesForceProjectID);
                        if (checkListID == 0)
                        {
                            InsertCheckList(woId, wl.Check_List_Item__c, wl.Content__c, wl.Content_For_Check_List_Item_As_Others__c);
                            int newId = SqlCommon.GetNewlyInsertedRecordID(TableName.WO_WORKORDER_CHECKLIST_DATATABLE);
                            if (newId > 0)
                            {
                                CommonMethods.InsertToMISSalesForceMapping(TableName.WO_WORKORDER_CHECKLIST_DATATABLE_PC, wl.Id, newId.ToString(), sfWorkOrderID, salesForceProjectID);
                            }
                        }
                        else
                        {
                            UpdateCheckListInstruction(checkListID, wl.Check_List_Item__c, wl.Content__c, wl.Content_For_Check_List_Item_As_Others__c);
                        }
                    }

                    /* use the same function with Work Shop Instruction */
                    DeleteAllDeletedCheckLists(items.ToArray(), sfWorkOrderID);
                    LogMethods.Log.Debug("GetAllCheckLists:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllCheckLists:Error:" + e.Message);
            }
        }

        private int DeleteCheckList(int dId)
        {
            int row = 0;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                Connection.Open();
                string SqlDelString = "DELETE FROM WO_WORKORDER_CHECKLIST_DATATABLE WHERE ([ID] = @ID)";
                var DelCommand = new SqlCommand(SqlDelString, Connection);
                DelCommand.Parameters.AddWithValue("@ID", dId);
                row = DelCommand.ExecuteNonQuery();
                Connection.Close();
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("DeleteCheckList:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return row;
        }

        private void DeleteAllDeletedCheckLists(string[] items, string sfWorkOrderID)
        {
            try
            {
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.WO_WORKORDER_CHECKLIST_DATATABLE_PC, sfWorkOrderID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(items, i) == -1)
                    {
                        // get MISID
                        int itemIDTemp = CommonMethods.GetMISID(TableName.WO_WORKORDER_CHECKLIST_DATATABLE_PC, i, sfWorkOrderID, salesForceProjectID);
                        // get a row
                        if (DeleteCheckList(itemIDTemp) > 0)
                        {
                            // remove MISID out of MISSalesForceMapping
                            CommonMethods.Delete(TableName.WO_WORKORDER_CHECKLIST_DATATABLE_PC, i, sfWorkOrderID, salesForceProjectID);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedCheckLists:Error:" + e.Message);
            }
        }

        private bool UpdateCheckListInstruction(int checkListID, string item, string content, string contentForOthers)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlUpdateString = "UPDATE [WO_WORKORDER_CHECKLIST_DATATABLE] SET  [checklistID] = @checklistID, [value] = @value WHERE [ID] = @ID";
                var UpdateCommand = new SqlCommand(SqlUpdateString, Connection);
                UpdateCommand.Parameters.AddWithValue("@ID", checkListID);
                UpdateCommand.Parameters.AddWithValue("@checklistID", GetCheckListID(item));
                if (item == "Others")
                {
                    UpdateCommand.Parameters.AddWithValue("@value", contentForOthers);
                }
                else
                {
                    UpdateCommand.Parameters.AddWithValue("@value", content);
                }
                Connection.Open();
                UpdateCommand.ExecuteNonQuery();
                ret = true;
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("UpdateCheckListInstruction:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

        private bool InsertCheckList(int woId, string item, string content, string contentForOthers)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlSelectString = "INSERT INTO WO_WORKORDER_CHECKLIST_DATATABLE(woID, checklistID, value) VALUES (@woID, @checklistID, @value)";
                var InsertCommand = new SqlCommand(SqlSelectString, Connection);
                InsertCommand.Parameters.AddWithValue("@woID", woId);
                InsertCommand.Parameters.AddWithValue("@checklistID", GetCheckListID(item));
                if (item == "Others")
                {
                    InsertCommand.Parameters.AddWithValue("@value", contentForOthers);
                }
                else
                {
                    InsertCommand.Parameters.AddWithValue("@value", content);
                }
                Connection.Open();
                InsertCommand.ExecuteNonQuery();
                ret = true;
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("InsertCheckList:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

        private int GetCheckListID(string item)
        {
            int id = 0;
            switch (item)
            {
                case "Certificate Of Liability Insurance":
                    id = 35;
                    break;
                case "Duty Cop":
                    id = 115;
                    break;
                case "Hoarding Is Up":
                    id = 105;
                    break;
                case "Hoisting Permit":
                    id = 25;
                    break;
                case "Others":
                    id = 209;
                    break;
                case "Parking Space Or Loading Dock":
                    id = 110;
                    break;
                case "Security Office - Contact Phone":
                    id = 200;
                    break;
                case "Sign Permit":
                    id = 10;
                    break;
                case "Sign Permit Waiver":
                    id = 15;
                    break;
                case "Site Check Report":
                    id = 5;
                    break;
                case "Special Equipment":
                    id = 205;
                    break;
                case "Stake Out Report":
                    id = 20;
                    break;
                default:
                    id = 30;
                    break;
            }

            return id;
        }

        private bool InsertNote(int woId, string title, string content)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlSelectString = "INSERT INTO [WO_ShippingItem] ([WoID], [ItemNumber], [Qty], [Description]) VALUES (@WoID, @ItemNumber, @Qty, @Description)";
                var InsertCommand = new SqlCommand(SqlSelectString, Connection);
                InsertCommand.Parameters.AddWithValue("@woID", woId);
                InsertCommand.Parameters.AddWithValue("@ItemNumber", 1);
                InsertCommand.Parameters.AddWithValue("@Qty", title);
                if (content != null)
                {
                    InsertCommand.Parameters.AddWithValue("@Description", content);
                }
                else
                {
                    InsertCommand.Parameters.AddWithValue("@Description", "");
                }
                Connection.Open();
                InsertCommand.ExecuteNonQuery();
                ret = true;
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("InsertNote:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

        private void UpdateNote(int itemID, string title, string content)
        {
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlUpdateString = "UPDATE [WO_ShippingItem] SET [Qty] = @Qty, [Description] = @Description WHERE [ItemID] = @ItemID";
                var UpdateCommand = new SqlCommand(SqlUpdateString, Connection);
                UpdateCommand.Parameters.AddWithValue("@ItemID", itemID);
                UpdateCommand.Parameters.AddWithValue("@Qty", title);
                if (content != null)
                {
                    UpdateCommand.Parameters.AddWithValue("@Description", content);
                }
                else
                {
                    UpdateCommand.Parameters.AddWithValue("@Description", "");
                }
                Connection.Open();
                UpdateCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("UpdateNote:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }
        }

        private int DeleteNote(int itemID)
        {
            int row = 0;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                Connection.Open();
                string SqlDelString = "DELETE FROM WO_ShippingItem WHERE ([itemID] = @itemID)";
                var DelCommand = new SqlCommand(SqlDelString, Connection);
                DelCommand.Parameters.AddWithValue("@itemID", itemID);
                row = DelCommand.ExecuteNonQuery();
                Connection.Close();
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("DeleteNote:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return row;
        }

        private void DeleteAllDeletedNotes(string[] items, string sfWorkOrderID)
        {
            try
            {
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.WO_ShippingItem_P, sfWorkOrderID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(items, i) == -1)
                    {
                        // get MISID
                        int itemIDTemp = CommonMethods.GetMISID(TableName.WO_ShippingItem_P, i, sfWorkOrderID, salesForceProjectID);
                        // get a row
                        if (DeleteNote(itemIDTemp) > 0)
                        {
                            // remove MISID out of MISSalesForceMapping
                            CommonMethods.Delete(TableName.WO_ShippingItem_P, i, sfWorkOrderID, salesForceProjectID);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedNotes:Error:" + e.Message);
            }
        }

        public void GetAllNotes(int woId, enterprise.QueryResult result, string sfWorkOrderID)
        {
            try
            {
                if (result != null)
                {
                    IEnumerable<enterprise.AttachedContentNote> noteList = result.records.Cast<enterprise.AttachedContentNote>();
                    List<string> notes = new List<string>();
                    foreach (var q in noteList)
                    {
                        notes.Add(q.Id);
                        int noteID = CommonMethods.GetMISID(TableName.WO_ShippingItem_P, q.Id, sfWorkOrderID, salesForceProjectID);
                        if (noteID == 0)
                        {
                            InsertNote(woId, q.Title, q.TextPreview);
                            int newNoteId = SqlCommon.GetNewlyInsertedRecordID(TableName.WO_ShippingItem);
                            if (newNoteId > 0)
                            {
                                CommonMethods.InsertToMISSalesForceMapping(TableName.WO_ShippingItem_P, q.Id, newNoteId.ToString(), sfWorkOrderID, salesForceProjectID);
                            }
                        }
                        else
                        {
                            UpdateNote(noteID, q.Title, q.TextPreview);
                        }
                    }

                    DeleteAllDeletedNotes(notes.ToArray(), sfWorkOrderID);
                    LogMethods.Log.Debug("GetAllNotes:Debug:" + "Done");
                }
            }
            catch (Exception e) 
            {
                LogMethods.Log.Error("GetAllNotes:Error:" + e.Message);
            }
        }

    }
}
