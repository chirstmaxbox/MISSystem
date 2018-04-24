using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using ProjectDomain;

namespace SalesCenterDomain.BLL
{
    public class MyInvoiceItem
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private readonly int _invoiceID;
        private readonly int _itemID;

        public MyInvoiceItem(int invoiceID, int itemID)
        {
            _invoiceID = invoiceID;
            _itemID = itemID;
        }

        public List<Invoice_Item> InvoiceItems { get; set; }

        public void GenerateMySelfSerialID()
        {
            Sales_JobMasterList_Invoice invoice = _db.Sales_JobMasterList_Invoice.Find(_invoiceID);
            int jobID = invoice.jobID;
            Sales_JobMasterList project = _db.Sales_JobMasterList.Find(jobID);
            int lastItemID = project.LastInvoiceItemID;
            project.LastInvoiceItemID = lastItemID + 1;
            _db.Entry(project).State = EntityState.Modified;

            Invoice_Item item = _db.Invoice_Item.Find(_itemID);
            item.SerialID = "I" + (lastItemID + 1).ToString("D2");
            _db.Entry(item).State = EntityState.Modified;

            _db.SaveChanges();
        }
    }
}