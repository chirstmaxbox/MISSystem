using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using CustomerDomain.BO;
using CustomerDomain.Model;
using MyCommon;

namespace CustomerDomain.BLL
{
    public class MyCustomers
    {

        private readonly CustomerDbEntities _db = new CustomerDbEntities();

        public List<CUSTOMER> GetFranchisees(int customerID)
        {
            return _db.CUSTOMERs.Where(x => x.HEADOFFICE == customerID && x.selectType == 11).ToList();
        }

		public List<CUSTOMER> GetCustomersByTeamAndSales(int teamID, int myID)
		{
			return _db.CUSTOMERs.Where(x => myID == x.SalesID ||
			                                myID == x.Sa1ID ||
			                                myID == 5020 && (x.FW_Employees.Team == teamID || teamID == 0)
				).OrderBy(x => x.NAME).ToList();
		}


		public List<CUSTOMER> GetHeadOfficesByTeamAndSales(int teamID, int myID)
		{
			return _db.CUSTOMERs.Where(x =>( myID == x.SalesID ||
											myID == x.Sa1ID ||
											myID == 5020 && (x.FW_Employees.Team == teamID || teamID == 0)
				) && x.selectType == (int)CustomerEN.NCustomerSelectType.c1Headoffice) .OrderBy(x => x.NAME).ToList();
		}

        
        //public List<CUSTOMER> GetCustomerByIDs(List< int> customerIDs)
        //{
        //    return _db.CUSTOMERs.Where(x => x.ROWID > 100 & customerIDs.Contains(x.ROWID)).ToList();

        //}
  
	}



}