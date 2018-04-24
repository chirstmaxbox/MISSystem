using System;

namespace MyCommon
{
    /// <summary>
    /// Summary description for ReportParameter
    /// </summary>
     public class MyReportInqueryParameter
        {
            public int reportID = 0;

            public DateTime bd = DateTime.Today;
            public DateTime ed = DateTime.Today;
            public int kam = 5020;
            public int team = 0;
            public int teamType = 2;
            public Boolean cbx1Checked = false;
            public Boolean cbx1Visible = false;
            public string cbx1Text = "";
            public string AeNameString = "";

            public Boolean cbx2Checked = false;
            public Boolean cbx2Visible = false;
            public string cbx2Text = "";
            //public Boolean cbx3Checked = false;
            //public Boolean cbx4Checked = false;

            public int ddlSelectedValue = 0;
            public string[][] ddlItems;
            public int PrintingEmployeeID = 110;
            //    public string GeneralPurpose = "";

        }

}