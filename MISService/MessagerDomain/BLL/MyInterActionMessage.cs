using System;
using System.Collections.Generic;
using System.Linq;

namespace MessagerDomain.BLL
{
	public class MyInterActionMessage
	{
		public InterActionMessage Value { get; private set; }

		private readonly MessagerDbDataContext _dbml = new MessagerDbDataContext();
	
		public MyInterActionMessage(int messageID)
		{
			Value = _dbml.InterActionMessages.Single(x => x.MessageID == messageID);
		}

		public void Dismiss()
		{
            try
            {
                //Message
                Value.Status = (int)NInterActionMessageStatus.Dismissed;
                _dbml.SubmitChanges();

                //Message Status of Employee
                var mmes = new MyInterActionMessageEmployeeStatusUpdate(Value.Receiver);
                mmes.Update();
            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message;
                throw;
            }




		}

		public void Postpone()
		{

            try
            {
                //Message
                Value.Status = (int)NInterActionMessageStatus.Postponed;
                _dbml.SubmitChanges();

                //Message Status of Employee
                var mmes = new MyInterActionMessageEmployeeStatusUpdate(Value.Receiver);
                mmes.Update();
            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message;
                throw;
            }



		}


		public void Approve()
		{
            try
            {

                //Message of all this kinds of msg, multiple approver, one approve, then all are dismissed
                var messages = _dbml.InterActionMessages.Where(x => x.HandlingItemID == Value.HandlingItemID & x.MessageTypeID == Value.MessageTypeID).ToList();
                foreach (var message in messages)
                {
                    //Dismiss
                    message.Status = (int)NInterActionMessageStatus.Dismissed;
                    message.ResponseTime = DateTime.Now;
                    _dbml.SubmitChanges();

                    //var mp =new MyProject(message.ProjectID);
                    //mp.UpdateAbnormalWorkorderIssueReason(message.Note);
                    //mp.UpdateAbnormalWorkorderIssueStatus(10);		//New
                    //Message Status of Employee
                    var mmes = new MyInterActionMessageEmployeeStatusUpdate(message.Receiver);
                    mmes.Update();
                }	
            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message;
                throw;
            }


		}




		//Insert
		public  MyInterActionMessage()
		{}
		public void AddNew(InterActionMessage  message)
		{
            try
            {
                _dbml.InterActionMessages.InsertOnSubmit(message);
                _dbml.SubmitChanges();

                //Message Status of Employee
                var mmes = new MyInterActionMessageEmployeeStatusUpdate(message.Receiver);
                mmes.Update();
            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message;
                throw;
            }

		}

		public void AddNew(List<InterActionMessage> messages)
		{
            try
            {
                foreach (var message in messages)
                {
                    _dbml.InterActionMessages.InsertOnSubmit(message);
                    _dbml.SubmitChanges();

                    //Message Status of Employee
                    var mmes = new MyInterActionMessageEmployeeStatusUpdate(message.Receiver);
                    mmes.Update();
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message;
                throw;
            }


			
		}
	
	}
}


        