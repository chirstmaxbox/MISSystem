using System;
using System.Data;

namespace SalesCenterDomain.BDL.Invoice
{
    public class VersionControl

    {
        private readonly DataTable _invoiceDatatable;

        public VersionControl(int invoiceID)
        {
            var ip = new InvoiceProperty(invoiceID);
            string invoiceNumber = ip.InvoiceNumber;
            var ic = new InvoiceCollection();
            _invoiceDatatable = ic.GetNewInvoiceRevision(invoiceNumber);
        }

        public VersionControl(string invoiceNumber)
        {
            var ic = new InvoiceCollection();
            _invoiceDatatable = ic.GetNewInvoiceRevision(invoiceNumber);
        }


        public int NewestInvoiceRevision
        {
            get { return GetNewInvoiceRevision(); }
        }


        private int GetNewInvoiceRevision()
        {
            int qr = 0;
            if (_invoiceDatatable == null) return qr + 1;

            foreach (DataRow row in _invoiceDatatable.Rows)
            {
                int temp = Convert.ToInt32(row["Revision"]);
                if (temp > qr)
                {
                    qr = temp;
                }
            }

            return qr + 1;
        }
    }
}