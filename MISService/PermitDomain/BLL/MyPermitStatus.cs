using PermitDomain.Model;

namespace PermitDomain.BLL
{
	public class MyPermitStatus
	{
		public PermitStatu Value { get; private set; }
		private readonly PermitDbEntities _db = new PermitDbEntities();

        public MyPermitStatus(int statusID)
		{
            Value =_db.PermitStatus.Find( statusID );
		}
	
	}
}