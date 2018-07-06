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
    public class SiteCheckWOMethods
    {
        private EndpointAddress apiAddr;
        private enterprise.SessionHeader header;
        private string salesForceProjectID;

        public SiteCheckWOMethods(string salesForceProjectID)
        {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
            this.salesForceProjectID = salesForceProjectID;
        }

        public void GetAllInspectorInstructions(int woId, string sfWorkOrderID)
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Category__c, Final_Instruction__c, Instruction__c "
                        + " FROM Inspector_Instruction__c where Work_Order_Name__c = '" + sfWorkOrderID + "'";

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
                    IEnumerable<enterprise.Inspector_Instruction__c> inspectorList = result.records.Cast<enterprise.Inspector_Instruction__c>();
                    List<string> items = new List<string>();
                    foreach (var wl in inspectorList)
                    {
                        items.Add(wl.Id);
                        /* check if the work order exists */
                        int workShopID = CommonMethods.GetMISID(TableName.WO_Instruction_DataTable_SC_I, wl.Id, sfWorkOrderID, salesForceProjectID);
                        if (workShopID == 0)
                        {
                            InsertInspectorInstruction(woId, wl.Category__c, wl.Instruction__c, wl.Final_Instruction__c);
                            int newId = SqlCommon.GetNewlyInsertedRecordID(TableName.WO_Instruction_DataTable);
                            CommonMethods.InsertToMISSalesForceMapping(TableName.WO_Instruction_DataTable_SC_I, wl.Id, newId.ToString(), sfWorkOrderID, salesForceProjectID);
                        }
                        else
                        {
                            UpdateInspectorInstruction(workShopID, wl.Category__c, wl.Instruction__c, wl.Final_Instruction__c);
                        }
                    }

                    DeleteAllDeletedInspectorInstructions(items.ToArray(), sfWorkOrderID);
                    LogMethods.Log.Debug("GetAllInspectorInstructions:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllInspectorInstructions:Error:" + e.Message);
            }
        }

        private int DeleteInspectorInstruction(int dId)
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
                LogMethods.Log.Error("DeleteInspectorInstruction:Crash:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return row;
        }

        private void DeleteAllDeletedInspectorInstructions(string[] items, string sfWorkOrderID)
        {
            try
            {
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.WO_Instruction_DataTable_SC_I, sfWorkOrderID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(items, i) == -1)
                    {
                        // get MISID
                        int itemIDTemp = CommonMethods.GetMISID(TableName.WO_Instruction_DataTable_SC_I, i, sfWorkOrderID, salesForceProjectID);
                        // get a row
                        if (DeleteInspectorInstruction(itemIDTemp) > 0)
                        {
                            // remove MISID out of MISSalesForceMapping
                            CommonMethods.Delete(TableName.WO_Instruction_DataTable_SC_I, i, sfWorkOrderID, salesForceProjectID);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedInspectorInstructions:Error:" + e.Message);
            }
        }

        private bool UpdateInspectorInstruction(int dID, string category, string content, string finalContent)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlUpdateString = "UPDATE [WO_Instruction_Datatable] SET  [Contents] = @Contents, [InstructionID] = @InstructionID WHERE [dID] = @dID";
                var UpdateCommand = new SqlCommand(SqlUpdateString, Connection);
                UpdateCommand.Parameters.AddWithValue("@dID", dID);
                UpdateCommand.Parameters.AddWithValue("@InstructionID", GetInspectorInstructionID(category, content));
                UpdateCommand.Parameters.AddWithValue("@Contents", finalContent);
                Connection.Open();
                UpdateCommand.ExecuteNonQuery();
                ret = true;
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("UpdateInspectorInstruction:Crash:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

        private bool InsertInspectorInstruction(int woID, string category, string content, string finalContent)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlSelectString = "INSERT INTO WO_Instruction_Datatable(woID, InstructionID, Contents, recordType) VALUES (@woID, @InstructionID, @Contents, @recordType)";
                var InsertCommand = new SqlCommand(SqlSelectString, Connection);
                InsertCommand.Parameters.AddWithValue("@woID", woID);
                InsertCommand.Parameters.AddWithValue("@InstructionID", GetInspectorInstructionID(category, content));
                InsertCommand.Parameters.AddWithValue("@Contents", finalContent);
                InsertCommand.Parameters.AddWithValue("@recordType", 0); //1 for workship instruction, 0 for Instruction for Installer and Site...
                Connection.Open();
                InsertCommand.ExecuteNonQuery();
                ret = true;
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("InsertInspectorInstruction:Crash:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

        private int GetInspectorInstructionID(string category, string content)
        {
            int id = 0;
            switch (category)
            {
                case "Bulkhead":
                    id = 1061;
                    break;
                case "Carrier Box":
                    id = 1051;
                    break;
                case "Color":
                    id = 1052;
                    break;
                case "Dimensions":
                    id = 1063;
                    break;
                case "Hoarding":
                    id = 1058;
                    break;
                case "Hoisting Permit / Duty Cop":
                    id = 1054;
                    break;
                case "Installation Difficulty":
                    id = 1050;
                    break;
                case "Measurement":
                    id = 1060;
                    break;
                case "Meet Client":
                    switch (content)
                    {
                        case "Meet client on site for < insert your content here >":
                            id = 1056;
                            break;
                        default:
                            id = 1055;
                            break;
                    }
                    break;
                case "Others":
                    id = 1065;
                    break;
                case "Pin Point Location":
                    id = 1057;
                    break;
                case "Sign Permit":
                    id = 1053;
                    break;
                case "Special Equipment":
                    id = 1059;
                    break;
                case "Stake Out":
                    id = 1101;
                    break;
                case "Take Photo":
                    id = 1062;
                    break;
                default:
                    id = 1064;
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
                    string query = "SELECT Id, Check_List_Item__c, Content__c "
                        + " FROM Site_Check_Check_List__c where Work_Order_Name__c = '" + sfWorkOrderID + "'";

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
                    IEnumerable<enterprise.Site_Check_Check_List__c> checkList = result.records.Cast<enterprise.Site_Check_Check_List__c>();
                    List<string> items = new List<string>();
                    foreach (var wl in checkList)
                    {
                        items.Add(wl.Id);
                        int checkListID = CommonMethods.GetMISID(TableName.WO_WORKORDER_CHECKLIST_DATATABLE_SC_C, wl.Id, sfWorkOrderID, salesForceProjectID);
                        if (checkListID == 0)
                        {
                            InsertCheckList(woId, wl.Check_List_Item__c, wl.Content__c);
                            int newId = SqlCommon.GetNewlyInsertedRecordID(TableName.WO_WORKORDER_CHECKLIST_DATATABLE);
                            CommonMethods.InsertToMISSalesForceMapping(TableName.WO_WORKORDER_CHECKLIST_DATATABLE_SC_C, wl.Id, newId.ToString(), sfWorkOrderID, salesForceProjectID);
                        }
                        else
                        {
                            UpdateCheckListInstruction(checkListID, wl.Check_List_Item__c, wl.Content__c);
                        }
                    }

                    /* use the same function with Work Shop Instruction */
                    DeleteAllDeletedCheckLists(items.ToArray(), sfWorkOrderID);
                    LogMethods.Log.Debug("GetAllInstallerInstructions:Debug:" + "Done");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllInstallerInstructions:Error:" + e.Message);
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
                LogMethods.Log.Error("DeleteCheckList:Crash:" + ex.Message);
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
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.WO_WORKORDER_CHECKLIST_DATATABLE_SC_C, sfWorkOrderID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(items, i) == -1)
                    {
                        // get MISID
                        int itemIDTemp = CommonMethods.GetMISID(TableName.WO_WORKORDER_CHECKLIST_DATATABLE_SC_C, i, sfWorkOrderID, salesForceProjectID);
                        // get a row
                        if (DeleteCheckList(itemIDTemp) > 0)
                        {
                            // remove MISID out of MISSalesForceMapping
                            CommonMethods.Delete(TableName.WO_WORKORDER_CHECKLIST_DATATABLE_SC_C, i, sfWorkOrderID, salesForceProjectID);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedCheckLists:Error:" + e.Message);
            }
        }

        private bool UpdateCheckListInstruction(int checkListID, string item, string content)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlUpdateString = "UPDATE [WO_WORKORDER_CHECKLIST_DATATABLE] SET  [checklistID] = @checklistID, [value] = @value WHERE [ID] = @ID";
                var UpdateCommand = new SqlCommand(SqlUpdateString, Connection);
                UpdateCommand.Parameters.AddWithValue("@ID", checkListID);
                UpdateCommand.Parameters.AddWithValue("@checklistID", GetCheckListID(item));
                UpdateCommand.Parameters.AddWithValue("@value", content);
                Connection.Open();
                UpdateCommand.ExecuteNonQuery();
                ret = true;
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("UpdateCheckListInstruction:Crash:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

        private bool InsertCheckList(int woId, string item, string content)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlSelectString = "INSERT INTO WO_WORKORDER_CHECKLIST_DATATABLE(woID, checklistID, value) VALUES (@woID, @checklistID, @value)";
                var InsertCommand = new SqlCommand(SqlSelectString, Connection);
                InsertCommand.Parameters.AddWithValue("@woID", woId);
                InsertCommand.Parameters.AddWithValue("@checklistID", GetCheckListID(item));
                InsertCommand.Parameters.AddWithValue("@value", content);
                Connection.Open();
                InsertCommand.ExecuteNonQuery();
                ret = true;
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("InsertCheckList:Crash:" + ex.Message);
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
                    id = 219;
                    break;
                case "Designer Drawing":
                    id = 222;
                    break;
                case "Elevation Drawing":
                    id = 224;
                    break;
                case "Photo Of Sign":
                    id = 221;
                    break;
                case "Previous Site Check Report":
                    id = 225;
                    break;
                case "Site Plan / Sign Location Map":
                    id = 223;
                    break;
                default:
                    id = 220;
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
                LogMethods.Log.Error("InsertNote:Crash:" + ex.Message);
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
                LogMethods.Log.Error("UpdateNote:Crash:" + ex.Message);
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
                LogMethods.Log.Error("DeleteNote:Crash:" + ex.Message);
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
                List<string> ids = CommonMethods.GetAllSalesForceID(TableName.WO_ShippingItem_SC, sfWorkOrderID, salesForceProjectID);
                foreach (string i in ids)
                {
                    // not found
                    if (Array.IndexOf(items, i) == -1)
                    {
                        // get MISID
                        int itemIDTemp = CommonMethods.GetMISID(TableName.WO_ShippingItem_SC, i, sfWorkOrderID, salesForceProjectID);
                        // get a row
                        if (DeleteNote(itemIDTemp) > 0)
                        {
                            // remove MISID out of MISSalesForceMapping
                            CommonMethods.Delete(TableName.WO_ShippingItem_SC, i, sfWorkOrderID, salesForceProjectID);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteAllDeletedWorkOrderItems:Error:" + e.Message);
            }
        }

        public void GetAllNotes(int woId, enterprise.QueryResult result, string sfWorkOrderID)
        {
            if (result != null)
            {
                IEnumerable<enterprise.AttachedContentNote> noteList = result.records.Cast<enterprise.AttachedContentNote>();
                List<string> notes = new List<string>();
                foreach (var q in noteList)
                {
                    notes.Add(q.Id);
                    int noteID = CommonMethods.GetMISID(TableName.WO_ShippingItem_SC, q.Id, sfWorkOrderID, salesForceProjectID);
                    if (noteID == 0)
                    {
                        InsertNote(woId, q.Title, q.TextPreview);
                        int newNoteId = SqlCommon.GetNewlyInsertedRecordID(TableName.WO_ShippingItem);
                        CommonMethods.InsertToMISSalesForceMapping(TableName.WO_ShippingItem_SC, q.Id, newNoteId.ToString(), sfWorkOrderID, salesForceProjectID);
                    }
                    else
                    {
                        UpdateNote(noteID, q.Title, q.TextPreview);
                    }
                }

                DeleteAllDeletedNotes(notes.ToArray(), sfWorkOrderID);
                LogMethods.Log.Debug("HandleNotes:Debug:" + "Done");
            }
        }
    }
}
