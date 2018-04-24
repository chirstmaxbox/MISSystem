using MyCommon.MyEnum;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Invoice
{
    public class InvoiceCreditNote : InvoiceTitleCopy
    {
        private readonly int _myID;

        public InvoiceCreditNote(int myParentID, int newParentID, int myID)
            : base(myParentID, newParentID, myID)
        {
            _myID = myID;
        }

        public void GenerateCreditNote()
        {
            Copy();

            //retrive  existing Invoice Number and Type

            var invs = new InvoiceProperty(_myID);
            string invoiceNumber = invs.InvoiceNumber;
            if (NewID > SalesCenterConstants.BEGIN_INVOICE_ID)
            {
                var ip = new InvoiceProperty(NewID);
                ip.InvoiceNumber = GetCreditNoteNumber(invoiceNumber);

                var invType = new InvoiceType(NewID);
                invType.ChangeTo((int) NInvoiceType.CreditNote);

                var invU = new InvoiceTitleUpdate(NewID);
                invU.UpdateRev(1);

                var note = "This Credit Note is Against Invoice No. " + invoiceNumber ;
                invU.UpdateNote(note);

                //New revision Number 
            }
        }

        private static string GetCreditNoteNumber(string invNumber)
        {
            //Replace "V" with "C"
            string s = invNumber;
            if (s.Contains("V"))
            {
                s = s.Replace("V", "C");
            }
            else if (s.Contains("v"))
            {
                s = s.Replace("v", "C");
            }

            return s;
        }
    }
}