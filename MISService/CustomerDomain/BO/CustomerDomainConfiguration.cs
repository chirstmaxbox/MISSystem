using System;

namespace CustomerDomain.BO
{
    public class CustomerDomainConfiguration
    {

        public static readonly string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SQLLHDBConnectionString"].ConnectionString;

    }
}