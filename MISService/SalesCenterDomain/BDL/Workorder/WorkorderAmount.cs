using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Workorder
{
    public class WorkorderAmount
	{

        public void Update(int woID, double amount)
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string updateString = "UPDATE [Sales_JobMasterList_WO] SET [WorkorderAmount] = @WorkorderAmount WHERE [WoID] = @WoID";
                var updateCommand = new System.Data.SqlClient.SqlCommand(updateString, connection);
                try
                {
                    updateCommand.Parameters.Add("@WoID", SqlDbType.Int).Value = woID;
                    updateCommand.Parameters.Add("@WorkorderAmount", SqlDbType.Float).Value = amount;

                    connection.Open();
                    updateCommand.ExecuteNonQuery();

                }
                catch (SqlException ex)
                {
                    string errorLog = ex.Message;
                }
                finally
                {
                    connection.Close();
                }

            }

        }

    }


}
