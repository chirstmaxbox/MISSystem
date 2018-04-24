using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using ProjectDomain;

namespace SalesCenterDomain.BLL
{
    public class MyWorkorder
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private readonly int _woID;

        public MyWorkorder(int woID)
        {
            _woID = woID;
            Value = _db.Sales_JobMasterList_WO.Find(_woID);
        }

        public Sales_JobMasterList_WO Value { get; set; }

        public string GetInvoices()
        {
            string r = "";
            List<Sales_JobMasterList_Invoice_Workorder> invoiceWorkorders =
                _db.Sales_JobMasterList_Invoice_Workorder.Where(x => x.woID == _woID).ToList();
            if (invoiceWorkorders.Count == 0) return r;
            foreach (Sales_JobMasterList_Invoice_Workorder iw in invoiceWorkorders)
            {
                var invoice = new MyInvoice(iw.InvoiceID);
                if (invoice.Value != null)
                {
                    r += invoice.Value.invoiceNo + "  ";
                }
            }
            return r;
        }

        public double GetWorkorderAmount()
        {
            return _db.WO_Item.Where(x => x.woID == _woID).Sum(y => y.qiAmount);
        }

        public void UpdateWorkorderAmount()
        {
            double currentWorkorderItemAmount = _db.WO_Item.Where(x => x.woID == _woID).Sum(y => y.qiAmount);
            Value.WorkorderAmount = currentWorkorderItemAmount;
            _db.Entry(Value).State = EntityState.Modified;
            _db.SaveChanges();
        }


    }
}