using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using SubContractDomain.Model;
using System.Data.Entity;


namespace SubContractDomain.BLL
{

	public class MySubcontractWorkorder
	{
//		public SubcontractWorkorder Value { get; private set; }
        public List<SubcontractWorkorder> Values { get; private set; }
		private readonly int _subcontractID;
		private readonly SubContractDbEntities _db = new SubContractDbEntities();

        public MySubcontractWorkorder(int subcontractID)
        {
            _subcontractID = subcontractID;
            Values = _db.SubcontractWorkorders.Where(x => x.SubcontractID == _subcontractID).ToList();
		}

        public void Synchronize(List<int> workorders )
        {
            //Insert New
            var existingWoIDs = Values.Select(x => x.WorkorderID).ToList();
            foreach (int woID in workorders )
            {
               if(! existingWoIDs .Contains( woID ))
               {
                   var swo = new SubcontractWorkorder()
                                 {
                                     SubcontractID = _subcontractID,
                                     WorkorderID = woID,
                                 };
                   _db.SubcontractWorkorders.Add(swo);
               }
            }

            //Delete if not in new list
            foreach (SubcontractWorkorder wo in Values )
            {
                if (! workorders .Contains( wo.WorkorderID))
                {
                    _db.Entry(wo).State = EntityState.Deleted;
                }
            }
            _db.SaveChanges();
        }
	
	}
}