using System.Collections.Generic;
using System.Linq;
using SubContractDomain.Model;

namespace SubContractDomain.BLL
{
	public class MyProjectSubContract
	{
		public List<SubContract>  Values { get; set; }
		private readonly int _projectID;
		private readonly SubContractDbEntities _db = new SubContractDbEntities();

		public MyProjectSubContract(int projectID)
		{
			_projectID = projectID;
			Values = _db.SubContracts.Where (x=>x.JobID ==_projectID ).OrderByDescending(x=>x.RequestNumber) .ToList();
		}

	
		public int GetFirstSubContractID()
		{
			if (Values.Count == 0) return 0;

			var sc = Values.First();
			return sc.SubcontractID;

		}




	}
}