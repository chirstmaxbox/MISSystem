
using System.Data;
using System.Data.SqlClient;

namespace EmployeeDomain.BLL
{
    public class EmployeePicture
    {
        public bool IsThereImage { get; private set;}

        private readonly int _employeeID ;

        public EmployeePicture(int employeeID)
        {
            _employeeID = employeeID;
        }


        public void Update(byte[] pic, int rowID)
        {
            using (var connection = new SqlConnection(EmployeeConfiguration.ConnectionString))
            {
                const string updateString = "UPDATE [FW_EmployeePicture] SET  [Picture] = @Picture WHERE [RowID] = @RowID";
                var updateCommand = new System.Data.SqlClient.SqlCommand(updateString, connection);
                updateCommand.Parameters.Add("@RowID", SqlDbType.Int).Value = rowID;
                updateCommand.Parameters.Add("@Picture", SqlDbType.Image).Value = pic;

                try
                {
                    connection.Open();
                    updateCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorlog = ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }

        }


        public byte[] GetImage()
        {
            byte[] imageData = null;

            var connectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            const string sqlSelectString = "SELECT RowID, Picture FROM FW_EmployeePicture WHERE EmployeeNumber=@EmployeeNumber";
            var selectCommand = new SqlCommand(sqlSelectString, connectionSQL);
            var adapter = new SqlDataAdapter(selectCommand);
            var ds2 = new DataSet();
            ds2.Tables.Clear();
            adapter.SelectCommand.Parameters.Add("@EmployeeNumber", SqlDbType.Int).Value = _employeeID;

            try
            {
                connectionSQL.Open();
                int affectedRows = adapter.Fill(ds2, "t2");
                if (affectedRows > 0)
                {
                    var row1 = ds2.Tables["t2"].Rows[0];
                    if (row1["Picture"] != null)
                    {
                        imageData = (byte[])row1["Picture"];
                        IsThereImage = true;
                    }
                }
            }
            catch (SqlException ex)
            {
                string errolog = ex.Message;
            }
            finally
            {
                connectionSQL.Close();
            }

            return imageData;

        }

        
        public void Insert(byte[] pic)
        {
            using (var connection = new SqlConnection(EmployeeConfiguration.ConnectionString))
            {
                const string insertString = "INSERT INTO [FW_EmployeePicture] ([EmployeeNumber], [Picture]) VALUES (@EmployeeNumber, @Picture)";
                var insertCommand = new System.Data.SqlClient.SqlCommand(insertString, connection);
                insertCommand.Parameters.Add("@EmployeeNumber", SqlDbType.Int).Value = _employeeID;
                insertCommand.Parameters.Add("@Picture", SqlDbType.Image).Value = pic;
                try
                {
                    connection.Open();
                    insertCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorlog = ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }

        }
        

    }
}