using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmployeeDomain.Models;
using MyCommon;

namespace EmployeeDomain.BLL
{
    public class MyEmployeeCollection
    {
        private readonly EmployeeDbModelEntities _db=new EmployeeDbModelEntities(); 
        public List<FW_Employees>  GetEmployees(int departmentID)
        {
            var es = _db.FW_Employees.Where(x => x.dID == departmentID).OrderBy(x => x.NickName).ToList();
            return es;
        }

        public List<FW_Department1 >GetDepartments()
        {
            var es = _db.FW_Department1.Where(x=>x.InputHourSelect>0) .OrderBy(x => x.DEPARTMENT_NAME ).ToList();
            return es;
        }

        public List<FW_Employees> GetEmployeeProduction(int departmentID)
        {
            var dids = new int[]{2};
            if (departmentID != (int) EmployeeEN.NDepartmentID.Metal)
            {
                dids = new int[] {3, 4, 6};
            }
            
                var es = _db.FW_Employees.Where(x => dids.Contains(x.dID  ) & 
                                                 x.NickName!=null &
                                                 x.Active &
                                                  x.IsShowOnSubdepartmentSchedule &
                                                  x.FwLabourTicketStationEmployees.Any() |
                                                  x.EmployeeNumber ==(int)EmployeeEN.NEmployeeIDDefault.ChooseProduction 
                                                 ).OrderBy(x => x.NickName)
                                                  .ToList();
            return es;
        }

        public List<FW_Employees> GetProductionEmployeesForPPPSchedule(int procedureGroupID)
        {
            var dids = new int[] { 2,3,4,6,7,21 };
            
            var es = _db.FW_Employees.Where(x => dids.Contains(x.dID) &
                                               ( x.ShowOnInputHour | 
                                                 x.NickName ==null ))
                                              .OrderBy(x => x.NickName )
                                              .ToList();
           
 

            if (procedureGroupID > 0)
            {
                es = es.Where(x => x.ProcedureGroupID == procedureGroupID).ToList();
            }

            return es;
        }

        public List<FW_Employees> GetProductionEmployeesForPPPTimecard(int procedureGroupID)
        {
            var dids = new int[] { 2, 3, 4, 6, 7, 21 };

            var es = _db.FW_Employees.Where(x => dids.Contains(x.dID)  &&
                                                 x.NickName != null &&
                                                 !x.IsSubcontract) 
                                                  .OrderBy(x => x.EmployeeNumber)
                                                  .ToList();

            if (procedureGroupID > 0)
            {
                es = es.Where(x => x.ProcedureGroupID == procedureGroupID).ToList();
            }

            return es;
        }


        public List<FW_Employees> GetEmployees(List<int> employeeIDs)
        {
            var es = _db.FW_Employees.Where(x =>employeeIDs.Contains(x.EmployeeNumber) )
                                              .OrderBy(x => x.EmployeeNumber)
                                              .ToList();
            return es;
        }

    }

   public class MyInstaller
    {
       public string[] Values { get; private set; }

        private readonly EmployeeDbModelEntities _db=new EmployeeDbModelEntities(); 
        public MyInstaller()
        {
            Values= _db.FW_Employees.Where(x => x.dID == (int)EmployeeEN.NDepartmentID.Installation && x.Active && x.IsMtoAwared).OrderBy(x=>x.NickName).Select(x => x.NickName ).ToArray();
        }
   
    }
     
}
