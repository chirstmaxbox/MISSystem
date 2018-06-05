using CustomerDomain.Model;
using MISService.Method;
using MISService.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MISService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            /*
            CUSTOMER_CONTACT cc = new CUSTOMER_CONTACT();
            cc.CONTACT_ID = 20329;
            cc.CUSTOMER_ROWID = 18458;
            cc.CONTACT_FIRST_NAME = "firstname";
            cc.CONTACT_LAST_NAME = "lastname";
            cc.CONTACT_ACTIVE = true;

            CustomerMethods cmt = new CustomerMethods();
            cmt.EditCustomerContact(cc);
             * */

            if (SalesForceMethods.AuthenticateSfdcEnterpriseUser())
            {
                ProjectMethods pm = new ProjectMethods();
                pm.GetAllProjects();
            }

            /*
            // Making a window service
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new MyMISService() 
            };
            ServiceBase.Run(ServicesToRun);
             * */
        }
    }
}
