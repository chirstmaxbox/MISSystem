
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using SpecDomain.Model;

namespace SpecDomain.BLL.Service
{
    public class MyEstService
    {
        public string DeleteResult { get; private set; }
        public EST_Service Value { get; set; }
        public int ParentID { get; set; }

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public MyEstService(int qsID)
        {
            Value = _db.EST_Service.Find(qsID);
            ParentID = Value.Sales_JobMasterList_EstRev.JobID;
        }

        public void Delete()
        {
            try
            {
                _db.Entry(Value).State = EntityState.Deleted;
                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                DeleteResult = dbEx.Message;
                throw;
            }

        }

        public MyEstService()
        {
        }

        public void Update()
        {
            try
            {
                var newValue  = _db.EST_Service.Find(Value.qsID);

                newValue.qsDescription=Value.qsDescription;
                newValue.qsAmount=Value.qsAmount;
                newValue.qsAmountText =string.Format( "{0:C2}", MyCommon.MyConvert .ConvertToDouble(Value.qsAmount));
                _db.Entry(newValue).State = EntityState.Modified;
                _db.SaveChanges();

                var sameGroupServices =_db.EST_Service.Where(x => x.EstRevID == newValue.EstRevID && x.qsServiceID == newValue.qsServiceID).ToList();
                foreach (var service in sameGroupServices )
                {
                    service.qsPrintOrder = Value.qsPrintOrder;
                    _db.Entry(service ).State = EntityState.Modified;
                }
              
                _db.SaveChanges();

            }
            catch (DbEntityValidationException dbEx)
            {
                DeleteResult = dbEx.Message;
                throw;
            }

        }
     
    }
}