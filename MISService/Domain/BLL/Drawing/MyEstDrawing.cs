
using System.Data.Entity;
using SpecDomain.Model;

namespace SpecDomain.BLL.Drawing
{
    public class MyEstDrawing
    {
        //public long EstItemDrawingID { get; set; }
        public int EstRevID { get; set; }
        public int ProjectID { get; set; }

        public string DrawingType { get; set; }
        public string DrawingName { get; set; }
        
        public void Create(EST_Drawing value)
        {
           var db = new SpecificationDbEntities();
            db.EST_Drawing.Add(value);
            db.SaveChanges();
        }

        public void Delete(long drawingID)
        {
            var db = new SpecificationDbEntities();
            var drw = db.EST_Drawing.Find(drawingID);
            db.Entry(drw).State = EntityState.Deleted;
            db.SaveChanges();
        }

    }

}