using System;
using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BLL;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Workorder
{
    public class WorkorderTitleUpdate
    {
        private int _woID;

        public WorkorderTitleUpdate(int woID)
        {
            _woID = woID;
        }

        public int WoID
        {
            set { _woID = value; }
        }

        public int Company1
        {
            get { return GetCompanny1(); }
            set { UpdateCompany1(value); }
        }

        public int Company2
        {
            get { return GetCompanny2(); }
            set { UpdateCompany2(value); }
        }

        public int Contact1
        {
            set { UpdateContact1(value); }
        }

        public int Contact2
        {
            set { UpdateContact2(value); }
        }

        public int PaymentMethod
        {
            set { UpdatePaymentMethod(value); }
        }

        public bool Rush
        {
            set { UpdateRush(value); }
        }


        public void UpdateJobID(int jobID)
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string updateString = "UPDATE [Sales_JobMasterList_WO] SET [jobID] = @jobID WHERE [woID] = @woID";
                var updateCommand = new SqlCommand(updateString, connection);

                updateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
                updateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = jobID;
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

        public void ChangeWorkorderStatus(int woStatus)
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string updateString =
                    "UPDATE [Sales_JobMasterList_WO] SET [woStatus] = @woStatus WHERE [woID] = @woID";
                var updateCommand = new SqlCommand(updateString, connection);

                updateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
                updateCommand.Parameters.Add("@woStatus", SqlDbType.Int).Value = woStatus;
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

        public void StatusUpdate_SiteCheck(string sitecheck)
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string updateString =
                    "UPDATE [Sales_JobMasterList_WO] SET [SiteCheck] = @SiteCheck WHERE [woID] = @woID";
                var updateCommand = new SqlCommand(updateString, connection);

                updateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
                updateCommand.Parameters.Add("@SiteCheck", SqlDbType.VarChar, 25).Value = sitecheck;
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

        public void StatusUpdate_structuralDwgPreparation(string structuralDwgPreparation)
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string updateString =
                    "UPDATE [Sales_JobMasterList_WO] SET [structuralDwgPreparation] = @structuralDwgPreparation WHERE [woID] = @woID";
                var updateCommand = new SqlCommand(updateString, connection);

                updateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
                updateCommand.Parameters.Add("@structuralDwgPreparation", SqlDbType.VarChar, 25).Value =
                    structuralDwgPreparation;
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

        public void StatusUpdate_graphicDwgPreparation(string graphicDwgPreparation)
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string updateString =
                    "UPDATE [Sales_JobMasterList_WO] SET [graphicDwgPreparation] = @graphicDwgPreparation WHERE [woID] = @woID";
                var updateCommand = new SqlCommand(updateString, connection);

                updateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
                updateCommand.Parameters.Add("@graphicDwgPreparation", SqlDbType.VarChar, 25).Value =
                    graphicDwgPreparation;
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


        private int GetCompanny1()
        {
            int cID = 0;
            var wo = new MyWorkorder(_woID);
            if (wo.Value != null)
            {
                cID = Convert.ToInt32(wo.Value.Company1);
            }

            return cID;
        }

        private void UpdateCompany1(int value)
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string updateString =
                    "UPDATE [Sales_JobMasterList_WO] SET [Company1] = @Company1 WHERE [woID] = @woID";
                var updateCommand = new SqlCommand(updateString, connection);

                updateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
                updateCommand.Parameters.Add("@Company1", SqlDbType.Int).Value = value;
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


        private int GetCompanny2()
        {
            int cID = 0;
            var wo = new MyWorkorder(_woID);
            if (wo.Value != null)
            {
                cID = Convert.ToInt32(wo.Value.Company2);
            }

            return cID;
        }


        private void UpdateCompany2(int value)
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string updateString =
                    "UPDATE [Sales_JobMasterList_WO] SET [Company2] = @Company2 WHERE [woID] = @woID";
                var updateCommand = new SqlCommand(updateString, connection);

                updateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
                updateCommand.Parameters.Add("@Company2", SqlDbType.Int).Value = value;
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


        private void UpdateContact1(int value)
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string updateString =
                    "UPDATE [Sales_JobMasterList_WO] SET [Contact1] = @Contact1 WHERE [woID] = @woID";
                var updateCommand = new SqlCommand(updateString, connection);

                updateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
                updateCommand.Parameters.Add("@Contact1", SqlDbType.Int).Value = value;
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


        private void UpdateContact2(int value)
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string updateString =
                    "UPDATE [Sales_JobMasterList_WO] SET [Contact2] = @Contact2 WHERE [woID] = @woID";
                var updateCommand = new SqlCommand(updateString, connection);

                updateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
                updateCommand.Parameters.Add("@Contact2", SqlDbType.Int).Value = value;
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


        private void UpdatePaymentMethod(int value)
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string updateString =
                    "UPDATE [Sales_JobMasterList_WO] SET [PayMethods] = @Paymethods WHERE [woID] = @woID";
                var updateCommand = new SqlCommand(updateString, connection);

                updateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
                updateCommand.Parameters.Add("@Paymethods", SqlDbType.Int).Value = value;
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

        private void UpdateRush(bool value)
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string updateString = "UPDATE [Sales_JobMasterList_WO] SET [Rush] = @Rush WHERE [woID] = @woID";
                var updateCommand = new SqlCommand(updateString, connection);

                updateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
                updateCommand.Parameters.Add("@Rush", SqlDbType.Bit).Value = value;
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
    }
}