using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using ProjectDomain;
using SpecDomain.BLL.EstItem;

namespace SalesCenterDomain.BLL
{
    public class MyQuoteItem
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private readonly int _quoteItemID;
        private readonly int _quoteRevID;

        public MyQuoteItem(int quoteRevID, int quoteItemID)
        {
            _quoteRevID = quoteRevID;
            _quoteItemID = quoteItemID ;
        }

        public List<Quote_Item> QuoteItems { get; set; }

        public void UpdateSpecialFieldFromEstimation()
        {
            List<Quote_Item> items = _db.Quote_Item.Where(x => x.quoteRevID == _quoteRevID &&
                                                                 ( _quoteItemID ==0 ||
                                                                   x.quoteItemID==_quoteItemID) 
                                                               ).ToList();
            if (items.Count == 0) return;

            foreach (Quote_Item item in items)
            {
                int estItemID = item.estItemID;
                if (estItemID == 0) continue;
                var estItem = new MyEstItem(estItemID);

                item.SerialID = estItem.Value.SerialID.ToString("");
                item.Position = estItem.Value.Position;
                item.qiDescription = estItem.GetDescriptionWithHtmlBreak();

                _db.Entry(item).State = EntityState.Modified;
            }

            _db.SaveChanges();
        }

        public void GenerateMySelfSerialID()
        {
            if (_quoteItemID == 0) return;

            Sales_JobMasterList_QuoteRev quote = _db.Sales_JobMasterList_QuoteRev.Find(_quoteRevID);
            int jobID = quote.jobID;
            Sales_JobMasterList project = _db.Sales_JobMasterList.Find(jobID);
            int lastItemID = project.LastQuoteItemID;
            project.LastQuoteItemID = lastItemID + 1;
            _db.Entry(project).State = EntityState.Modified;

            Quote_Item item = _db.Quote_Item.Find(_quoteItemID);
            item.SerialID = "Q" + (lastItemID + 1).ToString("D2");
            _db.Entry(item).State = EntityState.Modified;

            _db.SaveChanges();
        }

    }
}