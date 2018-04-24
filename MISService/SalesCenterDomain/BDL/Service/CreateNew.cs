using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon;
using MyCommonWeb;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Service
{
    public class ServiceGenerate
    {
        private readonly int _myParentID;
        //Source Parent ID

        //   recordType As String = "Quote" / "Invoice" / "EST"   
        private readonly string _myType = "Quote";
        private readonly int _sourceTitleID;

        private readonly string _sourceType = "EST";

        public ServiceGenerate(int myParentID, string myType, int sourceID, string sourceType)
        {
            _myParentID = myParentID;
            _sourceTitleID = sourceID;
            _myType = myType;
            _sourceType = sourceType;
        }


        public void insert()
        {
            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM [FW_QUOTE_SERVICE] WHERE ([estRevID] = @estRevID and [recordType]=@recordType)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = _sourceTitleID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.NChar, 7).Value = _sourceType;
            var ds1 = new DataSet();

            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
                {
                    //Destination
                    DataTable t2 = ds1.Tables["t1"].Copy();
                    t2.TableName = "t2";
                    ds1.Tables.Add(t2);
                    ds1.Tables["t2"].Clear();

                    //3. Copy
                    int itemIndex = ds1.Tables["t1"].Columns.Count;
                    DataRow row = null;
                    DataRow rowNew = null;
                    int i = 0;
                    foreach (DataRow row_loopVariable in ds1.Tables["t1"].Rows)
                    {
                        row = row_loopVariable;
                        rowNew = ds1.Tables["t2"].NewRow();
                        for (i = 1; i <= itemIndex - 1; i++)
                        {
                            rowNew[i] = row[i];
                        }

                        rowNew["recordType"] = _myType;
                        rowNew["estRevID"] = _myParentID;

                        int serviceID = Convert.ToInt32(row["qsServiceID"]);
                        if (serviceID != (int) NServiceID.FreeType)
                        {
                            var sd = new ServiceDefinition(serviceID);
                            rowNew["qsTitle"] = sd.ServiceTitle;
                        }
                        else
                        {
                            rowNew["qsTitle"] = VbHtml.MyHtmlDecode(rowNew["qsTitle"]);
                        }

                        rowNew["qsDescription"] = VbHtml.MyHtmlDecode(rowNew["qsDescription"]);

                        rowNew["qsAmountText"] = rowNew["qsAmount"];

                        ds1.Tables["t2"].Rows.Add(rowNew);
                    }

                    //4. Write ds2,  back to DB

                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    NumRowsAffected = adapter1.Update(ds1, "t2");
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
    }
}