
using System.Data.Entity;
using SpecDomain.Model;

namespace SpecDomain.BLL.Drawing
{
    public class MyEstItemDrawing
    {
        public int EstRevID { get; set; }
        public long EstItemID { get; set; }
        public string DrawingType { get; set; }
        public string DrawingName { get; set; }
        
        public void Create(EST_Item_Drawing value)
        {
           var db = new SpecificationDbEntities();
            db.EST_Item_Drawing.Add(value);
            db.SaveChanges();
        }

        public void Delete(long drawingID)
        {
            var db = new SpecificationDbEntities();
            var drw = db.EST_Item_Drawing.Find(drawingID);
            db.Entry(drw).State = EntityState.Deleted;
            db.SaveChanges();
        }

    }


}