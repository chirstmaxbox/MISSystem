using System;
using System.Data;
using System.Data.SqlClient;
using CustomerDomain.BO;


namespace CustomerDomain.BDL
{
    public class CustomerList
    {
        private DataTable _customerTable = null;
        public DataTable CustomerTable
        {
            get { return _customerTable; }
        }

        public CustomerList(int Category, int companyType, int SalesID)
        {
            _customerTable = GetCustomerList(Category, companyType, SalesID);
        }

        private DataTable GetCustomerList(int Category, int companyType, int SalesID)
        {
            //Select All:
            //Category=0 and CompanyType=0
            DataTable t1 = null;

            SqlConnection ConnectionSQL = new SqlConnection(CustomerDomainConfiguration.ConnectionString);


            int NumRowsAffected = 0;
            SqlCommand SelectCommand = new SqlCommand("getLevelOneCustomerWithSalesName", ConnectionSQL);

            //Stored Procedure
            SelectCommand.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter1 = new SqlDataAdapter(SelectCommand);

            adapter1.SelectCommand.Parameters.Add("@Category", SqlDbType.Int).Value = Category;
            adapter1.SelectCommand.Parameters.Add("@cType", SqlDbType.Int).Value = companyType;
            adapter1.SelectCommand.Parameters.Add("@salesID", SqlDbType.Int).Value = SalesID;

            DataSet ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
                {
                    t1 = ds1.Tables["t1"];
                }

            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;

            }
            finally
            {
                ConnectionSQL.Close();
            }

            return t1;

        }
        

        public CustomerList(CustomerListParameters clp)
        {
            //if (clp.SalesID ==(int)EmployeeEN .NEmployeeIDDefault .Other )
            //{
            //   _customerTable = GetCustomerListOther(clp);
            //}
            //else
            //{
            _customerTable = GetCustomerList(clp);        

            // }
        }


        //private DataTable GetCustomerListOther(CustomerListParameters clp)
        //{
        //    //all Ae
        //    //find ae in quota table
        //    //delete ae in quota table
        //    //return other
        //    clp.SalesID = (int) EmployeeEN.NEmployeeIDDefault.All;
        //    var allAEs = GetCustomerList(clp);

        //    var quota = new SalesQuotaOriginalData(DateTime.Today.Year);
        //    var quotaAEs = quota.QuataDatatable;

        //    foreach (DataRow row in allAEs.Rows)
        //    {
        //        var allAE = Convert.ToInt32(row["SalesID"]);
        //        foreach (DataRow rowQuota in quotaAEs.Rows)
        //        {
        //            var quotaAe = Convert.ToInt32(rowQuota["SalesID"]);
        //            if (allAE == quotaAe)
        //            {
        //                row.Delete();
        //            }
        //        }
        //    }

        //    allAEs.AcceptChanges();
        //    return allAEs;

        //}


        private DataTable GetCustomerList(CustomerListParameters clp)
        {

            DataTable t1 = null;
            SqlConnection ConnectionSQL = new SqlConnection(CustomerDomainConfiguration.ConnectionString);

            SqlCommand SelectCommand = new SqlCommand("GetCustomerList", ConnectionSQL);

            //Stored Procedure
            SelectCommand.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter1 = new SqlDataAdapter(SelectCommand);

            adapter1.SelectCommand.Parameters.Add("@Headoffice", SqlDbType.Bit).Value = clp.CbxHeadoffice;
            adapter1.SelectCommand.Parameters.Add("@Franchisee", SqlDbType.Bit).Value = clp.CbxFranchisee;
            adapter1.SelectCommand.Parameters.Add("@IndividualStore", SqlDbType.Bit).Value = clp.CbxIndividualStore;
            adapter1.SelectCommand.Parameters.Add("@Partner", SqlDbType.Bit).Value = clp.CbxPartner;
            
            adapter1.SelectCommand.Parameters.Add("@TeamID", SqlDbType.Int).Value = clp.TeamID;
            adapter1.SelectCommand.Parameters.Add("@salesID", SqlDbType.Int).Value = clp.SalesID;
            adapter1.SelectCommand.Parameters.Add("@FirstCharacter", SqlDbType.NChar, 3).Value = clp.FirstCharacterOfName;
            adapter1.SelectCommand.Parameters.Add("@CategoryID", SqlDbType.Int).Value = clp.IndustryID;
            adapter1.SelectCommand.Parameters.Add("@CustomerID", SqlDbType.Int).Value = clp.CustomerrID ;

            DataSet ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
                {
                    t1 = ds1.Tables["t1"];
                }

            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;

            }
            finally
            {
                ConnectionSQL.Close();
            }

            return t1;

        }


    }
}