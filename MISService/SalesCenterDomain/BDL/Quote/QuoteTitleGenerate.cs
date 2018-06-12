using System;
using System.Data;
using System.Data.SqlClient;
using CustomerDomain.BDL;
using MyCommon.MyEnum;
using SalesCenterDomain.BDL.Project;
using SalesCenterDomain.BDL.Service;
using SalesCenterDomain.BLL;
using SalesCenterDomain.BO;
using SpecDomain.BLL.EstTitle;

namespace SalesCenterDomain.BDL.Quote
{
    public class QuoteTitleGenerate
    {
        private readonly int _cID;
        private readonly int _estRevID;
        private readonly int _jobID;
        private int _myID;

        public QuoteTitleGenerate(int jobID, int estRevID)
        {
            _jobID = jobID;
            _estRevID = estRevID;
            var cp = new ProjectCompany(jobID);
            _cID = cp.QuoteToCompanyID;
        }

        public int MyID
        {
            get { return _myID; }
            set { _myID = value; }
        }

        public int ValidationErrorID { get; private set; }


        public void Generate()
        {
            var v = new GenerateQuoteValidation(_jobID, _estRevID);
            ValidationErrorID = v.ValidationErrorID;
            if (ValidationErrorID != 0) return;

            GenerateTitle();
            _myID = GetNewID();

            GenerateNewItems();
            GenerateNewService();

            var mqi = new MyQuoteItem(_myID, 0);

            //SerialID
            //QiDescription
            mqi.UpdateSpecialFieldFromEstimation();
        }

        public void GenerateTitle()
        {
            //generate Quote Title

            var connectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            //Get Row Schema
            const string sqlSelectString = "SELECT * FROM Sales_JobMasterList_quoteRev WHERE (quoteRevID =0)";
            var selectCommand = new SqlCommand(sqlSelectString, connectionSQL);
            var adapter1 = new SqlDataAdapter(selectCommand);

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                connectionSQL.Open();
                adapter1.Fill(ds1, "t1");

                DataRow row = ds1.Tables["t1"].NewRow();

                var pcv = new ProjectChildrenVersion(_jobID);
                row["quoteRev"] = pcv.NewestQuoteRev;
                row["quoteOption"] = 0;
                row["quoteAmount"] = 0;

                row["quoteStatus"] = NJobStatus.qProcessing;
                row["estRevID"] = _estRevID;

                row["isssueDate"] = DateTime.Today;

                var cst = new CustomerDetails(_cID);
                row["termBalance"] = cst.TermID;
                row["Currency"] = cst.Currency;
                row["termDeposit"] = cst.TermDeposit;

                row["jobID"] = _jobID;
                row["quoteGST"] = true;
                row["quotePST"] = false;
                row["quoteLocked"] = false;
                row["isItemCopied"] = false;
                row["PrintOption"] = 1;
                row["DiscountText"] = "Discount";
                row["DiscountAmount"] = 0;

                var est = new MyEstRev(_estRevID);
                row["sa1ID"] = est.Value.sa1ID;

                //Add this into dataset
                ds1.Tables["t1"].Rows.Add(row);

                var cb = new SqlCommandBuilder(adapter1);
                adapter1 = cb.DataAdapter;
                adapter1.Update(ds1, "t1");
            }
            catch (SqlException ex)
            {
                string result = ex.Message;
            }
            finally
            {
                connectionSQL.Close();
            }
        }

        public int GetNewID()
        {
            const string tblName = "Sales_JobMasterList_QuoteRev";
            return SqlCommon.GetNewlyInsertedRecordID(tblName);
        }


        public void GenerateNewItems()
        {
            var qItem = new QuoteItemsGenerateFromEstimation(_myID, _estRevID);
            qItem.Generate();
        }

        public int GenerateNewItems(int estItemID)
        {
            var qItem = new QuoteItemsGenerateFromEstimation(_myID, _estRevID);
            return qItem.Generate(estItemID);
        }

        public void GenerateNewService()
        {
            var svc = new ServiceGenerate(_myID, "Quote", _estRevID, "EST");
            svc.insert();
        }


    }
}