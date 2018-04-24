using System;
using System.Configuration;

namespace SpecDomain.BO
{
    public class SpecConfiguration
    {
        public static readonly string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SQLLHDBConnectionString"].ConnectionString;
        public static readonly string ServerPrefix = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ServerPrefix"]);

        public static readonly int ESTIMATION_EID =Convert.ToInt32(ConfigurationManager.AppSettings["DefaultEstimateEmployeeID"]);

        public static readonly int ESTIMATION_EID01 = 93;

        public static readonly int STRUCTURAL_DRAWING_EID =Convert.ToInt32(ConfigurationManager.AppSettings["DefaultStructuralDrawingEmployeeID"]);

        public static readonly int GRAPHIC_DRAWING_EID =Convert.ToInt32(ConfigurationManager.AppSettings["DefaultGraphicDrawingEmployeeID"]);

        public static readonly int WORKORDER_APPROVE_Production_EID =Convert.ToInt32(ConfigurationManager.AppSettings["DefaultWorkorderApproverEmployeeIDProduction"]);

        public static readonly int WORKORDER_APPROVE_Service_EID =Convert.ToInt32(ConfigurationManager.AppSettings["DefaultWorkorderApproverEmployeeIDService"]);

        public static readonly int WORKORDER_APPROVE_Sitecheck_EID =Convert.ToInt32(ConfigurationManager.AppSettings["DefaultWorkorderApproverEmployeeIDSitecheck"]);

    
        //Response leadtime Hours
        public static readonly int ResponseTimeEstimation = Convert.ToInt32(ConfigurationManager.AppSettings["ResponseTime-Estimation"]);
        public static readonly int ResponseTimeArtDrawing = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResponseTime-ArtDrawing"]);
        public static readonly int ResponseTimeStructuralDrawing = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResponseTime-StructuralDrawing"]);
        public static readonly int ResponseTimeWorkorderApprove = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ResponseTime-WorkorderApprove"]);
        //? resposne Report Employee List
        private static readonly char[] Separator = new char[] { ',' };
        public static readonly string[] RESPONSE_REPORT_EMPLOYEE_LIST_ARTROOM = System.Configuration.ConfigurationManager.AppSettings["ResponseReportEmployeeListArtroom"].Split(Separator);
        public static readonly string[] RESPONSE_REPORT_EMPLOYEE_LIST_QD = System.Configuration.ConfigurationManager.AppSettings["ResponseReportEmployeeListQD"].Split(Separator);
 

   }
}