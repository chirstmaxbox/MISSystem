
using System.Data;
using System.Data.SqlClient;

namespace EmployeeDomain.BLL
{
    public class EmployeeSignature
    {
        public bool IsThereImage
        {
            get;private set;
        }

        private readonly int _employeeID = 0;

        public EmployeeSignature(int employeeID)
        {
            _employeeID = employeeID;
        }


        public void Update(byte[] pic)
        {
            using (var connection =new SqlConnection(EmployeeConfiguration.ConnectionString ))
            {
                const string updateString = "UPDATE [FW_EMPLOYEES] SET  [Signature] = @Signature WHERE [EmployeeNumber] = @EmployeeNumber";
                var updateCommand = new System.Data.SqlClient.SqlCommand(updateString,connection);
                updateCommand.Parameters.Add("@EmployeeNumber", SqlDbType.Int).Value = _employeeID;
                updateCommand.Parameters.Add("@Signature", SqlDbType.Image).Value = pic;

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


        public  byte[] GetImage()
        {
            byte[] imageData = null;

            var connectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            const string sqlSelectString = "SELECT EmployeeNumber,Signature FROM FW_EMPLOYEES WHERE EmployeeNumber=@EmployeeNumber";
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
                    if (row1["Signature"]!=null)
                    {
                        imageData =(byte[]) row1["Signature"];
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

    }
}