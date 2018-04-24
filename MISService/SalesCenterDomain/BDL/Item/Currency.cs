using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Item
{
    public abstract class CurrencyChange
    {
        public double Rate;

        public CurrencyChange(string OriginalCurrency, double currencyRate)
        {
            Rate = currencyRate;
            if (SalesCenterConstants.MAIN_CURRENCY == OriginalCurrency)
            {
                Rate = 1/Rate;
            }
        }

        //for each row in tbl.rows amount=amount*rate
        public abstract void ChangeRate(string recordType);
    }


    public class CurrencyChangeProduct : CurrencyChange
    {
        private readonly int _myID;

        public CurrencyChangeProduct(string originalCurrency, double currencyRate, int myID)
            : base(originalCurrency, currencyRate)
        {
            _myID = myID;
        }


        public override void ChangeRate(string rType)
        {
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM [QUOTE_Item] WHERE ([quoteRevID] = @quoteRevID and [recordType]=@recordType)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = _myID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.Char).Value = rType;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();

                int NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
                {
                    foreach (DataRow row in ds1.Tables["t1"].Rows)
                    {
                        row["qiAmount"] = getNewValue(row["qiAmount"]);
                    }

                    //4. Write ds2,  back to DB
                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    NumRowsAffected = adapter1.Update(ds1, "t1");
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
        }

        private double getNewValue(object amount)
        {
            double d = 0;
            if (!Convert.IsDBNull(amount))
            {
                double amt = Convert.ToDouble(amount);
                d = amt*Rate;
            }
            return d;
        }
    }


    public class CurrencyChangeService : CurrencyChange
    {
        private readonly int _myID;

        public CurrencyChangeService(string originalCurrency, double currencyRate, int myID)
            : base(originalCurrency, currencyRate)
        {
            _myID = myID;
        }


        public override void ChangeRate(string rType)
        {
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM [FW_QUOTE_SERVICE] WHERE ([estRevID] = @estRevID and [recordType]=@recordType)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = _myID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.NChar, 7).Value = rType;
            var ds1 = new DataSet();

            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int numRowsAffected = adapter1.Fill(ds1, "t1");
                if (numRowsAffected != 0)
                {
                    //Destination

                    foreach (DataRow row in ds1.Tables["t1"].Rows)
                    {
                        row["qsAmount"] = GetNewValue(row["qsAmount"]);
                    }

                    //4. Write ds2,  back to DB

                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    numRowsAffected = adapter1.Update(ds1, "t1");
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
        }

        private string GetNewValue(object str)
        {
            string s = "";

            if (!MyConvert.IsNullString(str))
            {
                string s1 = Convert.ToString(str);
                //remove '
                while (s1.Contains(","))
                {
                    int int1 = s1.IndexOf(",");
                    s1 = s1.Remove(int1, 1);
                }

                //handle "$"
                try
                {
                    if (s1.Contains("$"))
                    {
                        int int1 = s1.IndexOf("$");
                        s1 = s1.Remove(int1, 1);
                    }
                    double d = MyConvert.ConvertToDouble(s1);
                    if (d != 0)
                    {
                        s1 = "$" + (d*Rate).ToString();
                    }

                    s = s1;
                }
                catch (SqlException ex)
                {
                }
            }


            return s;
        }
    }
}