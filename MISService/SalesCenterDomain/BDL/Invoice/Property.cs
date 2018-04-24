using System;
using System.Data;
using EmployeeDomain;

namespace SalesCenterDomain.BDL.Invoice
{
    public class InvoiceProperty
    {
        private readonly InvoiceField _iField;

        public InvoiceProperty(int myID)
        {
            _iField = new InvoiceField(myID);
        }


        public string InvoiceNumber
        {
            get { return GetInvoiceNumber(); }
            set { _iField.UpdateNumber(value); }
        }


        public int JobID
        {
            get { return GetJobID(); }
        }


        public int CustomerID
        {
            get { return GetCustomerID(); }
            set { _iField.UpdateCustomerID(value); }
        }

        public int InvoiceType
        {
            get
            {
                int i = 0;
                DataRow row = _iField.GetInvoiceTitleDataRow();
                if (row != null)
                {
                    i = Convert.ToInt32(row["invoiceType"]);
                }
                return i;
            }
        }

        public int InvoiceStatus
        {
            get
            {
                int i = 0;
                DataRow row = _iField.GetInvoiceTitleDataRow();
                if (row != null)
                {
                    i = Convert.ToInt32(row["iStatus"]);
                }
                return i;
            }
        }

        public int ContactID
        {
            set { _iField.UpdateContactID(value); }
        }

        public int SalesID
        {
            get
            {
                var i = (int) EmployeeEN.NEmployeeIDDefault.NullSales110;
                DataRow row = _iField.GetInvoiceTitleDataRow();
                if (row != null)
                {
                    i = Convert.ToInt32(row["Sales"]);
                }
                return i;
            }
        }

        public int Sa1ID
        {
            get
            {
                var i = (int) EmployeeEN.NEmployeeIDDefault.NullSales110;
                DataRow row = _iField.GetInvoiceTitleDataRow();
                if (row != null)
                {
                    i = Convert.ToInt32(row["Sa1"]);
                }
                return i;
            }
        }

        private string GetInvoiceNumber()
        {
            string invNumber = "";

            DataRow row = _iField.GetInvoiceTitleDataRow();
            if (row != null)
            {
                invNumber = Convert.ToString(row["invoiceNo"]);
            }

            return invNumber;
        }

        private int GetJobID()
        {
            //by invoiceID
            int jobID = 0;
            DataRow row = _iField.GetInvoiceTitleDataRow();
            if (row != null)
            {
                jobID = Convert.ToInt32(row["jobID"]);
            }

            return jobID;
        }

        private int GetCustomerID()
        {
            int i = 0;
            DataRow row = _iField.GetInvoiceTitleDataRow();
            if (row != null)
            {
                i = Convert.ToInt32(row["CustomerID"]);
            }
            return i;
        }
    }
}