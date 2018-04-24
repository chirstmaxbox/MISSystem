//Obsolete, June , 2011

using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Project
{
    public class ProjectMarketing
    {
        private readonly int _jobID;
        private int _customerType;
        private int _product;

        private int _teamID;

        public ProjectMarketing(int jobID)
        {
            _jobID = jobID;
        }

        public int TeamID
        {
            set
            {
                _teamID = value;
                UpdateTeamID();
            }
        }

        public int CustomerType
        {
            set
            {
                _customerType = value;
                UpdateCustomerType();
            }
        }

        public int Product
        {
            set
            {
                _product = value;
                UpdateProduct();
            }
        }


        private void UpdateTeamID()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList] SET [MarketingTeam] = @MarketingTeam WHERE [jobID] = @jobID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;
                UPdateCommand.Parameters.Add("@MarketingTeam", SqlDbType.Int).Value = _teamID;
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


        private void UpdateProduct()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList] SET [MarketingProduct] = @MarketingProduct WHERE [jobID] = @jobID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;
                UPdateCommand.Parameters.Add("@MarketingProduct", SqlDbType.Int).Value = _product;
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


        private void UpdateCustomerType()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList] SET [MarketingAttribute] = @MarketingAttribute WHERE [jobID] = @jobID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;
                UPdateCommand.Parameters.Add("@MarketingAttribute", SqlDbType.Int).Value = _customerType;

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