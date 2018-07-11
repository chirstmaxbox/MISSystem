using MISService.Method;
using MISService.Models;
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
    public class ServiceWOMethods
    {
        private EndpointAddress apiAddr;
        private enterprise.SessionHeader header;
        private string salesForceProjectID;

        public ServiceWOMethods(string salesForceProjectID)
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
                        int workShopID = CommonMethods.GetMISID(TableName.WO_Instruction_DataTable_SW, wl.Id, sfWorkOrderID, salesForceProjectID);
                        if (workShopID == 0)
                        {
                            InsertWorkShopInstruction(woId, wl.Category__c, wl.Instruction__c, wl.Final_Instruction__c);
                            int newId = SqlCommon.GetNewlyInsertedRecordID(TableName.WO_Instruction_DataTable);
                            CommonMethods.InsertToMISSalesForceMapping(TableName.WO_Instruction_DataTable_SW, wl.Id, newId.ToString(), sfWorkOrderID, salesForceProjectID);
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
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.WO_Instruction_DataTable_SW, sfWorkOrderID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(items, i) == -1)
                    {
                        // get MISID
                        int itemIDTemp = CommonMethods.GetMISID(TableName.WO_Instruction_DataTable_SW, i, sfWorkOrderID, salesForceProjectID);
                        // get a row
                        if (DeleteWorkShopInstruction(itemIDTemp) > 0)
                        {
                            // remove MISID out of MISSalesForceMapping
                            CommonMethods.Delete(TableName.WO_Instruction_DataTable_SW, i, sfWorkOrderID, salesForceProjectID);
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
            switch (category)
            {
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


        public void GetAllServicerInstructions(int woId, string sfWorkOrderID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Category__c, Final_Instruction__c, Instruction__c "
                        + " FROM Servicer_Instruction__c where Work_Order_Name__c = '" + sfWorkOrderID + "'";

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
                    IEnumerable<enterprise.Servicer_Instruction__c> servInstructionList = result.records.Cast<enterprise.Servicer_Instruction__c>();
                    List<string> items = new List<string>();
                    foreach (var wl in servInstructionList)
                    {
                        items.Add(wl.Id);
                        /* check if the work order exists */
                        int instInstructionID = CommonMethods.GetMISID(TableName.WO_Instruction_DataTable_SS, wl.Id, sfWorkOrderID, salesForceProjectID);
                        if (instInstructionID == 0)
                        {
                            InsertServicerInstruction(woId, wl.Category__c, wl.Instruction__c, wl.Final_Instruction__c);
                            int newId = SqlCommon.GetNewlyInsertedRecordID(TableName.WO_Instruction_DataTable);
                            CommonMethods.InsertToMISSalesForceMapping(TableName.WO_Instruction_DataTable_SS, wl.Id, newId.ToString(), sfWorkOrderID, salesForceProjectID);
                        }
                        else
                        {
                            UpdateServicerInstruction(instInstructionID, wl.Category__c, wl.Instruction__c, wl.Final_Instruction__c);
                        }
                    }

                    DeleteAllDeletedServicerInstructions(items.ToArray(), sfWorkOrderID);
                    LogMethods.Log.Debug("GetAllServicerInstructions:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllServicerInstructions:Error:" + e.Message);
            }
        }

        private int DeleteServicerInstruction(int dId)
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
                LogMethods.Log.Error("DeleteServicerInstruction:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return row;
        }

        private void DeleteAllDeletedServicerInstructions(string[] items, string sfWorkOrderID)
        {
            try
            {
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.WO_Instruction_DataTable_SS, sfWorkOrderID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(items, i) == -1)
                    {
                        // get MISID
                        int itemIDTemp = CommonMethods.GetMISID(TableName.WO_Instruction_DataTable_SS, i, sfWorkOrderID, salesForceProjectID);
                        // get a row
                        if (DeleteServicerInstruction(itemIDTemp) > 0)
                        {
                            // remove MISID out of MISSalesForceMapping
                            CommonMethods.Delete(TableName.WO_Instruction_DataTable_SS, i, sfWorkOrderID, salesForceProjectID);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedServicerInstructions:Error:" + e.Message);
            }
        }


        private bool UpdateServicerInstruction(int dID, string category, string content, string finalContent)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlUpdateString = "UPDATE [WO_Instruction_Datatable] SET  [Contents] = @Contents, [InstructionID] = @InstructionID WHERE [dID] = @dID";
                var UpdateCommand = new SqlCommand(SqlUpdateString, Connection);
                UpdateCommand.Parameters.AddWithValue("@dID", dID);
                UpdateCommand.Parameters.AddWithValue("@InstructionID", GetServicerInstructionID(category, content));
                UpdateCommand.Parameters.AddWithValue("@Contents", finalContent);
                Connection.Open();
                UpdateCommand.ExecuteNonQuery();
                ret = true;
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("UpdateServicerInstruction:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

        private bool InsertServicerInstruction(int woID, string category, string content, string finalContent)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlSelectString = "INSERT INTO WO_Instruction_Datatable(woID, InstructionID, Contents, recordType) VALUES (@woID, @InstructionID, @Contents, @recordType)";
                var InsertCommand = new SqlCommand(SqlSelectString, Connection);
                InsertCommand.Parameters.AddWithValue("@woID", woID);
                InsertCommand.Parameters.AddWithValue("@InstructionID", GetServicerInstructionID(category, content));
                InsertCommand.Parameters.AddWithValue("@Contents", finalContent);
                InsertCommand.Parameters.AddWithValue("@recordType", 0); //0 for Instruction for Installer and Site-Check inspector.
                Connection.Open();
                InsertCommand.ExecuteNonQuery();
                ret = true;
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("InsertServicerInstruction:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

        private int GetServicerInstructionID(string category, string content)
        {
            int id = 0;
            switch (category)
            {
                case "C.O.D":
                    id = 1035;
                    break;
                case "Mall":
                    switch (content)
                    {
                        case "Service after mall hour - mall arrangement made, sign in with security before work. Call site supervisor upon arrival and leaving site.":
                            id = 1037;
                            break;
                        case "Service before mall hour - mall arrangement made, sign in with security before work. Call site supervisor upon arrival and leaving site.":
                            id = 1036;
                            break;
                        default:
                            id = 1038;
                            break;
                    }
                    break;
                case "Others":
                    id = 1049;
                    break;
                case "Relocation":
                    id = 1039;
                    break;
                case "Sign Location":
                    id = 1034;
                    break;
                case "Special Equipment":
                    id = 1040;
                    break;
                case "Take Down":
                    switch (content)
                    {
                        case "Patch Hole":
                            id = 1041;
                            break;
                        case "Take down & deliver to < insert your content here >":
                            id = 1047;
                            break;
                        case "Take down & repair.":
                            id = 1042;
                            break;
                        case "Take down in good shape & bring back shop for repair.":
                            id = 1046;
                            break;
                        case "Take down in good shape & store in FS workshop (If Requirement used: TD & Bring back to shop for stock)":
                            id = 1045;
                            break;
                        case "Take down, take picture, & leave sign at the site with client.":
                            id = 1043;
                            break;
                        default:
                            id = 1044;
                            break;
                    }
                    break;
                default:
                    id = 1048;
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
                        + " FROM Service_Check_List__c where Work_Order_Name__c = '" + sfWorkOrderID + "'";

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
                    IEnumerable<enterprise.Service_Check_List__c> checkList = result.records.Cast<enterprise.Service_Check_List__c>();
                    List<string> items = new List<string>();
                    foreach (var wl in checkList)
                    {
                        items.Add(wl.Id);
                        int checkListID = CommonMethods.GetMISID(TableName.WO_WORKORDER_CHECKLIST_DATATABLE_SC, wl.Id, sfWorkOrderID, salesForceProjectID);
                        if (checkListID == 0)
                        {
                            InsertCheckList(woId, wl.Check_List_Item__c, wl.Content__c, wl.Content_For_Check_List_Item_As_Others__c);
                            int newId = SqlCommon.GetNewlyInsertedRecordID(TableName.WO_WORKORDER_CHECKLIST_DATATABLE);
                            CommonMethods.InsertToMISSalesForceMapping(TableName.WO_WORKORDER_CHECKLIST_DATATABLE_SC, wl.Id, newId.ToString(), sfWorkOrderID, salesForceProjectID);
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
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.WO_WORKORDER_CHECKLIST_DATATABLE_SC, sfWorkOrderID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(items, i) == -1)
                    {
                        // get MISID
                        int itemIDTemp = CommonMethods.GetMISID(TableName.WO_WORKORDER_CHECKLIST_DATATABLE_SC, i, sfWorkOrderID, salesForceProjectID);
                        // get a row
                        if (DeleteCheckList(itemIDTemp) > 0)
                        {
                            // remove MISID out of MISSalesForceMapping
                            CommonMethods.Delete(TableName.WO_WORKORDER_CHECKLIST_DATATABLE_SC, i, sfWorkOrderID, salesForceProjectID);
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
                case "Art Drawing":
                    id = 210;
                    break;
                case "C.O.D Invoice":
                    id = 214;
                    break;
                case "Certificate Of Liability Insurance":
                    id = 217;
                    break;
                case "Others":
                    id = 218;
                    break;
                case "Photo Of Sign":
                    id = 213;
                    break;
                case "Site Plan / Sign Location Map":
                    id = 215;
                    break;
                case "Structural Drawing":
                    id = 211;
                    break;
                case "Wiring Diagram":
                    id = 212;
                    break;
                default:
                    id = 216;
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
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.WO_ShippingItem_S, sfWorkOrderID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(items, i) == -1)
                    {
                        // get MISID
                        int itemIDTemp = CommonMethods.GetMISID(TableName.WO_ShippingItem_S, i, sfWorkOrderID, salesForceProjectID);
                        // get a row
                        if (DeleteNote(itemIDTemp) > 0)
                        {
                            // remove MISID out of MISSalesForceMapping
                            CommonMethods.Delete(TableName.WO_ShippingItem_S, i, sfWorkOrderID, salesForceProjectID);
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
                        int noteID = CommonMethods.GetMISID(TableName.WO_ShippingItem_S, q.Id, sfWorkOrderID, salesForceProjectID);
                        if (noteID == 0)
                        {
                            InsertNote(woId, q.Title, q.TextPreview);
                            int newNoteId = SqlCommon.GetNewlyInsertedRecordID(TableName.WO_ShippingItem);
                            CommonMethods.InsertToMISSalesForceMapping(TableName.WO_ShippingItem_S, q.Id, newNoteId.ToString(), sfWorkOrderID, salesForceProjectID);
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
