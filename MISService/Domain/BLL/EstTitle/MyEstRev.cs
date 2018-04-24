using System;

using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using SpecDomain.BLL.EstItem;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.EstTitle
{
	public class MyEstRev
	{
		public Sales_JobMasterList_EstRev Value { get; private set; }

        private readonly int _estRevID;
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();


		public MyEstRev(int estRevID)
		{
			_estRevID = estRevID;
			Value = _db.Sales_JobMasterList_EstRev.Find(estRevID);
		}


		public void EstimatorFinished()
		{
			Value.erLocked  = false;
			//ItemStauts=Estimated if purpose=estimation
			var existingItems = _db.EST_Item .Where (x => x.EstRevID == _estRevID & 
														  x.ItemPurposeID==(int)NEstItemPurpose.ForEstimation  &
														  x.StatusID !=(int)NEstItemStatus.Estimated &
                                                          x.StatusID != (int)NEstItemStatus.New 
													).ToList();
            // Update Status to Estimated
			foreach (var item in existingItems)
			{
				item.StatusID  = (int) NEstItemStatus.Estimated;
			}
            _db.Entry(Value).State = EntityState.Modified;
			_db.SaveChanges();
            
            //Copy to Backup
            foreach (var item in existingItems)
            {
                var estItemCopy = new MyEstItemCopyToBackup(item.EstItemID);
                estItemCopy.Copy();
            }

		}

        public bool IsThisEstimationHaveChildren()
        {
            var existingItems = _db.EST_Item.Where(x => x.EstRevID == _estRevID &
                                              x.ItemPurposeID == (int)NEstItemPurpose.ForEstimation
                                        ).ToList();
            return existingItems.Any();

        }

        public bool IsThisEstimationHasBenSubmited()
        {
            if (Value.erRev > 0) return true;
            if (MyCommon.MyString.IsStringLengthLongerThan(3, Value.noEstimationReason)) return true;
            return false;
        }

	}


    public class MyEstRevCreate
    {

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public int NewEstRevID { get; set; }

        public void Create(int jobID)
        {
            //Create 
            //Set EstRevID
            CreateSalesJobMasterListEstRev(jobID);

            //
            CreateConfig();
            //CreateFirstItem();
        }


        private void CreateSalesJobMasterListEstRev(int jobID)
        {

            var job = new SpecProjectDetail(jobID);
            var sa1ID = job.Sa1ID;

            //public int EstRevID { get; set; }
            var est = new Sales_JobMasterList_EstRev
                          {
                              JobID = jobID, 
                              erRev = 0, 
                              erLocked = false, 
                              erAmount = 0, 
                              sa1ID = sa1ID, 
                              TaskID = 0
                          };

            //Save to DB
            _db.Sales_JobMasterList_EstRev.Add(est);
            _db.SaveChanges();

            //Output
            NewEstRevID = est.EstRevID;
        }

        private void CreateConfig()
        {
            try
            {
                var config = _db.EST_Cost_Configuration.Find(0);
                var newConfig = new EST_Cost_Configuration();
                MyCommon.MyReflection.Copy(config, newConfig);
                newConfig.EstRevID = NewEstRevID;
                _db.EST_Cost_Configuration.Add(newConfig);
                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
                throw;
            }
        }


    }


    public class MyEstRevCreateConfiguration
    {

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();


        public MyEstRevCreateConfiguration(int estRevID)
        {
            try
            {
                var config = _db.EST_Cost_Configuration.Find(0);
                var newConfig = new EST_Cost_Configuration();
                MyCommon.MyReflection.Copy(config, newConfig);
                newConfig.EstRevID = estRevID;
                _db.EST_Cost_Configuration.Add(newConfig);
                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
                throw;
            }
        }


    }

}    

