using CustomerDomain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISService.Models
{
    public class CrmEmployee
    {
        public int UserEmployeeID { get; private set; }
        public string NickName
        {
            get { return GetEmployeeNickname(); }
        }

        public bool IsAeSelectAllowedInLeadIndex
        {
            get { return _employee.CrmLeadSelectAeEnable; }
        }

        public int Role
        {
            get { return _employee.Role; }
        }

        private readonly CustomerDbEntities _db = new CustomerDbEntities();
        private readonly CustomerDomain.Model.FW_Employees _employee;


        public CrmEmployee(string userIdentityName)
        {
            _employee = GetEmployee(userIdentityName);
            UserEmployeeID = GetUserEmployeeID();
        }

        private FW_Employees GetEmployee(string userIdentityName)
        {
            char[] splitter = { '\\' };
            string[] serverPrefix = userIdentityName.Split(splitter);
            var userName = serverPrefix[1].ToUpper();
            return _db.FW_Employees.FirstOrDefault(e => e.UserName.ToUpper() == userName);
        }

        private int GetUserEmployeeID()
        {
            var eID = 0;
            if (_employee != null)
            {
                eID = _employee.EmployeeNumber;
            }
            return eID;
        }

        private string GetEmployeeNickname()
        {
            var name = string.Empty;
            if (_employee != null)
            {
                name = _employee.NickName;
            }
            return name;
        }
    }
}
