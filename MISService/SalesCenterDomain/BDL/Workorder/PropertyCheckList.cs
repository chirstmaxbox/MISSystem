using System;
using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Workorder
{
    public class WorkorderPropertyChecklist
    {
        private readonly int _woID;

        public WorkorderPropertyChecklist(int myID, int woID)
        {
            _woID = woID;
        }

        public bool ChecklistApplicable
        {
            get { return GetChecklistApplicable(); }
            set { UpdateChecklistApplicable(value); }
        }

        private bool GetChecklistApplicable()
        {
            bool b = false;

            if (_woID > 100)
            {
                DataRow row = WorkorderShared.GetWorkorderInfo(_woID);
                b = Convert.ToBoolean(row["ChecklistApplicable"]);
            }

            return b;
        }

        private void UpdateChecklistApplicable(bool checklistApplicableValue)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_WO] SET [ChecklistApplicable] = @ChecklistApplicable WHERE [woID] = @woID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
                UPdateCommand.Parameters.Add("@ChecklistApplicable", SqlDbType.Bit).Value = checklistApplicableValue;
                try
                {
                    Connection.Open();
                    UPdateCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorlog = ex.Message;
                }
                finally
                {
                    Connection.Close();
                }
            }
        }
    }
}