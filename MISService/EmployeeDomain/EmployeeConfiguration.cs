using System;
using System.Configuration;
using System.Reflection;

namespace EmployeeDomain
{
    public class EmployeeConfiguration
    {
        //  **** Shared  ****
        public static readonly string ConnectionString =ConfigurationManager.ConnectionStrings["SQLLHDBConnectionString"].ConnectionString;
        public static readonly string ServerPrefix =ConfigurationManager.AppSettings["ServerPrefix"];
        
        public static readonly string ConnectionStringForward = "Data Source=FORWARD1;Initial Catalog=FORWARD;Persist Security Info=True;User ID=sysadm;Password=sysadm";

    
        // ********* Employee Website  ************
        
        public static readonly string ConfigurationInstallerHourlyRate =ConfigurationManager.AppSettings["InstallerLabourhourRate"];
        
        public static readonly string EmailAddressSalesFollowupFrom1 =Convert.ToString(ConfigurationManager.AppSettings["EmailAddressSalesFollowupFrom1"]);

        public static readonly string EmailAddressMisDefault =Convert.ToString(ConfigurationManager.AppSettings["EmailAddressMisDefault"]);
    }
}