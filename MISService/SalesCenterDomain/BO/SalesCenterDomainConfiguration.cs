using System;
using System.Configuration;

namespace SalesCenterDomain.BO
{
    public class SalesCenterConfiguration
    {
        #region  **** Connection String  ****

        public static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["SQLLHDBConnectionString"].ConnectionString;

        #endregion

        #region ********* Application ************

        public static readonly string CopyDetailsToInvoice =
            Convert.ToString(ConfigurationManager.AppSettings["CopyDetailsToInvoice"]);

        public static readonly string CopyDetailsToWorkorder =
            Convert.ToString(ConfigurationManager.AppSettings["CopyDetailsToWorkorder"]);


        public static readonly string ConfigurationInstallerHourlyRate =
            Convert.ToString(ConfigurationManager.AppSettings["InstallerLabourhourRate"]);

        public static readonly string ServerPrefix = Convert.ToString(ConfigurationManager.AppSettings["ServerPrefix"]);

        public static readonly string LeadtimeSiteCheck =
            Convert.ToString(ConfigurationManager.AppSettings["Leadtime-SiteCheck"]);

        public static readonly string LeadtimeSpecialSample =
            Convert.ToString(ConfigurationManager.AppSettings["Leadtime-Special-Sample"]);

        public static readonly string LeadtimeService =
            Convert.ToString(ConfigurationManager.AppSettings["Leadtime-Service"]);

        #endregion

        #region ********* Application ************

        //Default Responsible EmployeeID
        public static readonly int ESTIMATION_EID =
            Convert.ToInt32(ConfigurationManager.AppSettings["DefaultEstimateEmployeeID"]);

        public static readonly int STRUCTURAL_DRAWING_EID =
            Convert.ToInt32(ConfigurationManager.AppSettings["DefaultStructuralDrawingEmployeeID"]);

        public static readonly int GRAPHIC_DRAWING_EID =
            Convert.ToInt32(ConfigurationManager.AppSettings["DefaultGraphicDrawingEmployeeID"]);

        public static readonly int WORKORDER_APPROVE_Production_EID =
            Convert.ToInt32(ConfigurationManager.AppSettings["DefaultWorkorderApproverEmployeeIDProduction"]);

        public static readonly int WORKORDER_APPROVE_Service_EID =
            Convert.ToInt32(ConfigurationManager.AppSettings["DefaultWorkorderApproverEmployeeIDService"]);

        public static readonly int WORKORDER_APPROVE_Sitecheck_EID =
            Convert.ToInt32(ConfigurationManager.AppSettings["DefaultWorkorderApproverEmployeeIDSitecheck"]);

        //Response leadtime Hours
        public static readonly int ResponseTimeEstimation =
            Convert.ToInt32(ConfigurationManager.AppSettings["ResponseTime-Estimation"]);

        public static readonly int ResponseTimeArtDrawing =
            Convert.ToInt32(ConfigurationManager.AppSettings["ResponseTime-ArtDrawing"]);

        public static readonly int ResponseTimeStructuralDrawing =
            Convert.ToInt32(ConfigurationManager.AppSettings["ResponseTime-StructuralDrawing"]);

        public static readonly int ResponseTimeWorkorderApprove =
            Convert.ToInt32(ConfigurationManager.AppSettings["ResponseTime-WorkorderApprove"]);

        //? resposne Report Employee List
        private static readonly char[] Separator = new[] {','};

        public static readonly string[] RESPONSE_REPORT_EMPLOYEE_LIST_ARTROOM =
            ConfigurationManager.AppSettings["ResponseReportEmployeeListArtroom"].Split(Separator);

        public static readonly string[] RESPONSE_REPORT_EMPLOYEE_LIST_QD =
            ConfigurationManager.AppSettings["ResponseReportEmployeeListQD"].Split(Separator);

        #endregion
    }
}