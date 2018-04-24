using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon;
using SpecDomain.BO;

namespace SpecDomain.BLL.Service
{
    public class MyEstServiceCreate
    {
        private readonly int _serviceParentID;
        public MyEstServiceCreate(int parentID)
        {
            _serviceParentID = parentID;            
        }

        public void InsertServices( int serviceGroupID)
        {
            var sd = new SpecServiceSingleGroup(serviceGroupID );
            DataTable t1 = sd.Values;
            if (t1 == null) return;
            int printOrder = GetQsMaxPrintOrder() + 1;

            try
            {
                foreach (DataRow row in t1.Rows)
                {
                    InsertRecord(Convert.ToInt32(row["PC_ID"]),
                                 MyConvert.ConvertToString(row["Charge"]),
                                 1,
                                 MyConvert.ConvertToString(row["CONTENTS"]),
                                 MyConvert.ConvertToString(row["PS_NAME"]),
                                 MyConvert.ConvertToString(row["Charge"]),
                                 printOrder
                        );
                }
                t1.Clear();
            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;
            }
            finally
            {
            }
        }

        //Insert New
        private void InsertRecord(int qsServiceID, string qsAmount, int qsQty, string qsDescription, string qsTitle, string qsAmountText, int printNumber)
        {
            using (var connection = new SqlConnection(SpecConfiguration.ConnectionString))
            {
                const string insertString = "INSERT INTO [EST_SERVICE] ([estRevID], [qsPrintOrder], [qsServiceID],[qsAmount], [qsQty],[recordType],[qsDescription],[qsTitle],[qsAmountText]) VALUES (@estRevID, @qsPrintOrder, @qsServiceID, @qsAmount, @qsQty, @recordType,@qsDescription,@qsTitle, @qsAmountText)";
                // Create the command and set its properties.
                var insertCommand = new SqlCommand(insertString, connection);
                try
                {
                    //@estRevID, @qsPrintOrder, @qsServiceID, @qsAmount, @qsQty, @recordType)"
                    insertCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = _serviceParentID;
                    insertCommand.Parameters.Add("@qsPrintOrder", SqlDbType.SmallInt).Value = printNumber;
                    insertCommand.Parameters.Add("@qsServiceID", SqlDbType.SmallInt).Value = qsServiceID;
                    insertCommand.Parameters.Add("@qsAmount", SqlDbType.NVarChar, 100).Value = qsAmount;
                    insertCommand.Parameters.Add("@qsQty", SqlDbType.SmallInt).Value = qsQty;
                    insertCommand.Parameters.Add("@recordType", SqlDbType.NChar, 7).Value = "Est";
                    insertCommand.Parameters.Add("@qsDescription", SqlDbType.NVarChar, 300).Value = qsDescription;
                    insertCommand.Parameters.Add("@qsTitle", SqlDbType.NVarChar, 300).Value = qsTitle;

                    insertCommand.Parameters.Add("@qsAmountText", SqlDbType.NVarChar, 300).Value = qsAmountText;


                    connection.Open();
                    insertCommand.ExecuteNonQuery();
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


        private int GetQsMaxPrintOrder()
        {
            int max = 0;
            var connectionSQL =new SqlConnection(SpecConfiguration.ConnectionString);
            const string sqlSelectString1 = "SELECT MAX(qsPrintOrder) AS qsPrintOrder FROM EST_SERVICE WHERE (estRevID = @estRevID) AND (recordType = @recordType)";
            var selectCommand1 = new SqlCommand(sqlSelectString1, connectionSQL);
            var adapter1 = new SqlDataAdapter(selectCommand1);
            adapter1.SelectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = _serviceParentID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.NChar, 7).Value = "Est";
            var ds1 = new DataSet();

            ds1.Tables.Clear();

            try
            {
                connectionSQL.Open();
                int numRowsAffected = adapter1.Fill(ds1, "t1");
                if (numRowsAffected != 0)
                {
                    //Destination
                    DataRow row = ds1.Tables["t1"].Rows[0];
                    if (!Convert.IsDBNull(row["qsPrintOrder"]))
                    {
                        max = Convert.ToInt32(row["qsPrintOrder"]);
                    }
                }
            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;
            }
            finally
            {
                connectionSQL.Close();
            }
            return max;
        }


    }
}