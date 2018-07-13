using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;

using SubContractDomain.Model;
using System.Data.Entity;


namespace SubContractDomain.BLL
{

	public class MyResponse
	{
		public SubcontractResponse Value { get; private set; }
		private readonly int _responseID;
		private readonly SubContractDbEntities _db = new SubContractDbEntities();

		public MyResponse(int responseID)
		{
			_responseID = responseID;
			Value = _db.SubcontractResponses.Find(_responseID );
		}


		public void ApproveOverBudget(int employeeID)
		{
			Value.IsOverBudgetApproved=true;
			Value.ApprovedBudget = Value.Quotation;
			Value.ApprovedDate = DateTime.Now;
			Value.ApprovedBy = employeeID;

			_db.Entry(Value).State = EntityState.Modified;
			_db.SaveChanges();
		}
	
	}
}