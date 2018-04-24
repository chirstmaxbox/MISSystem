using System;
using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Task
{

    #region Workorder Note for Invoice

    public class WorkorderNoteForInvoice
    {
        private readonly SqlConnection ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);

        private readonly int _id;
        private readonly int _woID;

        public WorkorderNoteForInvoice(int id, int woID)
        {
            //Constructor
            _id = id;
            _woID = woID;
        }

        public void InsertRecord(DateTime rDate, string remark, int noteType, Boolean invoiceExclude)
        {
            string scString =
                "INSERT INTO [Sales_JobMasterList_WO_Note] ([woID], [rDate], [remark], [NoteType],[InvoiceExclude]) VALUES (@woID, @rDate, @remark,@NoteType, @InvoiceExclude)";
            var sCommand = new SqlCommand(scString, ConnectionSQL);

            sCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
            sCommand.Parameters.Add("@rDate", SqlDbType.SmallDateTime).Value = rDate;
            sCommand.Parameters.Add("@remark", SqlDbType.NVarChar, 500).Value = remark;
            sCommand.Parameters.Add("@NoteType", SqlDbType.Int).Value = noteType;
            sCommand.Parameters.Add("@InvoiceExclude", SqlDbType.Bit).Value = invoiceExclude;

            try
            {
                ConnectionSQL.Open();
                sCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ConnectionSQL.Close();
            }
        }


        public void Update(DateTime rDate, string remark)
        {
            string scString =
                "UPDATE [Sales_JobMasterList_WO_Note] SET [rDate] = @rDate, [remark] = @remark WHERE [id] = @id";
            var sCommand = new SqlCommand(scString, ConnectionSQL);

            sCommand.Parameters.Add("@id", SqlDbType.Int).Value = _id;
            sCommand.Parameters.Add("@rDate", SqlDbType.SmallDateTime).Value = rDate;
            sCommand.Parameters.Add("@remark", SqlDbType.NVarChar, 500).Value = remark;

            try
            {
                ConnectionSQL.Open();
                sCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ConnectionSQL.Close();
            }
        }


        public void UpdateExclude(bool exclude)
        {
            string scString = "UPDATE [Sales_JobMasterList_WO_Note] SET [InvoiceExclude] = @exclude WHERE [id] = @id";
            var sCommand = new SqlCommand(scString, ConnectionSQL);

            sCommand.Parameters.Add("@id", SqlDbType.Int).Value = _id;
            sCommand.Parameters.Add("@Exclude", SqlDbType.Bit).Value = exclude;

            try
            {
                ConnectionSQL.Open();
                sCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ConnectionSQL.Close();
            }
        }
    }

    #endregion

    #region Projce Note for Invoice

    public class ProjectNoteForInvoiceCreate
    {
        //SELECT [id], [jobID], [rDate], [remark] FROM [tblSalesReportNoInvoiceReason]
        //DELETE FROM [tblSalesReportNoInvoiceReason] WHERE [id] = @id
        //UPDATE [tblSalesReportNoInvoiceReason] SET [jobID] = @jobID, [rDate] = @rDate, [remark] = @remark WHERE [id] = @id

        //INSERT INTO [tblSalesReportNoInvoiceReason] ([jobID], [rDate], [remark]) VALUES (@jobID, @rDate, @remark)

        private readonly SqlConnection ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
        private readonly int _id;

        public ProjectNoteForInvoiceCreate(int id)
        {
            //Constructor
            //Input 0 when create
            _id = id;
        }

        public void Insert(int jobID, DateTime rDate, string remark)
        {
            string scString =
                "INSERT INTO [tblSalesReportNoInvoiceReason] ([jobID], [rDate], [remark]) VALUES (@jobID, @rDate, @remark)";
            var sCommand = new SqlCommand(scString, ConnectionSQL);

            sCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = jobID;
            sCommand.Parameters.Add("@rDate", SqlDbType.SmallDateTime).Value = rDate;
            sCommand.Parameters.Add("@remark", SqlDbType.NVarChar, 500).Value = remark;

            try
            {
                ConnectionSQL.Open();
                sCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ConnectionSQL.Close();
            }
        }


        public void Update(DateTime rDate, string remark)
        {
            string scString =
                "UPDATE [tblSalesReportNoInvoiceReason] SET [rDate] = @rDate, [remark] = @remark WHERE [id] = @id";
            var sCommand = new SqlCommand(scString, ConnectionSQL);

            sCommand.Parameters.Add("@id", SqlDbType.Int).Value = _id;
            sCommand.Parameters.Add("@rDate", SqlDbType.SmallDateTime).Value = rDate;
            sCommand.Parameters.Add("@remark", SqlDbType.NVarChar, 500).Value = remark;

            try
            {
                ConnectionSQL.Open();
                sCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ConnectionSQL.Close();
            }
        }


        public void UpdateExclude(bool exclude)
        {
            string scString = "UPDATE [tblSalesReportNoInvoiceReason] SET [exclude] = @exclude WHERE [id] = @id";
            var sCommand = new SqlCommand(scString, ConnectionSQL);

            sCommand.Parameters.Add("@id", SqlDbType.Int).Value = _id;
            sCommand.Parameters.Add("@exclude", SqlDbType.Bit).Value = exclude;

            try
            {
                ConnectionSQL.Open();
                sCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ConnectionSQL.Close();
            }
        }
    }

    #endregion

    #region Quote Note

    public class NoteQuoteCreate
    {
        private readonly SqlConnection ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);

        private readonly int _jobID;

        public NoteQuoteCreate(int jobID)
        {
            //Constructor
            _jobID = jobID;
        }

        public void Insert(DateTime rDate, string remark, int employeeID)
        {
            InsertRecord(rDate, remark, employeeID);
        }

        private void InsertRecord(DateTime rDate, string remark, int employeeID)
        {
            string scString =
                "INSERT INTO [Sales_JobMasterList_QuoteFollow] ([jobID], [fDate], [Note],[EmployeeID]) VALUES (@jobID, @fDate, @Note, @EmployeeID)";
            var sCommand = new SqlCommand(scString, ConnectionSQL);

            sCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;
            sCommand.Parameters.Add("@fDate", SqlDbType.SmallDateTime).Value = rDate;
            sCommand.Parameters.Add("@Note", SqlDbType.NVarChar, 2988).Value = remark;
            sCommand.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = employeeID;

            try
            {
                ConnectionSQL.Open();
                sCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ConnectionSQL.Close();
            }
        }
    }

    public class NoteQuoteUpdate
    {
        private readonly SqlConnection ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);

        private readonly int _noteID;

        public NoteQuoteUpdate(int noteID)
        {
            //Constructor
            _noteID = noteID;
        }


        public bool IsSendToSales
        {
            set { UpdateIsSendToSales(value); }
        }

        public void Update(DateTime fDate, string remark)
        {
            string scString =
                "UPDATE [Sales_JobMasterList_QuoteFollow] SET [fDate] = @fDate, [Note] = @Note WHERE [id] = @id";
            var sCommand = new SqlCommand(scString, ConnectionSQL);

            sCommand.Parameters.Add("@id", SqlDbType.Int).Value = _noteID;
            sCommand.Parameters.Add("@fDate", SqlDbType.SmallDateTime).Value = fDate;
            sCommand.Parameters.Add("@Note", SqlDbType.NVarChar, 1000).Value = remark;

            try
            {
                ConnectionSQL.Open();
                sCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ConnectionSQL.Close();
            }
        }

        private void UpdateIsSendToSales(bool valule)
        {
            string scString =
                "UPDATE [Sales_JobMasterList_QuoteFollow] SET [IsEmailedToSales] = @IsEmailedToSales WHERE [id] = @id";
            var sCommand = new SqlCommand(scString, ConnectionSQL);

            sCommand.Parameters.Add("@id", SqlDbType.Int).Value = _noteID;
            sCommand.Parameters.Add("@IsEmailedToSales", SqlDbType.Bit).Value = valule;

            try
            {
                ConnectionSQL.Open();
                sCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ConnectionSQL.Close();
            }
        }
    }

    #endregion
}

//end Namespace