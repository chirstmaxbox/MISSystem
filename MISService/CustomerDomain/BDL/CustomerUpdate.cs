using System.Data;
using System.Data.SqlClient;
using CustomerDomain.BO;

namespace CustomerDomain.BDL
{
    public class CustomerUpdate
    {
        private int _rowID = -1;
        public CustomerUpdate(int CustomerID)
        {
            if (CustomerID > CustomerDomainConstants.BEGIN_CUSTOMER_ID)
            {
                _rowID = CustomerID;
            }
        }

        public System.DateTime FirstContractDate
        {
            set { UpdateFirstContractDate(value); }
        }


        private void UpdateFirstContractDate(System.DateTime contractDate)
        {
            using (System.Data.SqlClient.SqlConnection Connection = new System.Data.SqlClient.SqlConnection(CustomerDomainConfiguration.ConnectionString))
            {
                string UPdateString = "UPDATE [CUSTOMER] SET [FirstContractDate] = @FirstContractDate WHERE [ROWID] = @ROWID";
                System.Data.SqlClient.SqlCommand UpdateCommand = new System.Data.SqlClient.SqlCommand(UPdateString, Connection);
                try
                {
                    Connection.Open();
                    UpdateCommand.Parameters.Add("@FirstContractDate", SqlDbType.SmallDateTime);
                    UpdateCommand.Parameters.Add("@ROWID", SqlDbType.Int);

                    UpdateCommand.Parameters["@FirstContractDate"].Value = contractDate;
                    UpdateCommand.Parameters["@ROWID"].Value = _rowID;

                    UpdateCommand.ExecuteNonQuery();
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

        public void UpdateIndustryID(string industryID)
        {
            using (System.Data.SqlClient.SqlConnection Connection = new System.Data.SqlClient.SqlConnection(CustomerDomainConfiguration.ConnectionString))
            {
                string UPdateString = "UPDATE [CUSTOMER] SET [IndustryID]=@IndustryID WHERE [ROWID] = @ROWID";
                System.Data.SqlClient.SqlCommand UpdateCommand = new System.Data.SqlClient.SqlCommand(UPdateString, Connection);
                try
                {
                    Connection.Open();
                    UpdateCommand.Parameters.Add("@IndustryID", SqlDbType.Int).Value = industryID ;
                
                    UpdateCommand.Parameters.Add("@ROWID", SqlDbType.Int).Value = _rowID;

                    UpdateCommand.ExecuteNonQuery();

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

          

        public void UpdateSelectType(int selectType, int headoffice)
        {
            using (System.Data.SqlClient.SqlConnection Connection = new System.Data.SqlClient.SqlConnection(CustomerDomainConfiguration.ConnectionString))
            {
                string UPdateString = "UPDATE [CUSTOMER] SET [selectType]= @selectType,[HEADOFFICE]=@HEADOFFICE WHERE [ROWID] = @ROWID";
                System.Data.SqlClient.SqlCommand UpdateCommand = new System.Data.SqlClient.SqlCommand(UPdateString, Connection);
                try
                {
                    Connection.Open();
                    UpdateCommand.Parameters.Add("@selectType", SqlDbType.Int).Value = selectType;
                    UpdateCommand.Parameters.Add("@HEADOFFICE", SqlDbType.Int).Value = headoffice;
                    UpdateCommand.Parameters.Add("@ROWID", SqlDbType.Int).Value = _rowID;

                    UpdateCommand.ExecuteNonQuery();

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

        public void UpdateReferByEmployee(int ReferByEmployee)
        {
            using (System.Data.SqlClient.SqlConnection Connection = new System.Data.SqlClient.SqlConnection(CustomerDomainConfiguration.ConnectionString))
            {
                string UPdateString = "UPDATE [CUSTOMER] SET [ReferByEmployee]= @ReferByEmployee WHERE [ROWID] = @ROWID";
                System.Data.SqlClient.SqlCommand UpdateCommand = new System.Data.SqlClient.SqlCommand(UPdateString, Connection);
                try
                {
                    Connection.Open();
                    UpdateCommand.Parameters.Add("@ReferByEmployee", SqlDbType.Int).Value = ReferByEmployee;
                    UpdateCommand.Parameters.Add("@ROWID", SqlDbType.Int).Value = _rowID;

                    UpdateCommand.ExecuteNonQuery();

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

        public void UpdateReferByText(string ReferByText)
        {
            using (System.Data.SqlClient.SqlConnection Connection = new System.Data.SqlClient.SqlConnection(CustomerDomainConfiguration.ConnectionString))
            {
                string UPdateString = "UPDATE [CUSTOMER] SET [ReferByText]= @ReferByText WHERE [ROWID] = @ROWID";
                System.Data.SqlClient.SqlCommand UpdateCommand = new System.Data.SqlClient.SqlCommand(UPdateString, Connection);
                try
                {
                    Connection.Open();
                    UpdateCommand.Parameters.Add("@ReferByText", SqlDbType.NVarChar, 500).Value = ReferByText;
                    UpdateCommand.Parameters.Add("@ROWID", SqlDbType.Int).Value = _rowID;

                    UpdateCommand.ExecuteNonQuery();

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


        public void UpdateSalesName(int SalesID, int sa1ID)
        {
            using (System.Data.SqlClient.SqlConnection Connection = new System.Data.SqlClient.SqlConnection(CustomerDomainConfiguration.ConnectionString))
            {
                string UPdateString = "UPDATE [CUSTOMER] SET [SalesID]= @SalesID, [Sa1ID]= @Sa1ID WHERE [ROWID] = @ROWID";
                System.Data.SqlClient.SqlCommand UpdateCommand = new System.Data.SqlClient.SqlCommand(UPdateString, Connection);
                try
                {
                    Connection.Open();

                    UpdateCommand.Parameters.Add("@SalesID", SqlDbType.Int).Value = SalesID;
                    UpdateCommand.Parameters.Add("@Sa1ID", SqlDbType.Int).Value = sa1ID;
                    UpdateCommand.Parameters.Add("@ROWID", SqlDbType.Int).Value = _rowID;


                    UpdateCommand.ExecuteNonQuery();

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


        public void UpdateThisIsRepeatCustomer(bool value)
        {
            using (System.Data.SqlClient.SqlConnection Connection = new System.Data.SqlClient.SqlConnection(CustomerDomainConfiguration.ConnectionString))
            {
                string UPdateString = "UPDATE [CUSTOMER] SET [DeemedAsOld]= @DeemedAsOld WHERE [ROWID] = @ROWID";
                System.Data.SqlClient.SqlCommand UpdateCommand = new System.Data.SqlClient.SqlCommand(UPdateString, Connection);
                try
                {
                    Connection.Open();

                    UpdateCommand.Parameters.Add("@DeemedAsOld", SqlDbType.Bit).Value = value;
                    UpdateCommand.Parameters.Add("@ROWID", SqlDbType.Int).Value = _rowID;

                    UpdateCommand.ExecuteNonQuery();

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