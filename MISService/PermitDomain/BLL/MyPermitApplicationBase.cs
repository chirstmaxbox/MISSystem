using System.Collections.Generic;
using System.Data;
using System.Linq;
using PermitDomain.Model;

namespace PermitDomain.BLL
{
	public  class MyPermitApplicationBase
	{
     //   public string Result { get; private set; }
		public PermitBase Value { get; private set; }

		//private readonly int _baseAppID;
		private readonly PermitDbEntities _db = new PermitDbEntities();

	    public MyPermitApplicationBase(int baseAppID)
        {
          Value = _db.PermitBases.Find(baseAppID);
        }

		public void Edit(PermitBase applicationBase)
		{
			Value.Deadline = applicationBase.Deadline;
			Value.JobTitle = applicationBase.JobTitle;
			Value.Remark = applicationBase.Remark;
		    _db.Entry(Value).State = EntityState.Modified;
            _db.SaveChanges();
		}

	}

    public class MyPermits
    {
        public List<PermitBase> BaseValues { get; private set; }

        private readonly PermitDbEntities _db = new PermitDbEntities();
        
        public MyPermits(int jobID)
        {
            BaseValues = _db.PermitBases.Where(x => x.JobID == jobID).ToList();
        }

    }
}