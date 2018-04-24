using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagerDomain.BLL
{
	public class MyInterActionMessageEmployeeStatus
	{
		public InterActionMessageEmployeeStatus Value { get; private set; }

		private readonly MessagerDbDataContext _dbml = new MessagerDbDataContext();
		private readonly string _userName;

		public MyInterActionMessageEmployeeStatus(string userName)
		{
			if (userName == null)
			{
				Value = GetNullValue();
				return;
			}

			if (userName.Length <2)
			{
				Value = GetNullValue();
				return;
			}

			_userName = userName;

			Value = GetValue();

		}

		private InterActionMessageEmployeeStatus GetNullValue()
		{
			var nullResult = new InterActionMessageEmployeeStatus
			{
				EmployeeID = 0,
				UserName = "NA",
				NewMessageStatus = 0
			};
			return nullResult;
		}

		private InterActionMessageEmployeeStatus GetValue()
		{
			var iam = _dbml.InterActionMessageEmployeeStatus.FirstOrDefault(x => x.UserName == _userName);
			if ( iam!= null)
			{
				var result = new InterActionMessageEmployeeStatus
				{
					EmployeeID = iam.EmployeeID,
					UserName = iam.UserName ,
					NewMessageStatus = iam.NewMessageStatus,
				};
				return result;
			}

			var employee = _dbml.FW_Employees.FirstOrDefault(x => x.UserName == _userName);
			var eID = 0;
			if (employee !=null )
			{
				eID = employee.EmployeeNumber;
			}

			var nullResult = new InterActionMessageEmployeeStatus
			                 	{
			                 		EmployeeID = eID,
			                 		UserName = _userName,
			                 		NewMessageStatus = 0
			                 	};
			if (eID!=0)
			{
				_dbml.InterActionMessageEmployeeStatus.InsertOnSubmit(nullResult);
				_dbml.SubmitChanges();
			}
			
			return nullResult;
		}


	}


	public class MyInterActionMessageEmployeeStatusUpdate
	{

		private readonly MessagerDbDataContext _dbml = new MessagerDbDataContext();

		private readonly int _employeeID;

		public MyInterActionMessageEmployeeStatusUpdate(int employeeID)
		{
            if (employeeID <10) return;

			_employeeID = employeeID;

			var es = _dbml.InterActionMessageEmployeeStatus.SingleOrDefault  (x => x.EmployeeID == _employeeID);

			if (es==null )
			{
				var emp = _dbml.FW_Employees.Single(x => x.EmployeeNumber == _employeeID);
				var nullResult = new InterActionMessageEmployeeStatus
				{
					EmployeeID = _employeeID ,
					UserName =emp.UserName ,
					NewMessageStatus = 0
				};
				_dbml.InterActionMessageEmployeeStatus.InsertOnSubmit(nullResult);
				_dbml.SubmitChanges();
			}

		}

		private int GetStatus()
		{
			var status = (int) NInterActionEmployeeMessageStatus.NoNewMessage;
			var msgs =_dbml.InterActionMessages.Where(x => x.Receiver == _employeeID & x.Status == (int) NInterActionMessageStatus.New).ToList();
			if (msgs.Count > 0)
			{
				status = (int) NInterActionEmployeeMessageStatus.ThereIsNewMessage;
				msgs = msgs.Where(x => x.Priority == (int) NInterActionMessagePriority.Urgent).ToList();
				if (msgs.Count > 0)
				{
					status = (int) NInterActionEmployeeMessageStatus.ThterIsNewUrgentMessage;
				}
			}

			return status;

		}

		public void Update()
		{
			var es = _dbml.InterActionMessageEmployeeStatus.Single(x => x.EmployeeID == _employeeID);
			es.NewMessageStatus = GetStatus();
			_dbml.SubmitChanges();
		}

	}

}


         //   _dbml.Sales_Wips.DeleteOnSubmit(_task);
         //   _dbml.SubmitChanges();  


         
			//var internalTaskes = _dbml.Sales_Wips.Where(x => x.JobID == _jobID).ToList();
           
			//foreach (var task in internalTaskes )
			//{
			//    task.Status = 90;
			//    task.LastUpdatedAt = DateTime.Now;
			//    task.LastUpdatedBy = _employeeID;
			//}
			//_dbml.SubmitChanges();

