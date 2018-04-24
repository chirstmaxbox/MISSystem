using System.Data.Entity;
using SpecDomain.Model;


namespace SpecDomain.BLL.EstItem
{
    public class MyEstItemUpdate
    {
        public bool IsValidated { get; set; }
        private readonly EST_Item _estItem;

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public MyEstItemUpdate(long estItemID)
        {
            _estItem = _db.EST_Item.Find(estItemID);
        }

        public void UpdateDescription(string textValue)
        {
            _estItem.Description = textValue;
            _db.Entry(_estItem).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void UpdateRemark(string textValue)
        {
            _estItem.Remark = textValue;
            _db.Entry(_estItem).State = EntityState.Modified;
            _db.SaveChanges();
        }

    }
}