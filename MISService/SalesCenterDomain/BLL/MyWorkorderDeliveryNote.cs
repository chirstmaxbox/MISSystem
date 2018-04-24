using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using ProjectDomain;

namespace SalesCenterDomain.BLL
{
    public class MyWorkorderDeliveryNote
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private readonly int _woID;

        public MyWorkorderDeliveryNote(int woID)
        {
            _woID = woID;
        }

        public void Delete()
        {
            //Delivery Note
            WO_Shipping ws = _db.WO_Shipping.FirstOrDefault(x => x.WoID == _woID);
            if (ws == null) return;

            _db.Entry(ws).State = EntityState.Deleted;

            _db.SaveChanges();


            //Shipping Items
            List<WO_ShippingItem> items = _db.WO_ShippingItem.Where(x => x.WoID == _woID).ToList();
            if (items.Count == 0) return;

            foreach (WO_ShippingItem item in items)
            {
                _db.Entry(item).State = EntityState.Deleted;
            }

            _db.SaveChanges();
        }


        public void CopyWorkorderItems()
        {
            List<WO_Item> woItems = _db.WO_Item.Where(x => x.woID == _woID).ToList();
            if (woItems.Count == 0) return;

            List<WO_ShippingItem> existingShippingItems = _db.WO_ShippingItem.Where(x => x.WoID == _woID).ToList();
            int itemNumber = existingShippingItems.Count + 1;

            foreach (WO_Item woItem in woItems)
            {
                var deliveryItem = new WO_ShippingItem
                                       {
                                           WoID = _woID,
                                           ItemNumber = itemNumber,
                                           Qty = GetQty(woItem),
                                           Description = woItem.estItemNameText + ", " + woItem.woDescription
                                       };

                _db.WO_ShippingItem.Add(deliveryItem);
                itemNumber += 1;
            }

            _db.SaveChanges();
        }

        private string GetQty(WO_Item woItem)
        {
            string s = "";
            if (woItem.qty != 0)
            {
                s += woItem.qty + " " + woItem.qtyUnit;
            }

            if (woItem.qtyPC != 0)
            {
                s += woItem.qtyPC + " " + woItem.qtyPcUnit;
            }
            return s;
        }
    }
}