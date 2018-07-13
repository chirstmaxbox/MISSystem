using System;
using System.Data;
using System.Linq;
using CustomerDomain.BLL;
using MyCommon;
using SubContractDomain.Model;
using System.Data.Entity;


namespace SubContractDomain.BLL
{

	public class MySubContract
	{
		public SubContract Value { get; private set; }
		private readonly int _subcontractID;
		private readonly SubContractDbEntities _db = new SubContractDbEntities();

		public MySubContract(int subcontractID)
		{
			_subcontractID = subcontractID;
			Value = _db.SubContracts.Find(_subcontractID);
		}

		#region *************************** Delete *************************
		
		public string Delete()
		{
			if (!IsDeletable()) return "The Request Has been Submited, Can not be Deleted";


			var docs = _db.SubcontractDocuments.Where(x => x.SubcontractID == _subcontractID).ToList();
			foreach (var doc in docs)
			{
				_db.Entry(doc).State = EntityState.Deleted;
			}
			_db.SaveChanges();

			_db.Entry(Value).State = EntityState.Deleted;
			_db.SaveChanges();
			return "ok";

		}

		private bool IsDeletable()
		{
			return Value.StatusID == (int)NSubcontractRequestStatus.New ;
		}
		#endregion

		#region ********************** Submit *****************************

		public string Submit()
		{
			var r = IsValidated();
			if (r=="ok" )
			{
				//Submit
				Value.StatusID =(int) NSubcontractRequestStatus.Submit;
				Value.RequestDate = DateTime.Now;  
				_db.Entry(Value).State = EntityState.Modified;
				_db.SaveChanges();


			}

			return r;
        }

		private string IsValidated()
		{
			var b = true;
			string validationResult = "";


			var msc = new MySalesJobMasterListCustomer(Value.JobID);
			msc.SetInstallTo();

			if (msc.CustomerID<1000)
			{
			    validationResult += "No Installation Company Selected  " + System.Environment.NewLine;
			    b = false;
			}else
			{
                var myCustomer = new MyCustomer(msc.CustomerID);
           
                if (MyConvert.ConvertToString(myCustomer.Value.ADDR_1).Length < 5)
                {
                    b = false;
                    validationResult += "No Installation Address  " + System.Environment.NewLine;
                }
			
			}


			if (Value.ContactPerson1 == 0)
			{
				validationResult += "Contact person required." + System.Environment.NewLine;
				b = false;
			}

			if (MyConvert.IsDate(Value.TargetDate))
			{
				if (Value.TargetDate <= DateTime.Today)
				{
					validationResult += "Target Date should not be early than today. " + System.Environment.NewLine;
					b = false;
				}

			}
			else
			{
				validationResult += "Invalid Target Date format. " + System.Environment.NewLine;
				b = false;
			}


			if (!MyString.IsStringLengthLongerThan(3, Value.BudgetProvideBy))
			{
				validationResult += "Budget Provide By Whom Required." + System.Environment.NewLine;
				b = false;
			}

			if (!MyString.IsStringLengthLongerThan(3,Value.Requirement))
			{
					validationResult += "Please Enter Requirement." + System.Environment.NewLine;
					b = false;
			}

			var budgetAmount = 100;

			if (Value.RequirementID>=6530)
			{
				budgetAmount = 100;			//1
			}
			
				if (Value.Budget < budgetAmount )
				{
					validationResult += "Normal Job Budget should >=$100, other Job Budget >=$100 " + System.Environment.NewLine;
					b = false;
				}

			

			return b ? "ok" : validationResult;

		}



		#endregion


		public void UpdateStatusToRevise()
		{
			Value.StatusID = (int) NSubcontractRequestStatus.Revised;
			_db.Entry(Value).State = EntityState.Modified;
			_db.SaveChanges();
		}


	}
}