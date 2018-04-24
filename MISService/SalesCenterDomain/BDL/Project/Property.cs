using System;
using System.Data;
using EmployeeDomain;
using MyCommon;

namespace SalesCenterDomain.BDL.Project
{
    public class ProjectDetails
    {
        #region "Properties"

        public string JobTitle
        {
            get
            {
                string jobTitle = "DoesNotExist";
                DataRow row = _pf.GetProjectDataRow();
                if (row != null)
                {
                    if (!Convert.IsDBNull(row["jobTitle"]))
                    {
                        jobTitle = Convert.ToString(row["jobTitle"]);
                    }
                }
                return jobTitle;
            }
        }

        public string JobNumber
        {
            get
            {
                string jobNumber = "11P00000";
                DataRow row = _pf.GetProjectDataRow();
                if (row != null)
                {
                    if (!Convert.IsDBNull(row["jobNumber"]))
                    {
                        jobNumber = Convert.ToString(row["jobNumber"]);
                    }
                }
                return jobNumber;
            }
        }

        public string ContractNumber
        {
            get { return JobNumber.Replace("P", "T"); }
        }

        public string ReasonOfNoContract
        {
            get
            {
                string s = "";
                DataRow row = _pf.GetProjectDataRow();
                if (!Convert.IsDBNull(row["ReasonOfNoContract"]))
                {
                    s = Convert.ToString(row["ReasonOfNoContract"]);
                }
                return s;
            }

            set { _pf.UpdateReasonOfNoContract(value); }
        }

        //public int EstimationRevID
        //{
        //    get
        //    {
        //        var i = 0;
        //        var row = _pf.GetProjectDataRow();
        //        if (row!=null)
        //        {
        //            i=Common.MyConvert.ConvertToInteger ( row["EstimationRevID"]);
        //        }
        //        return i;
        //    }
        //    set { _pf.UpdateEstimationFinalRev(value); }
        //}
        //public int QuoteFinalRevID
        //{
        //    get
        //    {
        //        int id = 0;
        //        var row = _pf.GetProjectDataRow();
        //        if (!Convert.IsDBNull(row["quoteRevID"]))
        //        {
        //            id = Convert.ToInt32( row["quoteRevID"]);
        //        }
        //        return id;
        //    }

        //    set { _pf.UpdateQupteFinalRev(value); }

        //}


        public int CustomerID
        {
            set { _pf.UpdateCustomerID(value); }
        }

        public int Sales
        {
            get
            {
                DataRow row = _pf.GetProjectDataRow();
                var sales = (int) EmployeeEN.NEmployeeIDDefault.NullSales110;
                if (row != null)
                {
                    if (!Convert.IsDBNull(row["sales"]))
                    {
                        sales = Convert.ToInt32(row["sales"]);
                    }
                }

                return sales;
            }
            set { Sales = value; }
        }

        public int Sa1ID
        {
            get
            {
                DataRow row = _pf.GetProjectDataRow();
                var sa1ID = (int) EmployeeEN.NEmployeeIDDefault.NullSales110;
                if (row != null)
                {
                    if (!Convert.IsDBNull(row["sa1ID"]))
                    {
                        sa1ID = Convert.ToInt32(row["sa1ID"]);
                    }
                }

                return sa1ID;
            }
            set { Sa1ID = value; }
        }


        public bool IsSubProject
        {
            get
            {
                bool b = false;
                DataRow row = _pf.GetProjectDataRow();
                if (row != null)
                {
                    int subProjectID = Convert.ToInt32(row["subProject"]);
                    if (subProjectID > 0)
                    {
                        b = true;
                    }
                }


                return b;
            }
        }

        public bool IsBidToProject
        {
            get
            {
                DataRow row = _pf.GetProjectDataRow();
                bool isBidToProject = false;
                if (row != null)
                {
                    isBidToProject = Convert.ToBoolean(row["isBidToProject"]);
                }
                return isBidToProject;
            }
            set { _pf.UpdateIsBidToProject(value); }
        }


        public bool Rush
        {
            set { _pf.UpdateRush(value); }
        }

        public DateTime TargetDate
        {
            get
            {
                DateTime targetDate = DateTime.Today;
                DataRow row = _pf.GetProjectDataRow();
                if (row != null)
                {
                    if (!Convert.IsDBNull(row["TargetDate"]))
                    {
                        targetDate = Convert.ToDateTime(row["TargetDate"]);
                    }
                }
                return targetDate;
            }
            set
            {
                DateTime dt = DateTime.Today;
                if (MyConvert.IsDate(value))
                {
                    dt = value;
                }
                _pf.UpdateTargetDate(dt);
            }
        }


        public string ReasonOfNoWorkorder
        {
            set { _pf.UpdateReasonOfNoWorkorder(value); }
        }


        public int LeadID
        {
            get
            {
                DataRow row = _pf.GetProjectDataRow();

                if (row == null) return 0;

                return Convert.IsDBNull(row["LeadID"]) ? 0 : Convert.ToInt32(row["LeadID"]);
            }
        }

        #endregion

        private readonly ProjectField _pf;

        public ProjectDetails(int jobID)
        {
            _pf = new ProjectField(jobID);
        }
    }
}