using System;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL
{
    public static class SqlCommon
    {
        public static int GetNewlyInsertedRecordID(string tblName)
        {
            int ticketID = 0;

            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string SelectString = "SELECT IDENT_CURRENT('" + tblName + " ')";
                //Dim SelectString As String = "SELECT IDENT_CURRENT('Sales_JobMasterList_EstRev')"
                var SelectCommand = new SqlCommand(SelectString, Connection);
                try
                {
                    Connection.Open();
                    object tID = SelectCommand.ExecuteScalar();
                    ticketID = Convert.ToInt32(tID);
                }

                catch (SqlException ex)
                {
                    ticketID = 0;
                    //           LogError(ex.Message);
                }
                finally
                {
                    Connection.Close();
                }
            }
            return ticketID;
        }
    }
}