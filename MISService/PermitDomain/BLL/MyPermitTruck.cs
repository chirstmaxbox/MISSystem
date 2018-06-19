
using PermitDomain.Model;


namespace PermitDomain.BLL
{
    public class MyPermitTruckType
    {
      
	    private readonly PermitDbEntities _db = new PermitDbEntities();
   		public PermitTruckType Value { get; private set; }

        private readonly int _truckTypeID;

        public MyPermitTruckType(int truckTypeID)
		{
            _truckTypeID= truckTypeID;
	        Value = _db.PermitTruckTypes.Find(truckTypeID );
		}



    }
}

