using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeDomain.Models;

namespace EmployeeDomain.BLL
{
    public class EmployeesPastProbation
    {
        private readonly EmployeeDbModelEntities _dc= new EmployeeDbModelEntities();


        #region  ************* Alart past probation ****************

        public List<string> List
        {
            get { return GetIsPastProbation(); }
        }

        private List<string> GetIsPastProbation()
        {
            var eList = new List<String>();
            IOrderedQueryable<FW_Employees> ees =
                from c in _dc.FW_Employees
                where c.Active == true
                orderby c.NickName
                select c;

            foreach (FW_Employees ee in ees)
            {
                if (!ee.IsInProbation) continue;
                var swd = new EmployeeStartWorkingDate(ee.EmployeeNumber);

                if (DateTime.Today >= swd.StartWorkingDate.AddMonths(ee.ProbationPeriod))
                {
                    string ni = ee.NickName + " --- " + ee.EmployeeNumber.ToString();
                    eList.Add(ni);
                }
            }

            return eList;
        }

        #endregion
    }

    public class Probation
    {
        private readonly FW_Employees _ee;

        public Probation(int employeeID)
        {

            var dc = new EmployeeDbModelEntities();
            _ee = dc.FW_Employees.SingleOrDefault(c => c.EmployeeNumber == employeeID);
        }

        public DateTime ProbationEndingDate
        {
            get { return GetEmployeeProbationEndingDate(); }
        }

        public bool IsInProbation
        {
            get { return GetIsInProbation(); }
        }


        private DateTime GetEmployeeProbationEndingDate()
        {
            var d1 = new DateTime(2099, 1, 1);

            if (_ee != null)
            {
                var swd = new EmployeeStartWorkingDate(_ee.EmployeeNumber);
                d1 = swd.StartWorkingDate.AddMonths(_ee.ProbationPeriod);
            }

            return d1;
        }

        //
        private Boolean GetIsInProbation()
        {
            bool b = false;
            if (_ee != null)
            {
                b = _ee.IsInProbation;
            }
            return b;
        }
    }

    public class EmployeeStartWorkingDate
    {
        public EmployeeStartWorkingDate(int employeeID)
        {
            StartWorkingDate = GetStartWorkingDate(employeeID);
        }

        public DateTime StartWorkingDate { get; set; }

        private static DateTime GetStartWorkingDate(int employeeID)
        {
            DateTime startWorkingDate = DateTime.Today;
            var dc1 = new EmployeeDbModelEntities();
            var ewhs =
                from c in dc1.FW_Employee_WorkingHistory
                where c.EmployeeNumber == employeeID
                group c.StartWorkingDate by c.EmployeeNumber
                into g
                select new
                           {
                               EmployeeID = g.Key,
                               StartWorkingDate = g.Min()
                           };

            foreach (var row in ewhs)
            {
                startWorkingDate = Convert.ToDateTime(row.StartWorkingDate);
            }

            return startWorkingDate;
        }
    }
}