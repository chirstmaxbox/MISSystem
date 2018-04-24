using System;
using System.Data;
using System.Data.Entity;
using MyCommon.MyEnum;
using ProjectDomain;
using SalesCenterDomain.BDL.Project;
using System.Linq;


namespace SalesCenterDomain.BLL
{
    public class MyProjectBidding
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private Sales_JobMaster_BiddingJob _biddingJob;
        private readonly int _salesID;
        public int NewJobID { get; set; }

      public MyProjectBidding(int salesID, int bidingID)
        {
            _salesID = salesID;
            _biddingJob  = _db.Sales_JobMaster_BiddingJob.Find(bidingID);
        }

      public void CreateProject()
      {
          var insertBidding = new ProjectInsertBidding(_salesID);
          
          insertBidding.JobTitle = _biddingJob.TempProjectName;
          insertBidding.TargetDate = Convert.ToDateTime(_biddingJob.TempTargetDate);
          insertBidding.CustomerID = _biddingJob.BillToID;
          insertBidding.Insert();

          //For the purpose of databind, to highlight it
          NewJobID = insertBidding.JobID;

          UpdateProjectID();

          //Inser a related customer form
          var cp = new ProjectCompany(NewJobID);
          cp.Insert(NewJobID, _biddingJob.BillToID, true, true, true);

          var est = new SpecDomain.BLL.EstTitle.MyEstRevCreate();
          est.Create(NewJobID);

      }

        private  void UpdateProjectID()
        {
            _biddingJob.JobID = NewJobID;
            _db.Entry(_biddingJob).State = EntityState.Modified;
            _db.SaveChanges();
        }

   }

    public class MyProjectBiddingCreate
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private readonly Sales_JobMasterList _project;
        private readonly int _salesID;
        public int NewJobID { get; set; }

        public MyProjectBiddingCreate(int salesID, int projectID)
        {
            _salesID = salesID;
            _project = _db.Sales_JobMasterList.Find(projectID);
        }


       public void Create()
       {
           try
           {
               var t1 = MyCommon.MyConvert.ConvertToDate(_project.startTimeStamp);
               var bid = new Sales_JobMaster_BiddingJob()
               {
                   //public int BiddingID { get; set; }
                   JobID = _project.jobID,
                   BillToID = _project.Customer,
                   InstallToID = 0,
                   QuoteToID = 0,
                   SubmitID = 0,
                   IsValid = true,
                   TempCompanyName = "NA, Customer Has been Assigned",
                   BiddingSourceID = 0,
                   BiddingTypeID = 0,
                   ProductRequired = "",
                   CreatedBy = _project.sales,
                   AeAssignedDate = t1,
                   BidDeadline = t1.AddDays(14),

                   TempProjectName = _project.jobTitle,
                   TempTargetDate = _project.targetDate,
                   TempAE = _project.sales,
                   TempAmount = 0,
               };

               _db.Sales_JobMaster_BiddingJob.Add(bid);
               _db.SaveChanges();
           }
           catch (Exception e)
           {
               var s = e.Message;
               throw;
           }
           finally
           {
               
           }

  
        }


    }



    public class MyProjectBiddingUpdate
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private readonly Sales_JobMaster_BiddingJob _bid;
        private readonly Sales_JobMasterList_QuoteRev _quote;

        public bool IsBiddingJob { get; set; }
        public MyProjectBiddingUpdate(int projectID)
        {
           var project = _db.Sales_JobMasterList.Find(projectID);
            _quote = project.Sales_JobMasterList_QuoteRev.LastOrDefault();
            IsBiddingJob = project.isBidToProject;
            _bid = project.Sales_JobMaster_BiddingJob.FirstOrDefault();
            if(_bid==null )
            {
                IsBiddingJob = false;
            }
       }

        public void UpdateWinner (string winnerName, double winningAmount)
        {
            if (_bid == null) return;

            try
            {
                _bid.Winner = winnerName;
                _bid.WinnerAmount = winningAmount;
                _db.Entry(_bid).State = EntityState.Modified;

                _db.SaveChanges();
            }
            catch (Exception e)
            {
                var s = e.Message;
                throw;
            }
            finally
            {

            }


        }


        public void UpdateLastQuotation()
        {
            if (_quote ==null ) return;

            try
            {
                _quote.quoteStatus = (int) NJobStatus.Loss;
                _quote.isLost = true;
                _quote.contractDate = DateTime.Today;
                _db.Entry(_quote).State = EntityState.Modified;

                _db.SaveChanges();
            }
            catch (Exception e)
            {
                var s = e.Message;
                throw;
            }
            finally
            {

            }


        }

    }
}