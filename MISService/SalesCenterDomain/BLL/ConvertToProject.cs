using System;
using System.Linq;
using EmployeeDomain;
using ExportDomain.Parameter;
using MyCommon.MyEnum;
using ProjectDomain;
using SalesCenterDomain.BO;
using SpecDomain.Model;

namespace SalesCenterDomain.BLL
{
     public class ConvertToProject
    {
        public int NewJobID { get; set; }

        private readonly ProjectModelDbEntities _dbProject = new ProjectModelDbEntities();
        private readonly ExportParameter _ep;

        public ConvertToProject( int eID)
        {
        	//Get Parameters
			var epCrud = new ExportParameterCrud(eID);
			_ep = epCrud.Parameter;
        }
         
        //CustomerID = _customerID,
        //CustomInt1 = _contactID,
        //CustomInt2 = _lead.LeadID,
        //CustomString1 = _lead.CompanyName,
        //Kam = _lead.AccountExcutive,

        public void ConvertTo()
        {
            try
            {
               var jobID = CreateNewProject();
                NewJobID = jobID;

                CreateNewProjectCustomers(jobID, _ep.CustomerID, _ep.CustomInt1);
                var estRevID = CreateNewEstimation(jobID);

                var ms = new MySalesStatusTable(jobID,_ep.PrintingEmployeeID );
                ms.Create( Convert.ToInt16(NJobStatus.ProjectNew));
                ms.Create( Convert.ToInt16(NJobStatus.EstimationNew ));
                
            }

            catch (Exception ex)
            {
                var s = ex.Message;
                throw;
            }
      
        }

  
        private int CreateNewProject()
        {
            //Insert New Record
            var job = new Sales_JobMasterList();
            job.jobNumber = "ToBeAssigned";
            job.sales = _ep.Kam;
            job.sa1ID = Convert .ToInt32(EmployeeEN.NEmployeeIDDefault.NullSales110);
            job.startTimeStamp = DateTime.Now;
            job.JobStatus = Convert.ToInt16(NJobStatus.ProjectNew);
            job.subProject = 0;
            job.yearNumber = Convert.ToInt16(System.DateTime.Today.Year - 2000);
            job.jobTitle = "From Lead -" + _ep.CustomString1;
            job.StatusChanged = true;
            job.ProductLine = 1;                    // Convert.ToInt32(CommissionEN.ProductLine.Signage);
            job.Rush = true;
            job.Customer = _ep.CustomerID;
            job.LeadID = _ep.CustomInt2;
            job.targetDate = DateTime.Today.AddMonths(1);

            _dbProject.Sales_JobMasterList.Add(job);
            _dbProject.SaveChanges();

            //Job Number
            job.jobNumber = GetJobNumber(job.jobID);
			 
            return job.jobID;
        }

        private string GetJobNumber(int jobID)
        {
            //yy(year)+P+D5

            var jNumber = "ToBeAssigned";

            int yearNumber = Convert.ToInt16(System.DateTime.Today.Year - 2000);

            jNumber = Convert.ToString(yearNumber) + "P";

            var n1 = _dbProject.Sales_JobMasterList.Count(x => x.yearNumber == yearNumber &x.subProject==0);
            
            if (n1 > 0)
            {
                n1 = n1 + SalesCenterConstants.BEGIN_PROJECT_STARTER_SEED;

            }

            jNumber += n1.ToString("D5");

            //if error, xxP0000 is returned
            return jNumber;

        }

        private void CreateNewProjectCustomers(int projectID, int customerID, int contactID)
        {
            //Insert New Record
            var pCustomer = new Sales_JobMasterList_Customer()
                                {
                                    jobID = projectID,
                                    cID = customerID,
                                    contactName = contactID,
                                    isBillTo = true,
                                    isInstallOrShipTo = true,
                                    isQuoteTo = true

                                };

            try
            {

            _dbProject.Sales_JobMasterList_Customer.Add(pCustomer);
            _dbProject.SaveChanges();
           
            }
       
  
            catch (Exception ex)
            {
                var s = ex.Message;
                throw;
            }

        }

        private int  CreateNewEstimation(int projectID)
        {
           var dbSpec = new SpecificationDbEntities() ;
            var est = new Sales_JobMasterList_EstRev()
                          {
                              JobID = projectID,
                              erRev = 1,
                              sa1ID = 0
                          };
            
            dbSpec.Sales_JobMasterList_EstRev.Add(est);
            dbSpec.SaveChanges();

            return est.EstRevID;
        }

         
    }
}