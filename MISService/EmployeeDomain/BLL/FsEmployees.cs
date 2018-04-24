using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmployeeDomain.Models;
using MyCommon;

namespace EmployeeDomain.BLL
{
    public class FsEmployees
    {
        private readonly EmployeeDbModelEntities _dc=new EmployeeDbModelEntities(); 

        public List<MyKeyValuePair> GetValues(int role)
        {
            var values = new List<MyKeyValuePair>();
            var ees =
                from c in _dc.FW_Employees
                where c.Active && c.Role == role || c.EmployeeNumber == 0 || c.EmployeeNumber == 110
                orderby c.NickName
                select c;

            foreach (FW_Employees ee in ees)
            {
                var kvp = new MyKeyValuePair()
                              {
                                  Key = ee.EmployeeNumber,
                                  Value = ee.NickName
                              };
                values.Add(kvp);
            }
            return values;

        }

        public List<MyKeyValuePair> GetValues(int[] roles)
        {
            var values = new List<MyKeyValuePair>();

            for (int i = 0; i < roles.Length; i++ )
            {
                var roleID = roles[i];
                var ees =
                    from c in _dc.FW_Employees
                    where c.Active && (c.Role == roleID || c.EmployeeNumber == 0 || c.EmployeeNumber == 110)
                    orderby c.NickName
                    select c;

                foreach (FW_Employees ee in ees)
                {
                    var kvp = new MyKeyValuePair()
                                  {
                                      Key = ee.EmployeeNumber,
                                      Value = ee.NickName
                                  };
                    values.Add(kvp);
                }
            }
            return values;

        }

        public List<MyKeyValuePair> GetSalesPerformanceReportEmployees()
        {
            var values = new List<MyKeyValuePair>();

            var ees =
                from c in _dc.FW_Employees
                where (c.IsShowOnPerformanceReport & c.dID == 63)
                orderby c.NickName
                select c;

            foreach (FW_Employees ee in ees)
            {
                var kvp = new MyKeyValuePair()
                {
                    Key = ee.EmployeeNumber,
                    Value = ee.NickName
                };
                values.Add(kvp);
            }

            return values;
        }
    
    }
}
