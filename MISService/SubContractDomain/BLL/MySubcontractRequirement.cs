using SubContractDomain.Model;

namespace SubContractDomain.BLL
{
	public class MySubcontractRequirement
	{
		public TblSubcontractRequirement Value { get; private set; }

		private readonly SubContractDbEntities _db = new SubContractDbEntities();

		public MySubcontractRequirement(int requirementID)
		{
			Value = _db.TblSubcontractRequirements.Find(requirementID);
		}
	}
}