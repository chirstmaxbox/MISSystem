using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon.MyEnum;
using SalesCenterDomain.BDL.NumberControl;
using SalesCenterDomain.BDL.Project;
using SalesCenterDomain.BDL.Quote;
using SalesCenterDomain.BDL.Service;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Invoice
{
    public class ValidationGenerateInvoice
    {
        private readonly int _jobID;

        public ValidationGenerateInvoice(int jobID)
        {
            _jobID = jobID;
        }

        public int ValidationErrorID
        {
            get { return GetValidationErrorID(); }
        }

        private int GetValidationErrorID()
        {
            //Quote To
            var cp = new ProjectCompany(_jobID);

            if (!cp.isThereABilltoCompany) return (int) NValidationErrorValue.BillTo;
            return 0;
        }
    }

    public abstract class InvoiceTitleGenerate
    {
        private readonly int _jobID;
        private readonly int _sourceTitleID;
        private int _myID;


        public InvoiceTitleGenerate(int myParentID, int sourceID)
        {
            _jobID = myParentID;
            _sourceTitleID = sourceID;
        }

        public int ValidationErrorID { get; private set; }

        public int MyID
        {
            get { return _myID; }
        }

        public void Generate()
        {
            var v = new ValidationGenerateInvoice(_jobID);
            ValidationErrorID = v.ValidationErrorID;
            if (ValidationErrorID != 0) return;

            GenerateTitle();
            _myID = GetNewID();
            RegisterThisRecordID();

            GenerateNewItems(_myID);
            GenerateNewService(_myID);
            GenerateNewNotes(_myID);
            DoInsertMiscellousJobs(_myID);
        }


        private void GenerateTitle()
        {
            try
            {
                DataRow rowQt = GetSourceTitleDataRow();
                NewInvoiceTitle(rowQt);
            }
            catch (SqlException ex)
            {
                string errorstring = ex.Message;
            }
        }

        private int GetNewID()
        {
            string tblName = "Sales_JobMasterList_Invoice";
            int nID = SqlCommon.GetNewlyInsertedRecordID(tblName);
            return nID;
        }

        private void RegisterThisRecordID()
        {
            var inController = new InvoiceNumberController(_myID, (int) NInvoiceType.Regular);
            inController.Register();
        }


        public virtual void GenerateNewItems(int myID)
        {
        }

        public virtual void GenerateNewNotes(int myID)
        {
        }

        public virtual void GenerateNewService(int myID)
        {
        }

        public virtual void DoInsertMiscellousJobs(int myID)
        {
        }


        public abstract DataRow GetSourceTitleDataRow();

        private void NewInvoiceTitle(DataRow rowQt)
        {
            int NumRowsAffected = 0;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString2 = "SELECT * FROM [Sales_JobMasterList_Invoice] WHERE ([invoiceID] = 0)";
            var SelectCommand2 = new SqlCommand(SqlSelectString2, ConnectionSQL);
            var adapter2 = new SqlDataAdapter(SelectCommand2);
            var ds2 = new DataSet();
            ds2.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter2.Fill(ds2, "t2");

                DataRow rowNew = ds2.Tables["t2"].NewRow();

                // Sales_JobMasterList.estimationRevID,
                rowNew["jobID"] = _jobID;

                rowNew["InvoiceNo"] = "9I0000";
                //ToBeUpdate

                rowNew["Revision"] = 1;
                rowNew["invoiceType"] = NInvoiceType.Regular;
                rowNew["invoiceDate"] = DateTime.Today;
                rowNew["CustomerID"] = rowQt["cID"];
                rowNew["ContactID"] = rowQt["ContactName"];
                rowNew["sales"] = rowQt["Sales"];
                rowNew["Term"] = 0;
                if (!Convert.IsDBNull(rowQt["termBalance"]))
                {
                    rowNew["Term"] = rowQt["termBalance"];
                }

                rowNew["Workorders"] = "";

                string location = "";
                if (!Convert.IsDBNull(rowQt["jobLocation"]))
                {
                    location = Convert.ToString(rowQt["jobLocation"]);
                }

                if (!Convert.IsDBNull(rowQt["Town"]))
                {
                    if (string.IsNullOrEmpty(location))
                    {
                        location = Convert.ToString(rowQt["Town"]);
                    }
                    else
                    {
                        location += ", " + rowQt["Town"];
                    }
                }

                if (!Convert.IsDBNull(rowQt["State"]))
                {
                    if (string.IsNullOrEmpty(location))
                    {
                        location = Convert.ToString(rowQt["State"]);
                    }
                    else
                    {
                        location += ", " + rowQt["State"];
                    }
                }


                if (!Convert.IsDBNull(rowQt["PostCode"]))
                {
                    if (string.IsNullOrEmpty(location))
                    {
                        location = Convert.ToString(rowQt["PostCode"]);
                    }
                    else
                    {
                        location += ", " + rowQt["PostCode"];
                    }
                }

                rowNew["jobLocation"] = location;

                var p = new ProjectDetails(_jobID);

                rowNew["ContractNo"] = p.ContractNumber;
                //rowQt("contractNumber")
                if (rowQt["contractDate"] != null)
                {
                    rowNew["ContractDate"] = rowQt["contractDate"];
                }

                rowNew["ShipVia"] = "Installation";
                rowNew["Approved1"] = false;
                rowNew["Approved2"] = false;
                rowNew["Datetimestamp"] = DateTime.Now;
                rowNew["gst"] = rowQt["quoteGST"];
                rowNew["pst"] = rowQt["quotePST"];
                rowNew["Note"] = "";
                rowNew["Currency"] = rowQt["Currency"];
                if (Convert.IsDBNull(rowQt["contractAmount"]))
                {
                    rowNew["InvoiceAmount"] = 0;
                }

                rowNew["CurrencyRate"] = 1;

                rowNew["iStatus"] = (int) NJobStatus.invNew;
                //                rowNew("fromWhere") = _from

                rowNew["Final"] = true;
                rowNew["Title"] = "";
                if (!Convert.IsDBNull(rowQt["jobTitle"]))
                {
                    rowNew["Title"] = rowQt["jobTitle"];
                }

                ds2.Tables["t2"].Rows.Add(rowNew);

                //4. Write ds2,  back to DB

                var cb = new SqlCommandBuilder(adapter2);
                adapter2 = cb.DataAdapter;
                NumRowsAffected = adapter2.Update(ds2, "t2");
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

    public class InvoiceTitleGenerateFromProject : InvoiceTitleGenerate
    {
        private readonly int _jobID;

        public InvoiceTitleGenerateFromProject(int jobID)
            : base(jobID, jobID)
        {
            _jobID = jobID;
        }

        public override DataRow GetSourceTitleDataRow()
        {
            DataRow row = null;
            int rowsAffected = 0;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            //
            string SqlSelectString = "SELECT * FROM View_InvoiceTitleFromEstimation WHERE (jobID = @jobID)";

            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter2 = new SqlDataAdapter(SelectCommand);
            var ds2 = new DataSet();
            ds2.Tables.Clear();

            adapter2.SelectCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;

            try
            {
                ConnectionSQL.Open();
                rowsAffected = adapter2.Fill(ds2, "t2");
                if (rowsAffected > 0)
                {
                    row = ds2.Tables["t2"].Rows[0];
                }
            }
            catch (SqlException ex)
            {
                string errorlog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }

            return row;
        }
    }

    public class InvoiceTitleGenerateFromEstimation : InvoiceTitleGenerateFromProject
    {
        private readonly int _estRevID;

        public InvoiceTitleGenerateFromEstimation(int jobID, int estRevID)
            : base(jobID)
        {
            _estRevID = estRevID;
        }

        public override void GenerateNewItems(int myID)
        {
            //
            var iItem = new InvoiceItemsGenerateFromEstimation(myID, _estRevID);
            iItem.Generate();
        }

        public override void GenerateNewService(int myID)
        {
            var svc = new ServiceGenerate(myID, "Invoice", _estRevID, "Est");
            svc.insert();
        }
    }

    public class InvoiceTitleGenerateFromQuote : InvoiceTitleGenerate
    {
        //Generate Invoice from Quote

        private readonly int _quoteRevID;

        public InvoiceTitleGenerateFromQuote(int jobID, int quoteRevID)
            : base(jobID, quoteRevID)
        {
            _quoteRevID = quoteRevID;
        }

        public override DataRow GetSourceTitleDataRow()
        {
            return QuoteDataRow.GetQuoteTitleDataRow(_quoteRevID);
        }


        public override void GenerateNewItems(int myID)
        {
            //
            var iItem = new InvoiceItemsGenerateFromQuote(myID, _quoteRevID);
            iItem.Generate();
        }


        public override void GenerateNewService(int myID)
        {
            var svc = new ServiceGenerate(myID, "Invoice", _quoteRevID, "Quote");
            svc.insert();
        }
    }
}