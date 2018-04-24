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
        public static string userName = "anh_tranquoc@outlook.com";
        public static string password = "30ngay1thang";
        public static string token = "bV9wC0sjf49BVVhf5qkQihSl";

        private static string sessionId = string.Empty;
        private static string serverUrl = string.Empty;
        /// <summary>
        /// Call SFDC endpoint and retrieve authentication token and API URL for SOAP callers
        /// </summary>
        /// 
        public static bool AuthenticateSfdcEnterpriseUser()
        {
            //print message to console
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
                Console.WriteLine("Authentication error = ", e.Message);
                return false;
            }

            return true;

        }

        /// <summary>
        /// Use Enteprise API to query and retrieve SFDC records
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="serverUrl"></param>
        public static void QueryEnterpriseRecord()
        {
            Console.WriteLine("Querying account records with the Enterprise API ...");

            try
            {
                //set query endpoint to value returned by login request
                EndpointAddress apiAddr = new EndpointAddress(serverUrl);

                //instantiate session header object and set session id
                enterprise.SessionHeader header = new enterprise.SessionHeader();
                header.sessionId = sessionId;

                //create service client to call API endpoint
                using (enterprise.SoapClient queryClient = new enterprise.SoapClient("Soap", apiAddr))
                {
                    //create SQL query statement
                    string query = "SELECT Name, AccountNumber, BillingState FROM Account WHERE BillingState = 'CA'";

                    enterprise.QueryResult result;
                    queryClient.query(
                        header, //sessionheader
                        null, //queryoptions
                        null, //mruheader
                        null, //packageversion
                        query, out result);

                    //cast query results
                    IEnumerable<enterprise.Account> accountList = result.records.Cast<enterprise.Account>();

                    //show results
                    foreach (var account in accountList)
                    {
                        Console.WriteLine(string.Format("Account Name: {0}", account.Name));
                    }

                    Console.WriteLine("");
                    Console.WriteLine("Query complete.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("QueryEnterpriseRecord Error : " + e.Message);
            }

        }

    }
}
