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
            
            
            LogMethods.Log.Info("-------------- *** Starting MISService *** ------------ ");
            if (SalesForceMethods.AuthenticateSfdcEnterpriseUser())
            {
                ProjectMethods pm = new ProjectMethods();
                pm.GetAllProjects();
            }
            LogMethods.Log.Debug("-------------- *** Endings MISService *** ------------ ");
            

 /*
            // Making a window service
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new MyMISService() 
            };
            ServiceBase.Run(ServicesToRun);
*/
        }
    }
}
