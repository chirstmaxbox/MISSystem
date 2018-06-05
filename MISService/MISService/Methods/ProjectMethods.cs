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
using enterprise = MISService.SfdcReference;
using System.ServiceModel;
using MISService.Models;
using EmployeeDomain.BLL;
using MISService.Methods;

namespace MISService.Method
{
    public class ProjectMethods
    {
        private EndpointAddress apiAddr;
        private enterprise.SessionHeader header;

        public ProjectMethods() {
            //set query endpoint to value returned by login request
            apiAddr = new EndpointAddress(SalesForceMethods.serverUrl);

            //instantiate session header object and set session id
            header = new enterprise.SessionHeader();
            header.sessionId = SalesForceMethods.sessionId;
        }

        public void GetAllProjects()
        {
            try
            {
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Project_Number__c, Name, CloseDate, OwnerId FROM Opportunity";

                    enterprise.QueryResult result;
                    queryClient.query(
                        header, //sessionheader
                        null, //queryoptions
                        null, //mruheader
                        null, //packageversion
                        query, out result);

                    //cast query results
                    IEnumerable<enterprise.Opportunity> opportunityList = result.records.Cast<enterprise.Opportunity>();

                    //show results
                    foreach (var opportunity in opportunityList)
                    {
                        /* get project owner */
                        string un = CommonMethods.GetUserName(opportunity.OwnerId);
                        FsEmployee fsEmployee = new FsEmployee(un);
                        if (fsEmployee.EmployeeNumber > 0)
                        {
                            string projectID = CommonMethods.GetMISID(TableName.Sales_JobMasterList, opportunity.Id);
                            if (string.IsNullOrEmpty(projectID))
                            {
                                int jobID = CreateNewProject(fsEmployee.EmployeeNumber);
                                UpdateProject(jobID, opportunity.CloseDate, 110, fsEmployee.EmployeeNumber, opportunity.Name);
                                /* update jobnumber */
                                UpdateJobNumber(jobID, opportunity.Project_Number__c);
                                /* insert data to MISSalesForceMapping */
                                CommonMethods.InsertToMISSalesForceMapping(TableName.Sales_JobMasterList, opportunity.Id, jobID.ToString());
                            }
                            else
                            {
                                UpdateProject(Convert.ToInt32(projectID), opportunity.CloseDate, 110, fsEmployee.EmployeeNumber, opportunity.Name);
                            }
                        }
                    }

                    LogMethods.Log.Debug("GetAllProjects:Debug:" + "DONE");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("GetAllProjects:Error:" + e.Message);
            }
        }

        /// <summary>
        /// Create a new project
        /// </summary>
        /// <param name="userEmployeeID">it is employeenumber in FW_Employees</param>
        public int CreateNewProject(int userEmployeeID)
        {
            int newJobID = 0;
            try
            {
                var job = new ProjectInsert(userEmployeeID);
                if (job.ValidationErrorID == 0)
                {
                    job.Insert();
                    newJobID = job.JobID;

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

            return newJobID;
        }

        /// <summary>
        /// Edit a project
        /// </summary>
        public void UpdateProject(int jobID, DateTime? targetDate, int sa1ID, int sales, string jobTitle)
        {
            using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE Sales_JobMasterList SET targetDate = @targetDate, sa1ID = @sa1ID, sales = @sales, jobTitle = @jobTitle  WHERE (jobID = @jobID)";
                var UpdateCommand = new SqlCommand(UpdateString, Connection);
                if (targetDate != null)
                {
                    UpdateCommand.Parameters.Add("@targetDate", SqlDbType.DateTime).Value = targetDate;
                }
                else
                {
                    UpdateCommand.Parameters.Add("@targetDate", SqlDbType.DateTime).Value = DBNull.Value;
                }
                UpdateCommand.Parameters.Add("@sa1ID", SqlDbType.Int).Value = sa1ID;
                UpdateCommand.Parameters.Add("@sales", SqlDbType.Int).Value = sales;
                UpdateCommand.Parameters.Add("@jobTitle", SqlDbType.VarChar, 150).Value = jobTitle;
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

        private void UpdateJobNumber(int jID, string jobNum)
        {
            using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE [Sales_JobMasterList] SET [jobNumber] = @jobNumber WHERE [jobID] = @jobID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = jID;
                UPdateCommand.Parameters.Add("@jobNumber", SqlDbType.VarChar, 15).Value = jobNum;
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
