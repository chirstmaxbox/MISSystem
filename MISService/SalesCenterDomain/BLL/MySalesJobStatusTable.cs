using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using ProjectDomain;

namespace SalesCenterDomain.BLL
{
    public class MySalesStatusTable
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private readonly int _eID;
        private readonly int _jobID;

        public MySalesStatusTable(int jobID, int eID)
        {
            _jobID = jobID;
            _eID = eID;
        }


        public void Create(short statusID)
        {
            var newStatus = new Sales_JobStatusTable
                                {
                                    jobID = _jobID,
                                    jobStatus = statusID,
                                    Performer = _eID,
                                    accomplishDate = DateTime.Now
                                };

            _db.Sales_JobStatusTable.Add(newStatus);

            try
            {
                //Check Validation Errors
                var error = _db.GetValidationErrors();
                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
            }
        }
    }
}