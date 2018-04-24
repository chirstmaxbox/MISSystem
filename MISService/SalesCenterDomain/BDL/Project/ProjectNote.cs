//Obsolete, June , 2011

using System;
using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Project
{
    public class ProjectNote
    {
        public static void Insert(int jobID)
        {
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);

            using (ConnectionSQL)
            {
                string InsertString =
                    "INSERT INTO [Sales_JobMasterList_Note] ([jobID], [postDate], [lastEditDate]) VALUES (@jobID,  @postDate,  @lastEditDate)";
                var InsertCommand = new SqlCommand(InsertString, ConnectionSQL);
                InsertCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = jobID;
                InsertCommand.Parameters.Add("@postDate", SqlDbType.DateTime).Value = DateTime.Now;
                InsertCommand.Parameters.Add("@lastEditDate", SqlDbType.DateTime).Value = DateTime.Now;

                try
                {
                    ConnectionSQL.Open();
                    InsertCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorLog = ex.Message;
                }
                finally
                {
                    ConnectionSQL.Close();
                }
            }
        }
    }
}