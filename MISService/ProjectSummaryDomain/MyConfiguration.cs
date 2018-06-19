namespace ProjectSummaryDomain
{
    public class MyConfiguration
    {
        public static readonly string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SQLLHDBConnectionString"].ConnectionString;

   }
}