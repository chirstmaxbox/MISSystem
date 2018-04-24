using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using MyCommon.MyEnum;
using ProjectDomain;

namespace SalesCenterDomain.BLL
{
    public class MyProject
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private readonly int _projectID;


        public MyProject(int projectID)
        {
            _projectID = projectID;
            Value = _db.Sales_JobMasterList.Find(projectID);
        }

        public Sales_JobMasterList Value { get; private set; }

        public int MyMainProjectID
        {
            get { return GetMyMainProjectID(); }
        }


        public void UpdateGivenBy(int givenByCompanyID)
        {
            Value.ProjectGiveBy = givenByCompanyID;
            _db.Entry(Value).State = EntityState.Modified;
            _db.SaveChanges();
        }


        public void UpdateAbnormalWorkorderIssueReason(string value)
        {
            Value.AbnormalWorkorderIssueReason = value;
            _db.Entry(Value).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void UpdateAbnormalWorkorderIssueStatus(int value)
        {
            Value.AbnormalWorkorderIssueStatus = value;
            _db.Entry(Value).State = EntityState.Modified;
            _db.SaveChanges();
        }

        private int GetMyMainProjectID()
        {
            string projectName = Value.jobNumber;
            Sales_JobMasterList mainProject =
                _db.Sales_JobMasterList.First(x => x.jobNumber == projectName && x.subProject == 0);
            return mainProject.jobID;
        }


        public void Win(DateTime targetDate)
        {
            Value.JobStatus = (int) NJobStatus.win;
            Value.reasonOfLoss = "";
            Value.targetDate = targetDate;

            _db.Entry(Value).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void Lost(string reasonOfLost)
        {
            Value.JobStatus = (int) NJobStatus.Loss;
            Value.reasonOfLoss = reasonOfLost;

            _db.Entry(Value).State = EntityState.Modified;

            List<Sales_JobMasterList_QuoteRev> quotes =
                _db.Sales_JobMasterList_QuoteRev.Where(x => x.jobID == _projectID).ToList();
            if (quotes.Any())
            {
                foreach (Sales_JobMasterList_QuoteRev quote in quotes)
                {
                    quote.quoteStatus = (int) NJobStatus.Loss;
                    quote.contractAmount = 0;
                    _db.Entry(quote).State = EntityState.Modified;
                }
            }

            _db.SaveChanges();
        }

        public void Finish()
        {
            Value.IsFinished = true;
            Value.FinishedDate = DateTime.Now;

            _db.Entry(Value).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void Activate()
        {
            Value.IsFinished = false;
            Value.FinishedDate =null;

            _db.Entry(Value).State = EntityState.Modified;
            _db.SaveChanges();
        }


        public void OrderConformationPrinted()
        {
            Value.OrderConfirmationPrinted = 1;
            _db.Entry(Value).State = EntityState.Modified;
            _db.SaveChanges();
        }


        public void UpdateCommission(Sales_JobMasterList commissionFactor)
        {
            Value.SalesType = commissionFactor.SalesType;
            Value.ProductLine = commissionFactor.ProductLine;

            Value.IsPaidPremiu = commissionFactor.IsPaidPremiu;
            Value.isBidToProject = commissionFactor.isBidToProject;
            Value.IsGivenByOldPartner = commissionFactor.IsGivenByOldPartner;
            Value.OverLimitRate = commissionFactor.OverLimitRate;
            Value.CommissionGivenBy = commissionFactor.CommissionGivenBy;

            _db.Entry(Value).State = EntityState.Modified;
            _db.SaveChanges();
        }
    }
}