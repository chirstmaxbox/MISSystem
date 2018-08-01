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
                LogMethods.Log.Info("GetAllProjects:Info:" + "Start processing all projects");
                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Id, Project_Number__c, Name, CloseDate, Issue_Date__c, Type, OwnerId, Owner.CommunityNickname, Bidding_Type__c, Bidding_Source__c, Product_Line__c, "
                                           + " Bidding_Due_Date__c, Bidding_Remark__c, Sync__c, Account_Executive__r.CommunityNickname, "
                                           + " (SELECT Id, Name, Billing_Company_City__c, Billing_Contact_Name__r.Account.Id, Billing_Company_Name__r.Name, Billing_Company_Name__r.Id, Billing_Company_Postal_Code__c, Billing_Company_Province__c, Billing_Company_Street__c, Billing_Contact_Name__r.FirstName, Billing_Contact_Name__r.LastName, Billing_Contact_Name__r.Id, Billing_Contact_Phone__c, Billing_Company_Country__c, Quoting_Company_City__c, Quoting_Company_Name__r.Name, Quoting_Company_Name__r.Id,  Quoting_Contact_Name__r.Account.Id, Quoting_Company_Postal_Code__c, Quoting_Company_Province__c, Quoting_Company_Street__c, Quoting_Contact_Name__r.FirstName, Quoting_Contact_Name__r.LastName, Quoting_Contact_Name__r.Id, Quoting_Contact_Phone__c, Quoting_Company_Country__c,Installing_Company_City__c, Installing_Company_Name__r.Name, Installing_Company_Name__r.Id, Installing_Contact_Name__r.Account.Id, Installing_Company_Postal_Code__c, Installing_Company_Province__c, Installing_Company_Street__c, Installing_Contact_Name__r.FirstName, Installing_Contact_Name__r.LastName, Installing_Contact_Name__r.Id, Installing_Contact_Phone__c, Installing_Company_Country__c FROM Bill_Quote_Ships__r), "
                                           + " (SELECT Id, Number_of_Signs__c, Project_Value_Estimated__c,  Remarks__c, Issue_Date__c, Due_Date__c, LandLord__r.Name, LandLord_Contact__r.Name, LandLord_Phone_Number__c, LandLord__r.BillingStreet, LandLord__r.BillingCity, LandLord__r.BillingState, LandLord__r.BillingPostalCode FROM Sign_Permits__r),"
                                           + " (SELECT Id, Occupation_Start_Time__c, Occupation_End_Time__c, Issue_Date__c, Type_Of_Truck__c, Truck_Weight__c, Foreman_Name__c, Foreman_Phone__c, Remarks__c FROM Hoisting_Permits__r),"
                                           + " (SELECT Id, Stick_Position_Radius__c, Dept_Of_Holes__c, Issue_Date__c, Due_Date__c, Remarks__c FROM StakeOut_Permits__r),"
                                           + " (SELECT Id, Name, First_Site_Contact__c, Second_Site_Contact__c, Budget__c, Provided_By__c,  Remarks__c, Due_Date__c, Rush__c, Requirement__c, Requirement_As_Other__c, Estimated_Shipping_Cost__c, Shipping_Items_Total_Value__c, Work_Order_Number__c  FROM SubContracts__r) "
                                           + " FROM Opportunity "
                                           + " WHERE Sync__c = true and CloseDate >= TODAY";

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
                        //string un = CommonMethods.GetUserName(opportunity.OwnerId);
                        string un = (opportunity.Owner.CommunityNickname == null ? "" : opportunity.Owner.CommunityNickname);
                        FsEmployee fsEmployee = new FsEmployee(un);
                        if (fsEmployee.EmployeeNumber > 0)
                        {
                            LogMethods.Log.Info("GetAllProjects:Info:" + "Processing project name:" + opportunity.Name);
                            int sales_JobMasterListID = CommonMethods.GetMISID(TableName.Sales_JobMasterList, opportunity.Id, opportunity.Id);
                            if (sales_JobMasterListID == 0)
                            {
                                int jobID = CreateNewProject(fsEmployee.EmployeeNumber);
                                UpdateProject(jobID, opportunity.CloseDate, 110, fsEmployee.EmployeeNumber, opportunity.Name, opportunity.Type, opportunity.Account_Executive__r, opportunity.Product_Line__c);
                                /* update jobnumber */
                                UpdateJobNumber(jobID, opportunity.Project_Number__c);
                                /* insert data to MISSalesForceMapping */
                                if (jobID > 0)
                                {
                                    CommonMethods.InsertToMISSalesForceMapping(TableName.Sales_JobMasterList, opportunity.Id, jobID.ToString(), opportunity.Id);
                                }
                                else
                                {
                                    LogMethods.Log.Error("GetAllProjects:Error:" + "Cannot create a new project!");
                                    continue;
                                }
                                sales_JobMasterListID = jobID;
                            }
                            else
                            {
                                UpdateProject(sales_JobMasterListID, opportunity.CloseDate, 110, fsEmployee.EmployeeNumber, opportunity.Name, opportunity.Type, opportunity.Account_Executive__r, opportunity.Product_Line__c);
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
                            cm.GetAllAccounts(opportunity.Id, sales_JobMasterListID, fsEmployee.EmployeeNumber, opportunity.Bill_Quote_Ships__r);
                            
                            /* Estimation */
                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing estimation");
                            EstimationMethods em = new EstimationMethods(opportunity.Id);
                            int estRevID = CommonMethods.GetEstRevID(sales_JobMasterListID);
                            em.GetEstimation(opportunity.Id, estRevID, sales_JobMasterListID, fsEmployee.EmployeeNumber);

                            /*Drawing */
                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing drawing");
                            DrawingMethods dm = new DrawingMethods(opportunity.Id);
                            dm.GetAllDrawings(opportunity.Id, estRevID, sales_JobMasterListID, fsEmployee.EmployeeNumber);

                            /* Quote */
                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing quote");
                            QuoteMethods qm = new QuoteMethods(opportunity.Id);
                            qm.GetAllQuotes(opportunity.Id, sales_JobMasterListID, estRevID, fsEmployee.EmployeeNumber);

                            /* Sign/Hoisting/Stakeout permit */
                            PermitMethods pm = new PermitMethods(opportunity.Id);
                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing sign permit");
                            pm.GetAllSignPermits(opportunity.Id, sales_JobMasterListID, fsEmployee.EmployeeNumber, opportunity.Sign_Permits__r);
                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing hoisting permit");
                            pm.GetAllHoistingPermits(opportunity.Id, sales_JobMasterListID, fsEmployee.EmployeeNumber, opportunity.Hoisting_Permits__r);
                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing stakeout permit");
                            pm.GetAllStakeOutPermits(opportunity.Id, sales_JobMasterListID, fsEmployee.EmployeeNumber, opportunity.StakeOut_Permits__r);

                            /* WorkOrder */
                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing work order");
                            WorkOrderMethods wo = new WorkOrderMethods(opportunity.Id);
                            wo.GetAllWorkOrders(opportunity.Id, sales_JobMasterListID, estRevID, fsEmployee.EmployeeNumber);

                            /* Sub-Contract */
                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing SubContract");
                            SubContractMethods sc = new SubContractMethods(opportunity.Id);
                            sc.GetAllSubContracts(opportunity.Id, sales_JobMasterListID, fsEmployee.EmployeeNumber, opportunity.SubContracts__r);

                            /* Invoice */
                            LogMethods.Log.Debug("GetAllProjects:Debug:" + "Processing invoice");
                            InvoiceMethods im = new InvoiceMethods(opportunity.Id);
                            im.GetAllInvoices(opportunity.Id, sales_JobMasterListID, estRevID, fsEmployee.EmployeeNumber);

                            LogMethods.Log.Info("GetAllProjects:Info:" + "Done: " + opportunity.Name + "<Project Num:" + opportunity.Project_Number__c + ">");
                        }
                        else
                        {
                            LogMethods.Log.Error("GetAllProjects:Error:" + "User Name: " + un + " does not exist in database");
                        }
                    }
                    LogMethods.Log.Info("GetAllProjects:Info:" + "All projects are done");
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
        private void UpdateProject(int jobID, DateTime? targetDate, int sa1ID, int sales, string jobTitle, string salesType, enterprise.User AE, string productLine)
        {
            using (var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE Sales_JobMasterList SET targetDate = @targetDate, sa1ID = @sa1ID, sales = @sales, jobTitle = @jobTitle, salesType = @salesType, isBidToProject = @isBidToProject, AETM = @AETM, ProductLine = @ProductLine WHERE (jobID = @jobID)";
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
                        UpdateCommand.Parameters.AddWithValue("@isBidToProject", 0);
                        break;
                    case SalesType.Bid:
                        UpdateCommand.Parameters.AddWithValue("@salesType", 2);
                        UpdateCommand.Parameters.AddWithValue("@isBidToProject", 1);
                        break;
                    default:
                        UpdateCommand.Parameters.AddWithValue("@salesType", 1);
                        UpdateCommand.Parameters.AddWithValue("@isBidToProject", 0);
                        break;
                }

                UpdateCommand.Parameters.Add("@sa1ID", SqlDbType.Int).Value = sa1ID;
                UpdateCommand.Parameters.Add("@sales", SqlDbType.Int).Value = sales;
                UpdateCommand.Parameters.Add("@jobTitle", SqlDbType.VarChar, 150).Value = jobTitle;
                UpdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = jobID;

                if (AE != null)
                {
                    //string un = CommonMethods.GetUserName(AE.Id);
                    string un = AE.CommunityNickname == null ? "" :  AE.CommunityNickname;
                    FsEmployee fsEmployee = new FsEmployee(un);
                    if (fsEmployee.EmployeeNumber > 0)
                    {
                        UpdateCommand.Parameters.AddWithValue("@AETM", fsEmployee.EmployeeNumber);
                    }
                    else
                    {
                        LogMethods.Log.Error("UpdateProject:Error:" + "User Name: " + un + " does not exist in database");
                        UpdateCommand.Parameters.AddWithValue("@AETM", 0);
                    }
                }
                else
                {
                    UpdateCommand.Parameters.AddWithValue("@AETM", 0);
                }

                if (productLine != null)
                {
                    switch (productLine)
                    {
                        case "Signage":
                            UpdateCommand.Parameters.AddWithValue("@ProductLine", 1);
                            break;
                        default:
                            UpdateCommand.Parameters.AddWithValue("@ProductLine", 10);
                            break;
                    }
                }
                else
                {
                    UpdateCommand.Parameters.AddWithValue("@ProductLine", 1);
                }

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
