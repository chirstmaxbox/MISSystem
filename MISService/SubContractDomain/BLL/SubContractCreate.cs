using System.Linq;
using System.Text;
using System.Data.Entity.Validation;
using SubContractDomain.Model;

namespace SubContractDomain.BLL
{
    public class SubContractCreate
    {
        private readonly int _printingEmployeeID;
        public int NewlyInsertedID { get; private set; }

        private readonly SubContractDbEntities _db = new SubContractDbEntities();

        public SubContractCreate(int printingEmployeeID)
        {
            _printingEmployeeID = printingEmployeeID;
        }


        public void Create(SubContract subContract)
        {
        	var maxRequestNumber = 0;
            var jobID = subContract.JobID;
        	var ets = _db.SubContracts.Where(x => x.JobID == jobID).ToList();
			if (ets.Count > 0)
			{
				foreach (var et in ets)
				{
					if (et.RequestNumber > maxRequestNumber)
					{
						maxRequestNumber = et.RequestNumber;
					}
				}
			}

        	subContract.RequestNumber = maxRequestNumber + 1;
            _db.SubContracts.Add(subContract);
            try
            {
                //Check Validation Errors
                var error = _db.GetValidationErrors();
				
                _db.SaveChanges();
                NewlyInsertedID = subContract.SubcontractID;

            }
            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
            }

        }

		
       public void Create(SubcontractResponse  subContractResponse)
        {
            var subContractID = subContractResponse.SubcontractID;
            if (subContractID <= 0) return;

            var scr = _db.SubcontractResponses .SingleOrDefault(x => x.SubcontractID  == subContractID );
            if (scr != null) return;

            _db.SubcontractResponses .Add(subContractResponse);
            try
            {
                //Check Validation Errors
                var error = _db.GetValidationErrors();
                _db.SaveChanges();
                NewlyInsertedID = subContractResponse.ResponseID;
            }
            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
            }

        }


       public void Create(SubcontractDocument subcontractDocument )
        {
           _db.SubcontractDocuments .Add( subcontractDocument );
            try
            {
                //Check Validation Errors
                var error=_db.GetValidationErrors();  
                _db.SaveChanges();

            }
            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
            }
        }

        

    }
}

