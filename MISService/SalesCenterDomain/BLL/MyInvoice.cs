using System.Collections.Generic;
using System.Linq;
using ProjectDomain;

namespace SalesCenterDomain.BLL
{
    public class MyInvoice
    {
        //public int InvoiceID {get; private set;}

        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();

        public MyInvoice()
        {
        }

        public MyInvoice(int invoiceID)
        {
            Value = _db.Sales_JobMasterList_Invoice.Find(invoiceID);
        }

        #region ***************************  Collection ***************************************

        public List<Sales_JobMasterList_Invoice> GetInvoicesBySalesID(int salesID)
        {
            return
                _db.Sales_JobMasterList_Invoice.Where(x => salesID == x.Sales || salesID == x.SA1 || salesID == 5020).
                    OrderBy(x => x.invoiceNo).ToList();
        }

        #endregion

        public Sales_JobMasterList_Invoice Value { get; private set; }
    }
}