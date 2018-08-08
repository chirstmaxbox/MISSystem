using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using enterprise = MISService.SfdcReference;

namespace MISService.Method
{
    public class SalesForceMethods
    {
        public static string userName = "anh.tran@exocloud.ca";
        //public static string userName = "anh.tran@exocloud.ca.testing";
        public static string password = "6%namnhumoinamA";
        public static string token = "4OtoAt7RYUMt7NVhgdsCaKS8z";
        //public static string token = "8AUgkOkaesxHncSWplLKVrB4";

        public static string sessionId = string.Empty;
        public static string serverUrl = string.Empty;
        /// <summary>
        /// Call SFDC endpoint and retrieve authentication token and API URL for SOAP callers
        /// </summary>
        /// 
        public static bool AuthenticateSfdcEnterpriseUser()
        {
            //print message to console
            LogMethods.Log.Debug("Authenticating against the Enterprise API ...");
            Console.WriteLine("Authenticating against the Enterprise API ...");
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                //use default binding and address from app.config
                using (enterprise.SoapClient loginClient = new enterprise.SoapClient("Soap"))
                {
                    //set account password and account token variables
                    string sfdcPassword = password;
                    string sfdcToken = token;

                    //set to Force.com user account that has API access enabled
                    string sfdcUserName = userName;

                    //create login password value
                    string loginPassword = sfdcPassword + sfdcToken;

                    //call Login operation from Enterprise WSDL
                    enterprise.LoginResult result =
                        loginClient.login(
                        null, //LoginScopeHeader
                        sfdcUserName,
                        loginPassword);

                    //get response values
                    sessionId = result.sessionId;
                    serverUrl = result.serverUrl;

                    //print response values
                    Console.WriteLine(string.Format("The session ID is {0} and server URL is {1}", sessionId, serverUrl));
                    Console.WriteLine("");
                }
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("AuthenticateSfdcEnterpriseUser:Error = " + e.Message);
                return false;
            }

            return true;

        }


    }
}
