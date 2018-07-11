﻿using SalesCenterDomain.BDL.Project;
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
using SalesCenterDomain.BDL;

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
                LogMethods.Log.Debug("GetAllProjects:Debug:" + "Start processing all projects");
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Project_Number__c, Name, CloseDate, Issue_Date__c, Type, OwnerId, Bidding_Type__c, Bidding_Source__c,"
                                           + " Bidding_Due_Date__c, Bidding_Remark__c, Sync__c FROM Opportunity where Sync__c = true and CloseDate >= TODAY"; 

                    enterprise.QueryResult result;
                    queryClient.query(
                        header, //sessionheader
                        null, //queryoptions
                        null, //mruheader
                        null, //packageversion
                        query, out result);

                    /* if no any record, return */
                    if(result.size == 0) return;

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
                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing project name:" + opportunity.Name);
                            int sales_JobMasterListID = CommonMethods.GetMISID(TableName.Sales_JobMasterList, opportunity.Id, opportunity.Id);
                            if (sales_JobMasterListID == 0)
                            {
                                int jobID = CreateNewProject(fsEmployee.EmployeeNumber);
                                UpdateProject(jobID, opportunity.CloseDate, 110, fsEmployee.EmployeeNumber, opportunity.Name, opportunity.Type);
                                /* update jobnumber */
                                UpdateJobNumber(jobID, opportunity.Project_Number__c);
                                /* insert data to MISSalesForceMapping */
                                CommonMethods.InsertToMISSalesForceMapping(TableName.Sales_JobMasterList, opportunity.Id, jobID.ToString(), opportunity.Id);
                                sales_JobMasterListID = jobID;
                            }
                            else
                            {
                                UpdateProject(sales_JobMasterListID, opportunity.CloseDate, 110, fsEmployee.EmployeeNumber, opportunity.Name, opportunity.Type);
                            }

                            /* for bidding project */
                            if (opportunity.Type == SalesType.Bid)
                            {
                                /* check if the bidding record exists */
                                int biddingID = GetBiddingID(sales_JobMasterListID);
                                if (biddingID > 0)
                                {
                                    //exist
                                    UpdateBiddingProject(biddingID, sales_JobMasterListID, opportunity.Bidding_Type__c, opportunity.Bidding_Source__c, opportunity.Bidding_Due_Date__c, opportunity.Bidding_Remark__c);
                                }
                                else
                                {
                                    InsertBiddingProject(fsEmployee.EmployeeNumber);
                                    UpdateBiddingProject(SqlCommon.GetNewlyInsertedRecordID("Sales_JobMaster_BiddingJob"), Convert.ToInt32(sales_JobMasterListID), opportunity.Bidding_Type__c, opportunity.Bidding_Source__c, opportunity.Bidding_Due_Date__c, opportunity.Bidding_Remark__c);
                                }
                            }

                            /* Bill-Quote-Ship */
                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing account and contact");
                            CustomerMethods cm = new CustomerMethods(opportunity.Id);
                            cm.GetAllAccounts(opportunity.Id, sales_JobMasterListID, fsEmployee.EmployeeNumber);
                            
                            /* Get Estimation and Items and Services */
                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing estimation");
                            EstimationMethods em = new EstimationMethods(opportunity.Id);
                            int estRevID = CommonMethods.GetEstRevID(sales_JobMasterListID);
                            em.GetEstimation(opportunity.Id, estRevID, sales_JobMasterListID);

                             /* Get Drawing */
                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing drawing");
                            DrawingMethods dm = new DrawingMethods(opportunity.Id);
                            dm.GetAllDrawings(opportunity.Id, estRevID, sales_JobMasterListID);

                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing quote");
                            QuoteMethods qm = new QuoteMethods(opportunity.Id);
                            qm.GetAllQuotes(opportunity.Id, sales_JobMasterListID, estRevID, fsEmployee.EmployeeNumber);

                            PermitMethods pm = new PermitMethods(opportunity.Id);
                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing sign permit");
                            pm.GetAllSignPermits(opportunity.Id, sales_JobMasterListID, fsEmployee.EmployeeNumber);
                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing hoisting permit");
                            pm.GetAllHoistingPermits(opportunity.Id, sales_JobMasterListID, fsEmployee.EmployeeNumber);
                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing stakeout permit");
                            pm.GetAllStakeOutPermits(opportunity.Id, sales_JobMasterListID, fsEmployee.EmployeeNumber);

                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing work order");
                            WorkOrderMethods wo = new WorkOrderMethods(opportunity.Id);
                            wo.GetAllWorkOrders(opportunity.Id, sales_JobMasterListID, estRevID, fsEmployee.EmployeeNumber);

                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing invoice");
                            InvoiceMethods im = new InvoiceMethods(opportunity.Id);
                            im.GetAllInvoices(opportunity.Id, sales_JobMasterListID, estRevID, fsEmployee.EmployeeNumber);

                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Done " + opportunity.Project_Number__c);
                        }
                        else
                        {
                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "User Name: " + un + " does not exist in database");
                        }
                    }
                    LogMethods.Log.Debug("GetAllProjects:Debug:" + "All projects are done");
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
        private int CreateNewProject(int userEmployeeID)
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
                    LogMethods.Log.Debug("CreateNewProject:Debug:" + "Done");
                }
                else
                {
                    LogMethods.Log.Error("CreateNewProject:Error:" + job.ValidationErrorID);
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("CreateNewProject:Error:" + e.Message);
            }

            return newJobID;
        }

        /// <summary>
        /// Edit a project
        /// </summary>
        private void UpdateProject(int jobID, DateTime? targetDate, int sa1ID, int sales, string jobTitle, string salesType)
        {
            using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE Sales_JobMasterList SET targetDate = @targetDate, sa1ID = @sa1ID, sales = @sales, jobTitle = @jobTitle, salesType = @salesType WHERE (jobID = @jobID)";
                var UpdateCommand = new SqlCommand(UpdateString, Connection);
                if (targetDate != null)
                {
                    UpdateCommand.Parameters.Add("@targetDate", SqlDbType.DateTime).Value = targetDate;
                }
                else
                {
                    UpdateCommand.Parameters.Add("@targetDate", SqlDbType.DateTime).Value = DBNull.Value;
                }

                switch (salesType)
                {
                    case SalesType.Repeat:
                        UpdateCommand.Parameters.AddWithValue("@salesType", 0);
                        break;
                    case SalesType.Bid:
                        UpdateCommand.Parameters.AddWithValue("@salesType", 2);
                        break;
                    default:
                        UpdateCommand.Parameters.AddWithValue("@salesType", 1);
                        break;
                }

                UpdateCommand.Parameters.Add("@sa1ID", SqlDbType.Int).Value = sa1ID;
                UpdateCommand.Parameters.Add("@sales", SqlDbType.Int).Value = sales;
                UpdateCommand.Parameters.Add("@jobTitle", SqlDbType.VarChar, 150).Value = jobTitle;
                UpdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = jobID;
                try
                {
                    Connection.Open();
                    UpdateCommand.ExecuteNonQuery();
                    LogMethods.Log.Debug("UpdateProject:Debug:" + "Done");
                }
                catch (SqlException ex)
                {
                    LogMethods.Log.Error("UpdateProject:Error:" + ex.Message);
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
                    LogMethods.Log.Debug("UpdateJobNumber:Debug:" + "Done");
                }
                catch (SqlException ex)
                {
                    LogMethods.Log.Error("UpdateJobNumber:Error:" + ex.Message);
                }
                finally
                {
                    Connection.Close();
                }
            }
        }


        private bool InsertBiddingProject(int userEmployeeID)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlSelectString = "INSERT INTO [Sales_JobMaster_BiddingJob] (JobID, CreatedBy, AeAssignedDate, BillToID, TempAE) VALUES (@jobID, @createdBy, @aeAssignedDate, @billToID, @tempAE)";
                var SelectCommand = new SqlCommand(SqlSelectString, Connection);
                SelectCommand.Parameters.AddWithValue("@jobID", 0);
                SelectCommand.Parameters.AddWithValue("@createdBy", userEmployeeID);
                SelectCommand.Parameters.AddWithValue("@aeAssignedDate", DateTime.Now);
                SelectCommand.Parameters.AddWithValue("@billToID", 0);
                SelectCommand.Parameters.AddWithValue("@tempAE", userEmployeeID);
                Connection.Open();
                SelectCommand.ExecuteNonQuery();
                ret = true;
                LogMethods.Log.Debug("InsertBiddingProject:Debug:" + "Done");
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("InsertBiddingProject:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

        private void UpdateBiddingProject(int biddingID, int jobID, string biddingType, string biddingSource, DateTime? deadline, string remark)
        {
            using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE [Sales_JobMaster_BiddingJob] SET [jobID] = @jobID, [BiddingTypeID] = @biddingTypeID, [BiddingSourceID] = @biddingSourceID, [BidDeadline] = @bidDeadline, [Remark] = @remark  WHERE [BiddingID] = @biddingID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);
                UPdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = jobID;
                UPdateCommand.Parameters.Add("@biddingID", SqlDbType.Int).Value = biddingID;

                switch (biddingType)
                {
                    case BiddingType.Hard_Bid:
                        UPdateCommand.Parameters.AddWithValue("@biddingTypeID", 20);
                        break;
                    case BiddingType.Soft_Bid:
                        UPdateCommand.Parameters.AddWithValue("@biddingTypeID", 10);
                        break;
                    default:
                        UPdateCommand.Parameters.AddWithValue("@biddingTypeID", 0);
                        break;
                }

                switch (biddingSource)
                {
                    case BiddingSource.BiddingGo:
                        UPdateCommand.Parameters.AddWithValue("@BiddingSourceID", 10);
                        break;
                    case BiddingSource.Merx:
                        UPdateCommand.Parameters.AddWithValue("@BiddingSourceID", 15);
                        break;
                    case BiddingSource.GC:
                        UPdateCommand.Parameters.AddWithValue("@BiddingSourceID", 20);
                        break;
                    case BiddingSource.Government:
                        UPdateCommand.Parameters.AddWithValue("@BiddingSourceID", 25);
                        break;
                    case BiddingSource.Developer:
                        UPdateCommand.Parameters.AddWithValue("@BiddingSourceID", 30);
                        break;
                    case BiddingSource.Others:
                        UPdateCommand.Parameters.AddWithValue("@BiddingSourceID", 99);
                        break;
                    default:
                        UPdateCommand.Parameters.AddWithValue("@BiddingSourceID", 0);
                        break;
                }

                if (deadline != null)
                {
                    UPdateCommand.Parameters.Add("@bidDeadline", SqlDbType.DateTime).Value = deadline;
                }
                else
                {
                    UPdateCommand.Parameters.Add("@bidDeadline", SqlDbType.DateTime).Value = DBNull.Value;
                }

                if (remark != null)
                {
                    UPdateCommand.Parameters.AddWithValue("@remark", remark);
                }
                else
                {
                    UPdateCommand.Parameters.AddWithValue("@remark", DBNull.Value);
                }

                try
                {
                    Connection.Open();
                    UPdateCommand.ExecuteNonQuery();
                    LogMethods.Log.Debug("UpdateBiddingProject:Debug:" + "Done");
                }
                catch (SqlException ex)
                {
                    string errorlog = ex.Message;
                    LogMethods.Log.Error("UpdateBiddingProject:Error:" + ex.Message);
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        private int GetBiddingID(int jobID)
        {
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            int biddingID = 0;
            try
            {
                string SqlSelectString = "SELECT BiddingID FROM [Sales_JobMaster_BiddingJob] WHERE ([JobID] = @jobID)";
                var SelectCommand = new SqlCommand(SqlSelectString, Connection);
                SelectCommand.Parameters.AddWithValue("@jobID", jobID);
                Connection.Open();
                using (SqlDataReader dr = SelectCommand.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        biddingID = Convert.ToInt32(dr[0].ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("GetBiddingID:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return biddingID;
        }
    }
}
