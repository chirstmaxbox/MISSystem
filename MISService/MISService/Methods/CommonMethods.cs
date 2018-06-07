using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using enterprise = MISService.SfdcReference;
using System.ServiceModel;
using MISService.Models;
using MISService.Method;
using System.Data.SqlClient;

namespace MISService.Methods
{
    public class CommonMethods
    {
        public static string GetUserName(string Id)
        {
            string un = string.Empty;

            EndpointAddress apiAddr;
            enterprise.SessionHeader header;

            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;

            using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
            {
                //create SQL query statement
                string query = "SELECT CommunityNickname FROM User WHERE Id = '" + Id + "'";

                enterprise.QueryResult result;
                queryClient.query(
                    header, //sessionheader
                    null, //queryoptions
                    null, //mruheader
                    null, //packageversion
                    query, out result);

                //cast query results
                IEnumerable<enterprise.User> userList = result.records.Cast<enterprise.User>();
                foreach (var user in userList)
                {
                    un = user.CommunityNickname;
                    break;
                }
            }

            return un;
        }

        public static int GetMISID(string tableName, string salesforceID)
        {
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            int MISID = 0;
            try
            {
                string SqlSelectString = "SELECT MISID FROM [MISSalesForceMapping] WHERE ([TableName] = @tableName) and ([SalesForceID] = @salesforceID)";
                var SelectCommand = new SqlCommand(SqlSelectString, Connection);
                SelectCommand.Parameters.AddWithValue("@tableName", tableName);
                SelectCommand.Parameters.AddWithValue("@salesforceID", salesforceID);
                Connection.Open();
                using (SqlDataReader dr = SelectCommand.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        MISID = Convert.ToInt32(dr[0].ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("GetMISID:Crash:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return MISID;
        }

        public static bool InsertToMISSalesForceMapping(string tableName, string salesforceID, string MISID)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlSelectString = "INSERT INTO [MISSalesForceMapping] VALUES (@tableName, @salesforceID, @MISID)";
                var SelectCommand = new SqlCommand(SqlSelectString, Connection);
                SelectCommand.Parameters.AddWithValue("@tableName", tableName);
                SelectCommand.Parameters.AddWithValue("@salesforceID", salesforceID);
                SelectCommand.Parameters.AddWithValue("@MISID", MISID);
                Connection.Open();
                SelectCommand.ExecuteNonQuery();
                ret = true;
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("InsertToMISSalesForceMapping:Crash:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

    }
}
