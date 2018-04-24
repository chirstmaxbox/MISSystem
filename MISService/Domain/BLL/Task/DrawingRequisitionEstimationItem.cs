
using System.Data.Entity;
using SpecDomain.Model;

namespace SpecDomain.BLL.Task
{

    public class DrawingRequisitionEstimationItemModify
    {
        public Sales_Dispatching_DrawingRequisition_EstimationItem Value { get; set; }
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public int ProjectID { get; set; }
        public int EstRevID { get; set; }

        public DrawingRequisitionEstimationItemModify(int id)
        {
            Value = _db.Sales_Dispatching_DrawingRequisition_EstimationItem.Find(id);
            EstRevID = Value.EST_Item.EstRevID;
            ProjectID = Value.EST_Item.Sales_JobMasterList_EstRev.JobID;
        }

           public DrawingRequisitionEstimationItemModify()
           {}

        public void Update()
        {
            _db.Entry(Value).State = EntityState.Modified;
            _db.SaveChanges();
        }

        
        public void UpdateIsIncludedWhenSubmit(bool checkStatus)
        {
            Value.IsIncludedWhenPrint = checkStatus; 
            _db.Entry(Value).State = EntityState.Modified;
            _db.SaveChanges();
        }

        
        public void Delete()
        {
            _db.Entry(Value).State = EntityState.Deleted;
            _db.SaveChanges();
        }
}
}