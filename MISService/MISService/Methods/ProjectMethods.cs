using SalesCenterDomain.BDL.Project;
using SpecDomain.BLL.EstTitle;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;

namespace MISService.Method
{
    public class ProjectMethods
    {
        /// <summary>
        /// Create a new project
        /// </summary>
        /// <param name="userEmployeeID">it is employeenumber in FW_Employees</param>
        public void CreateNewProject(int userEmployeeID)
        {
            try
            {
                var job = new ProjectInsert(userEmployeeID);
                if (job.ValidationErrorID == 0)
                {
                    job.Insert();
                    int newJobID = job.JobID;

                    var cp = new ProjectCompany(newJobID);
                    cp.Insert(newJobID, 0, true, true, true);

                    var est = new MyEstRevCreate();
                    est.Create(newJobID);
                    LogMethods.Log.Debug("CreateNewProject:Debug:" + "DONE");
                }
                else
                {
                    LogMethods.Log.Error("CreateNewProject:Error:" + job.ValidationErrorID);
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("CreateNewProject:Crash:" + e.Message);
            }
        }

        /// <summary>
        /// Edit a project
        /// </summary>
        /// <param name="jobID"></param>
        /// <param name="targetDate"></param>
        /// <param name="sa1ID"></param>
        /// <param name="sales"></param>
        /// <param name="jobTitle"></param>
        public void EditNewProject(int jobID, DateTime targetDate, int sa1ID, int sales, string jobTitle)
        {
            using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE Sales_JobMasterList SET targetDate = @targetDate, sa1ID = @sa1ID, sales = @sales, jobTitle = @jobTitle  WHERE (jobID = @jobID)";
                var UpdateCommand = new SqlCommand(UpdateString, Connection);

                UpdateCommand.Parameters.Add("@targetDate", SqlDbType.DateTime).Value = targetDate;
                UpdateCommand.Parameters.Add("@sa1ID", SqlDbType.Int).Value = sa1ID;
                UpdateCommand.Parameters.Add("@sales", SqlDbType.Int).Value = sales;
                UpdateCommand.Parameters.Add("@sales", SqlDbType.VarChar, 150).Value = jobTitle;
                UpdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = jobID;
                try
                {
                    Connection.Open();
                    UpdateCommand.ExecuteNonQuery();
                    LogMethods.Log.Debug("EditNewProject:Debug:" + "DONE");
                }
                catch (SqlException ex)
                {
                    LogMethods.Log.Error("EditNewProject:Crash:" + ex.Message);
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        /// <summary>
        /// delete a project
        /// </summary>
        /// <param name="jobID"></param>
        public void DeleteProject(int jobID)
        {
            int estRevID = 0;
            try
            {
                using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
                {
                    string SqlSelectString = "SELECT EstRevID FROM [Sales_JobMasterList_EstRev] WHERE ([jobID] = @jobID)";
                    var SelectCommand = new SqlCommand(SqlSelectString, Connection);
                    SelectCommand.Parameters.AddWithValue("@jobID", jobID);
                    Connection.Open();
                    using (SqlDataReader dr = SelectCommand.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            estRevID = Convert.ToInt32(dr[0].ToString());
                            break;
                        }
                    }
                    Connection.Close();
                }

                if (estRevID != 0)
                {
                    using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
                    {
                        Connection.Open();
                        string SqlDelString = "DELETE FROM EST_Cost_Configuration WHERE ([EstRevID] = @EstRevID)";
                        var DelCommand = new SqlCommand(SqlDelString, Connection);
                        DelCommand.Parameters.AddWithValue("@EstRevID", estRevID);
                        DelCommand.ExecuteNonQuery();
                        Connection.Close();
                    }

                    using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
                    {
                        Connection.Open();
                        string SqlDelString = "DELETE FROM Sales_JobMasterList_EstRev WHERE ([EstRevID] = @EstRevID)";
                        var DelCommand = new SqlCommand(SqlDelString, Connection);
                        DelCommand.Parameters.AddWithValue("@EstRevID", estRevID);
                        DelCommand.ExecuteNonQuery();
                        Connection.Close();
                    }
                }

                using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
                {
                    Connection.Open();
                    string SqlDelString = "DELETE FROM Sales_JobMasterList_Customer WHERE ([jobID] = @jobID)";
                    var DelCommand = new SqlCommand(SqlDelString, Connection);
                    DelCommand.Parameters.AddWithValue("@jobID", jobID);
                    DelCommand.ExecuteNonQuery();
                    Connection.Close();
                }

                using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
                {
                    Connection.Open();
                    string SqlDelString = "DELETE FROM Sales_JobStatusTable WHERE ([jobID] = @jobID)";
                    var DelCommand = new SqlCommand(SqlDelString, Connection);
                    DelCommand.Parameters.AddWithValue("@jobID", jobID);
                    DelCommand.ExecuteNonQuery();
                    Connection.Close();
                }

                using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
                {
                    Connection.Open();
                    string SqlDelString = "DELETE FROM Sales_JobMasterList WHERE ([jobID] = @jobID)";
                    var DelCommand = new SqlCommand(SqlDelString, Connection);
                    DelCommand.Parameters.AddWithValue("@jobID", jobID);
                    DelCommand.ExecuteNonQuery();
                    Connection.Close();
                }
                LogMethods.Log.Debug("DeleteProject:Debug:" + "DONE");
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("DeleteProject:Crash:" + e.Message);
            }
            
        }

    }
}
