using System.Collections.Generic;
using System.Data.Entity.Validation;
using ProjectDomain;

namespace SalesCenterDomain.BLL
{
    public class MySalesDispatching
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private readonly int _jobID;

        public MySalesDispatching(int jobID)
        {
            _jobID = jobID;
        }

        public long ID { get; private set; }


        public void Create(Sales_Dispatching salesDispatching)
        {
            salesDispatching = OverrideResponsible(salesDispatching);
            _db.Sales_Dispatching.Add(salesDispatching);

            try
            {
                //Check Validation Errors
                var error = _db.GetValidationErrors();
                _db.SaveChanges();
                ID = salesDispatching.TaskID;
            }
            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
            }
        }

        private Sales_Dispatching OverrideResponsible(Sales_Dispatching salesDispatching)
        {
            return salesDispatching;
        }
    }
}