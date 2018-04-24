using System.Data;
using System.Data.Entity;
using ProjectDomain;

namespace SalesCenterDomain.BLL
{
    public class MyWorkorderItem
    {
        //public List<WO_Item> WorkorderItems { get; set; }

        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private readonly int _itemID;
        private readonly int _woID;

        public MyWorkorderItem(int woID, int itemID)
        {
            _woID = woID;
            _itemID = itemID;
        }

        public void GenerateMySelfSerialID()
        {
            Sales_JobMasterList_WO wo = _db.Sales_JobMasterList_WO.Find(_woID);
            int jobID = wo.jobID;
            Sales_JobMasterList project = _db.Sales_JobMasterList.Find(jobID);
            int lastItemID = project.LastWorkorderItemID;
            project.LastWorkorderItemID = lastItemID + 1;
            _db.Entry(project).State = EntityState.Modified;

            WO_Item item = _db.WO_Item.Find(_itemID);
            item.SerialID = "W" + (lastItemID + 1).ToString("D2");
            _db.Entry(item).State = EntityState.Modified;

            _db.SaveChanges();
        }

        public void UpdateIsHide(bool isHide)
        {
            WO_Item item = _db.WO_Item.Find(_itemID);

            item.IsHide = isHide;
            _db.Entry(item).State = EntityState.Modified;

            _db.SaveChanges();
        }
    }
}