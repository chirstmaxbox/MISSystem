using System;
using System.Collections.Generic;
using System.Linq;
using MyCommon;
using MyCommon.MyEnum;
using ProjectDomain;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BLL
{
    /// <summary>
    /// Projejct
    /// SubProject
    /// </summary>
    public class MyWorkorderValidationContractAmount
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private readonly string[] _exceptCustomerNames = {"SMART CENTRE"};
        private readonly bool _isSubProject;
        private readonly Sales_JobMasterList _project;
        private readonly Sales_JobMasterList_WO _value;
        private readonly int _woID;

        public MyWorkorderValidationContractAmount(int woID)
        {
            _woID = woID;
            _value = _db.Sales_JobMasterList_WO.Find(_woID);
            _project = _db.Sales_JobMasterList.Find(_value.jobID);
            _isSubProject = _project.subProject != 0;
        }

        public int Result
        {
            get { return GetResult(); }
        }


        public double IssuedWorkorderAmount { get; private set; }
        public double CurrentWorkorderItemAmount { get; private set; }
        public double ContractAmount { get; private set; }


        public string JobTitle
        {
            get { return GetJobTitle(); }
        }

        public string WorkorderTitle
        {
            get { return GetWorkorderTitle(); }
        }


        private int GetResult()
        {
            //Approved
            if (_project.AbnormalWorkorderIssueStatus == 10)
                return (int) NWorkorderValidationError.Validated;
            //No Charge
            var payMethods = new[]
                                 {
                                     (Int16) NPaymentMethods.COD,
                                     (Int16) NPaymentMethods.DueUponRecipt,
                                     (Int16) NPaymentMethods.Installersgiveinvoicetoclient,
                                     (Int16) NPaymentMethods.MailOutbyOffice
                                 };
            short payMethod = Convert.ToInt16(_value.PayMethods);
            if (!payMethods.Contains(payMethod)) return (int) NWorkorderValidationError.Validated;

            //isInExceptionalList
            if (GetIsInExceptionalList()) return (int) NWorkorderValidationError.Validated;

            //IsThereContract()
            if (!GetIsThereContract()) return (int) NWorkorderValidationError.NoContract;

            //Amount exceed Contract
            if (IsWorkorderAmountExceedContractAmount())
            {
                return (int) NWorkorderValidationError.ExceedContractAmount;
            }
            if (IsWorkorderAmountExceedContractAmountSubProject())
            {
                return (int) NWorkorderValidationError.ExceedContractAmount;
            }

            return (int) NWorkorderValidationError.Validated;
        }


        private bool GetIsInExceptionalList()
        {
            //var jobCustomers = _db.Sales_JobMasterList_Customer.Where(x => x.jobID == _project.jobID).ToList();
            //if (!jobCustomers.Any()) return false;
            //var b = false;
            //int length = _exceptCustomerNames.Length;
            //foreach (var jobCustomer in jobCustomers)
            //{

            //    var customer = _db.CUSTOMERs.Find(jobCustomer.cID);

            //    for (var i = 0; i <= length - 1; i++)
            //    {
            //        var exceptCustomerIDWord = _exceptCustomerNames[i];
            //        if (customer.NAME.ToUpper().Contains(exceptCustomerIDWord) |
            //            customer.LEGAL_NAME.ToUpper().Contains(exceptCustomerIDWord))
            //        {
            //            b = true;
            //            break;
            //        }
            //    }
            //}

            return false;
        }

        //Master Project Has to be WIN
        private bool GetIsThereContract()
        {
            //Is there are Contract
            List<Sales_JobMasterList_QuoteRev> contracts =
                _db.Sales_JobMasterList_QuoteRev.Where(
                    x => x.jobID == _project.jobID && x.contractAmount > 0 && x.quoteStatus == 420).ToList();
            if (contracts.Any()) return true;

            if (_project.subProject == 0) return false;
            //Check if the main project has contract
            Sales_JobMasterList mainProject =
                _db.Sales_JobMasterList.First(x => x.jobNumber == _project.jobTitle && x.subProject == 0);
            if (mainProject == null) return false;
            List<Sales_JobMasterList_QuoteRev> mainContracts =
                _db.Sales_JobMasterList_QuoteRev.Where(
                    x => x.jobID == mainProject.jobID && x.contractAmount > 0 & x.quoteStatus == 420).ToList();
            return mainContracts.Any();
        }


        private bool IsWorkorderAmountExceedContractAmount()
        {
            double contractAmount = 0;
            List<Sales_JobMasterList_QuoteRev> contracts =
                _db.Sales_JobMasterList_QuoteRev.Where(
                    x => x.jobID == _project.jobID && x.contractAmount > 0 && x.quoteStatus == 420).ToList();
            if (contracts.Any())
            {
                //Get Contract Amount
                foreach (Sales_JobMasterList_QuoteRev contract in contracts)
                {
                    contractAmount +=
                        Convert.ToDouble(
                            _db.Quote_Item.Where(x => x.quoteRevID == contract.quoteRevID && x.isFinal).Sum(
                                x => x.qiAmount));
                }
            }

            if (_isSubProject && Math.Abs(contractAmount - 0) < 0.001) return false; //NA

            //Get Approved Workorder Amount
            List<Sales_JobMasterList_WO> issued =
                _db.Sales_JobMasterList_WO.Where(
                    x =>
                    x.Sales_JobMasterList.jobNumber == _project.jobNumber && x.woStatus == 781 && x.woType == 10 &&
                    !x.revise && !x.reDo).ToList();

            //Get Current Workorder Amount
            double issuedWorkorderAmount = issued.Sum(x => x.WO_Item.Sum(y => y.qiAmount));
            double currentWorkorderItemAmount = _db.WO_Item.Where(x => x.woID == _woID).Sum(y => y.qiAmount);
            return issuedWorkorderAmount + currentWorkorderItemAmount > contractAmount;
        }

        private bool IsWorkorderAmountExceedContractAmountSubProject()
        {
            if (!_isSubProject) return false; //NA
            double contractAmount = 0;


            //Check if the main project has contract
            Sales_JobMasterList mainProject =
                _db.Sales_JobMasterList.First(x => x.jobNumber == _project.jobTitle && x.subProject == 0);
            if (mainProject != null)
            {
                List<Sales_JobMasterList_QuoteRev> mainContracts = _db.Sales_JobMasterList_QuoteRev.Where(
                    x => x.jobID == mainProject.jobID && x.contractAmount > 0 & x.quoteStatus == 420).ToList();
                if (mainContracts.Any())
                {
                    foreach (Sales_JobMasterList_QuoteRev mainContract in mainContracts)
                    {
                        MyConvert.ConvertToDouble(mainContract.contractAmount);
                    }
                }
            }
            return false;
        }


        ////Get Approved Workorder Amount
        //var issued = _db.Sales_JobMasterList_WO.Where(x =>x.Sales_JobMasterList.jobNumber ==_project .jobNumber && x.woStatus == 781 && x.woType == 10 && !x.revise && !x.reDo).ToList();

        ////Get Current Workorder Amount
        //IssuedWorkorderAmount = issued.Sum(x => x.WO_Item.Sum(y => y.qiAmount));
        //CurrentWorkorderItemAmount=_db.WO_Item .Where (x=>x.woID ==_woID ).Sum (y => y.qiAmount);


        private string GetJobTitle()
        {
            return _project.jobNumber + " - " + _project.jobTitle;
        }

        private string GetWorkorderTitle()
        {
            return _value.WorkorderNumber + " - " + _value.jobTitle;
        }
    }
}