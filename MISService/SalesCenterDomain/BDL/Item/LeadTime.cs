using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon;
using MyCommon.MyEnum;
using SalesCenterDomain.BDL.Workorder;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Item
{

    #region "LeadTime"

    public abstract class LeadtimeTableBase
    {
        public int woItemID;

        public LeadtimeTableBase(int workorderItemID)
        {
            woItemID = workorderItemID;
        }

        public void Update()
        {
            int leadtime = GetLeadtime();
            if (leadtime != 0)
            {
                UpdateLeadtime(leadtime);
            }
            else
            {
                UpdateReasonOfAlterLeadtime(leadtime);
            }
        }

        public abstract int GetLeadtime();

        private void UpdateLeadtime(int leadtime)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE [WO_Item] SET [leadtime] = @leadtime WHERE [woItemID] = @woItemID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);
                UPdateCommand.Parameters.Add("@woItemID", SqlDbType.Int).Value = woItemID;
                UPdateCommand.Parameters.Add("@leadtime", SqlDbType.Int).Value = leadtime;

                try
                {
                    Connection.Open();
                    UPdateCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorlog = ex.Message;
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        private void UpdateReasonOfAlterLeadtime(int leadtime)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [WO_Item] SET [leadtime] = @leadtime, [ReasonOfAlterLeadtime] = @ReasonOfAlterLeadtime WHERE [woItemID] = @woItemID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);
                UPdateCommand.Parameters.Add("@woItemID", SqlDbType.Int).Value = woItemID;
                UPdateCommand.Parameters.Add("@leadtime", SqlDbType.Int).Value = leadtime;
                UPdateCommand.Parameters.Add("@ReasonOfAlterLeadtime", SqlDbType.NVarChar, 200).Value =
                    "Out of Range of Leadtime Table";

                try
                {
                    Connection.Open();
                    UPdateCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorlog = ex.Message;
                }
                finally
                {
                    Connection.Close();
                }
            }
        }
    }

    public class LeadtimeTableProduction : LeadtimeTableBase
    {
        //       Private _woItemID As Integer = 0
        private readonly double _Price;
        private readonly int _estItemNameDetailsID;

        private readonly int _requirement = (int) NWorkorderRequirement.Installation;
        //When Edit Work order
        public LeadtimeTableProduction(int woItemID)
            : base(woItemID)
        {
            //     _woItemID = woItemID
            DataRow row = GetWoItemDataRow();
            if (row != null)
            {
                _Price = Convert.ToDouble(row["qiAmount"]);
                _estItemNameDetailsID = Convert.ToInt32(row["NameDetailsID"]);
                _requirement = Convert.ToInt32(row["Requirement"]);
            }
        }


        //When Generate Workorder
        public LeadtimeTableProduction(double Price, int estItemNameDetailsID, int requirement)
            : base(0)
        {
            _Price = Price;
            _estItemNameDetailsID = estItemNameDetailsID;
            _requirement = requirement;
        }

        public override int GetLeadtime()
        {
            int ltDays = 0;

            //       if (_estItemNameDetailsID != 0)
            //      {
            DataRow rowL = GetLeadtimeTableDatarow();
            if (rowL != null)
            {
                int leadtimeType = GetLeadtimeType();
                int baseDays = 0;
                if (leadtimeType == (int) NLeadtimeType.SupplyAndInstall)
                {
                    baseDays = Convert.ToInt32(rowL["SupplyAndInstall"]);
                }
                else
                {
                    baseDays = Convert.ToInt32(rowL["SupplyOnly"]);
                }
                double basePrice = Convert.ToDouble(rowL["BasePrice"]);
                ltDays = GetCalCulateLeadTime(baseDays, basePrice);
            }
            //    }

            return ltDays;
        }

        private DataRow GetWoItemDataRow()
        {
            DataRow row = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [WO_Item] WHERE ([woItemID] = @woItemID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@woItemID", SqlDbType.Int).Value = woItemID;
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
                {
                    row = ds1.Tables["t1"].Rows[0];
                }
            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }
            return row;
        }

        private DataRow GetLeadtimeTableDatarow()
        {
            DataRow row = null;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [Product] WHERE ([ProductID] = @DetailsID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@DetailsID", SqlDbType.Int).Value = _estItemNameDetailsID;
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
                {
                    row = ds1.Tables["t1"].Rows[0];
                }
            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }

            return row;
        }


        private int GetLeadtimeType()
        {
            var ltType = (int) NLeadtimeType.SupplyAndInstall;
            switch (_requirement)
            {
                case (int) NWorkorderRequirement.Installation:
                case (int) NWorkorderRequirement.Fabrication:
                    ltType = (int) NLeadtimeType.SupplyAndInstall;
                    break;
                default:
                    ltType = (int) NLeadtimeType.SupplyOnly;
                    break;
            }
            return ltType;
        }

        //Here is the fomula
        private int GetCalCulateLeadTime(int baseDays, double BasePrice)
        {
            double ltDays = 0;
            //calculate base on formula
            double markupPriceBase = 1;
            double markupLeadtimeBase = 1;

            double markupPriceRange1 = 2.4/1.3;
            double markupLeadtimeRange1 = 1.6;

            ltDays = baseDays*2;
            if (_Price <= BasePrice*markupPriceBase)
            {
                ltDays = baseDays*markupLeadtimeBase;
            }
            else if (_Price <= BasePrice*markupPriceRange1)
            {
                ltDays = baseDays*markupLeadtimeRange1;
            }


            //    'if there is 0.xx day then consider it as one day
            //Dim ltInteger = Math.Truncate(ltDays)
            //Dim ltFraction = ltDays - ltInteger

            //If ltFraction > 0 Then
            //    ltFraction = 1
            //End If

            //ltDays = ltInteger + ltFraction

            return Convert.ToInt32(ltDays);
        }
    }

    public class LeadtimeSpecialProcedures
    {
        private readonly int _spId;

        public LeadtimeSpecialProcedures(int spID)
        {
            _spId = spID;
        }

        public int Leadtime
        {
            get { return GetLeadtime(); }
        }

        private int GetLeadtime()
        {
            int lt = 0;
            DataRow row = GetSpecialProcedures();
            if (row != null)
            {
                lt = Convert.ToInt32(row["DaysNeeded"]);
            }

            return lt;
        }

        private DataRow GetSpecialProcedures()
        {
            DataRow row = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [LeadtimeSpecialProcedure] WHERE ([lspID] = @lspID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@lspID", SqlDbType.Int).Value = _spId;
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
                {
                    row = ds1.Tables["t1"].Rows[0];
                }
            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }

            return row;
        }
    }

    public class LeadtimeTableService : LeadtimeTableBase
    {
        private int _woItemID;

        public LeadtimeTableService(int woItemID)
            : base(woItemID)
        {
            _woItemID = woItemID;
        }

        public override int GetLeadtime()
        {
            int ltDays = 0;
            string s = SalesCenterConfiguration.LeadtimeService;
            ltDays = MyConvert.ConvertToInteger(s);
            return ltDays;
        }
    }

    public class LeadtimeTableSitecheck : LeadtimeTableBase
    {
        private int _woItemID;

        public LeadtimeTableSitecheck(int woItemID)
            : base(woItemID)
        {
            _woItemID = woItemID;
        }

        public override int GetLeadtime()
        {
            int ltDays = 0;
            string s = SalesCenterConfiguration.LeadtimeSiteCheck;
            ltDays = MyConvert.ConvertToInteger(s);
            return ltDays;
        }
    }

    public class WorkorderItemsLeadtime
    {
        private readonly int _longestLeadtime;
        private readonly int _woID;

        private int _woType;

        public WorkorderItemsLeadtime(int woID, int woType)
        {
            _woID = woID;
            _woType = woType;
            _longestLeadtime = GetLongestLeadtime();
        }

        public int LongestLeadtime
        {
            get { return _longestLeadtime; }
        }

        public void Update()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_WO] SET [LongestLeadtime] = @LongestLeadtime WHERE [woID] = @woID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
                UPdateCommand.Parameters.Add("@LongestLeadtime", SqlDbType.Int).Value = _longestLeadtime;
                try
                {
                    Connection.Open();
                    UPdateCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorlog = ex.Message;
                }
                finally
                {
                    Connection.Close();
                }
            }
        }


        private int GetLeadtime_Item(int woItemId)
        {
            int max = 0;
            DataRow row = GetWoItemDataRow(woItemId);
            if (row != null)
            {
                max = Convert.ToInt32(row["leadtime"]);
            }
            return max;
        }


        private DataRow GetWoItemDataRow(int woItemID)
        {
            DataRow row = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [WO_Item] WHERE ([woItemID] = @woItemID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@woItemID", SqlDbType.Int).Value = woItemID;
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
                {
                    row = ds1.Tables["t1"].Rows[0];
                }
            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }
            return row;
        }


        private int GetLeadtime_SpecialProcedure(int woItemId)
        {
            int max = 0;
            DataTable tbl = GetSpecialProcedures(woItemId);
            if (tbl != null)
            {
                foreach (DataRow row in tbl.Rows)
                {
                    int temp = Convert.ToInt32(row["leadtime"]);
                    if (temp > max)
                    {
                        max = temp;
                    }
                }
            }
            return max;
        }

        private DataTable GetSpecialProcedures(int woItemID)
        {
            DataTable tbl = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [WO_Item_SpecialProcedure] WHERE ([woItemID] = @woItemID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@woItemID", SqlDbType.Int).Value = woItemID;
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
                {
                    tbl = ds1.Tables["t1"];
                }
            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }
            return tbl;
        }


        private int GetLongestLeadtime()
        {
            int max = 0;
            switch (GetWorkorderLeadTimeType())
            {
                case (int) NWorkorderLeadtimeSpecialType.Normal:
                    max = GetNormalLeadtime();
                    break;
                case (int) NWorkorderLeadtimeSpecialType.Sample:
                    max = GetSpecailLeadtime();
                    break;
            }

            return max;
        }

        private int GetNormalLeadtime()
        {
            int max = 0;
            //loop each and every items
            DataTable itemTbl = WorkorderShared.getExistingWorkorderItems(_woID);
            if (itemTbl != null)
            {
                foreach (DataRow row in itemTbl.Rows)
                {
                    int woItemID = Convert.ToInt32(row["woItemID"]);
                    int temp = GetLeadtime_Item(woItemID) + GetLeadtime_SpecialProcedure(woItemID);

                    if (temp > max)
                    {
                        max = temp;
                    }
                }
            }

            return max;
        }

        public int GetWorkorderLeadTimeType()
        {
            var ltType = (int) NWorkorderLeadtimeSpecialType.Normal;
            if (IsSampleWorkorder())
            {
                ltType = (int) NWorkorderLeadtimeSpecialType.Sample;
            }
            return ltType;
        }

        private bool IsSampleWorkorder()
        {
            bool b = false;
            DataRow row = WorkorderShared.GetWorkorderInfo(_woID);
            if (row != null)
            {
                if (Convert.ToInt32(row["PayMethods"]) == (int) NPaymentMethods.NoChargeSample)
                {
                    b = true;
                }
            }
            return b;
        }

        public int GetSpecailLeadtime()
        {
            return MyConvert.ConvertToInteger(SalesCenterConfiguration.LeadtimeSpecialSample);
        }
    }

    #endregion
}