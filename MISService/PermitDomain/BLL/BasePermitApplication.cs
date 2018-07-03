using System;
using System.Data;
using PermitDomain.Model;
using System.Linq;
using System.Data.Entity;

namespace PermitDomain.BLL
{
	public abstract class BasePermit
	{
        public PermitBase BaseValue { get; private set; }
		private readonly int _baseAppID;
		private readonly PermitDbEntities _db = new PermitDbEntities();

		protected BasePermit(int baseAppID)
		{
			_baseAppID = baseAppID;
			BaseValue = _db.PermitBases.Find(baseAppID);
		}

		#region *************************** Delete *************************
		
		public string DeleteBase()
		{
			if (!IsDeletable()) return "The Request Has been Submited, Can not be Deleted";

			DeleteChildRecord();

			var docs = _db.PermitDocuments.Where(x => x.BaseAppID == _baseAppID).ToList();
			foreach (var doc in docs)
			{
				_db.Entry(doc).State = EntityState.Deleted;
			}

            var comms = _db.PermitCommunications.Where(x => x.BaseAppID == _baseAppID).ToList();
            foreach (var comm in comms)
            {
                _db.Entry(comm ).State = EntityState.Deleted;
            }
			_db.SaveChanges();

			_db.Entry(BaseValue).State = EntityState.Deleted;
			_db.SaveChanges();
			return "ok";

		}

		public abstract void  DeleteChildRecord();
	

		private  bool IsDeletable()
		{
			return BaseValue.StatusID == (int)NPermitStatus.New ;
		}
		#endregion

		#region ********************** Submit *****************************

		public string Submit()
		{
			var r = IsValidated();
			if (r=="ok" )
			{
				//Submit
				BaseValue.StatusID =(int) NPermitStatus.Submit;
				BaseValue.RequestDate = DateTime.Now;  
				_db.Entry(BaseValue).State = EntityState.Modified;
				_db.SaveChanges();
			}

			return r;
        }

		public abstract string IsValidated();

	
		#endregion


		public void UpdateStatusToRevise()
		{
			BaseValue.StatusID = (int) NPermitStatus.Revised;
			_db.Entry(BaseValue).State = EntityState.Modified;
			_db.SaveChanges();
		}


	}
}