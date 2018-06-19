using System;
using System.Data.Entity.Validation;
using System.Linq;
using CustomerDomain.BLL;
using PermitDomain.Model;
using ProjectSummaryDomain;


namespace PermitDomain.BLL
{
    public class CreatePermit
    {
        public int NewlyInsertedID { get; private set; }
        public int NewlyInsertedBaseAppID { get; set; }

        private readonly CreatePermitBase _foo;

        public CreatePermit(int printingEmployeeID, int jobID, int requirementID, int signpermitBaseAppIDForSignVariance)
        {
            switch (requirementID)
            {
                case (int)NPermitRequirment.SignPermit:
                    _foo = new CreatePermitForSignPermit(printingEmployeeID, jobID, requirementID);
                    break;
                case (int)NPermitRequirment.HoistingPermit:
                    _foo = new CreatePermitForHoistingPermit(printingEmployeeID, jobID, requirementID);
                    break;
                case (int)NPermitRequirment.StakeOut:
                    _foo = new CreatePermitForStakeOut(printingEmployeeID, jobID, requirementID);
                    break;
                case (int)NPermitRequirment.SignVariance:
                    _foo = new CreatePermitForSignVariance(printingEmployeeID, jobID, requirementID,signpermitBaseAppIDForSignVariance);
                    break;
            }
        }

        public void Create()
        {
            _foo.Create();
            NewlyInsertedBaseAppID = _foo.NewlyInsertedBaseAppID;
            NewlyInsertedID = _foo.NewlyInsertedID;
        }

    }

    public abstract class CreatePermitBase
    {
        private readonly int _printingEmployeeID;

        public int NewlyInsertedID { get; set; }
        public int NewlyInsertedBaseAppID { get; set; }
        public int RequestNumber { get; private set; }

        public readonly PermitDbEntities Db = new PermitDbEntities();
        private readonly int _jobID;
        private readonly int _requirementID;

        protected CreatePermitBase(int printingEmployeeID, int jobID, int requirementID)
        {
            _printingEmployeeID = printingEmployeeID;
            _jobID = jobID;
            _requirementID = requirementID;
        }

        public abstract void CreateDetail();

        public void Create()
        {
            RequestNumber = GetRequestNumber();
            CreateBaseApplication();
            CreateDetail();
        }
        
        public virtual void CreateBaseApplication()
		{
			var ab = new PermitBase()
			         	{
			         		JobID = _jobID,
							RequirementID = _requirementID,
			         		RequestNumber =RequestNumber ,
			         		RequestBy = _printingEmployeeID,
			         		RequestDate = DateTime.Now,
			         		StatusID = (int) NPermitStatus.New,
			         		Deadline = DateTime.Now,
							Version =1,
			         	};
            var mp = new MyProjectDetail(_jobID);
            ab.JobTitle =mp.JobTitle;
            ab.JobNumber = mp.JobNumber;
            ab.Description = mp.Description;

			var msc = new MySalesJobMasterListCustomer(_jobID);
			msc.SetInstallTo();

			ab.InstallToCustomerID = msc.CustomerID;
			Db.PermitBases.Add(ab);
			Db.SaveChanges();
			NewlyInsertedBaseAppID = ab.BaseAppID ;
         
		}

        private int GetRequestNumber()
        {
            int max = 0;
            var existingBaseApps = Db.PermitBases.Where(x => x.JobID == _jobID).ToList();
            if (existingBaseApps.Count > 0)
            {
                foreach (var permit in existingBaseApps)
                {
                    if (permit.RequestNumber > max)
                    {
                        max = permit.RequestNumber;
                    }
                }
            }
            return max + 1;

        }
    }

    public class CreatePermitForSignPermit : CreatePermitBase
    {
        public CreatePermitForSignPermit(int printingEmployeeID, int jobID, int requirementID)
            : base(printingEmployeeID, jobID, requirementID)
        {
        }

        public override void CreateDetail()
        {
            var sp = new PermitForSignPermit()
                         {
                             BaseAppID = NewlyInsertedBaseAppID,
                             LandlordID = 0,
                             LandlordContactID = 0,
                             NumberOfSigns = 0,
                         };

            Db.PermitForSignPermits.Add(sp);
            try
            {
                //Check Validation Errors
                var error = Db.GetValidationErrors();
                Db.SaveChanges();
                NewlyInsertedID = sp.AppID;
            }
            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
            }
        }
    }

    public class CreatePermitForHoistingPermit : CreatePermitBase
    {
        public CreatePermitForHoistingPermit(int printingEmployeeID, int jobID, int requirementID)
            : base(printingEmployeeID, jobID, requirementID)
        {
        }

        public override void CreateDetail()
        {
            var hp = new PermitForHoisting()
                         {
                             BaseAppID = NewlyInsertedBaseAppID,
                             TypeOfWork = "Install",
                             OccupationDate = DateTime.Today.AddDays(3),
                             //?
                             OccupationTimeStart = "9 AM",
                             OccupationTimeEnd = "6 PM",
                             TypeOfTruck = 0,
                             DutyCopOnly = false,
                         };

            Db.PermitForHoistings.Add(hp);
            try
            {
                //Check Validation Errors
                var error = Db.GetValidationErrors();
                Db.SaveChanges();
                NewlyInsertedID = hp.AppID;
            }
            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
            }
        }
    }

    public class CreatePermitForStakeOut : CreatePermitBase
    {
        public CreatePermitForStakeOut(int printingEmployeeID, int jobID, int requirementID)
            : base(printingEmployeeID, jobID, requirementID)
        {
        }

        public override void CreateDetail()
        {
            var so = new PermitForStakeout()
                         {
                             BaseAppID = NewlyInsertedBaseAppID,
                             WayofPointLocation = "",
                         };

            Db.PermitForStakeouts.Add(so);
            try
            {
                //Check Validation Errors
                var error = Db.GetValidationErrors();
                Db.SaveChanges();
                NewlyInsertedID = so.AppID;
            }
            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
            }
        }
    }

    public class CreatePermitForSignVariance : CreatePermitBase
    {
        private readonly MyApplicationForSignPermit _signPermit;

        public CreatePermitForSignVariance(int printingEmployeeID, int jobID, int requirementID, int signPermitBaseID)
            : base(printingEmployeeID, jobID, requirementID)
        {
            _signPermit = new MyApplicationForSignPermit(signPermitBaseID);
        }

        public override void CreateBaseApplication()
        {
            var ab = new PermitBase()
                         {
                             JobID = _signPermit.BaseValue.JobID,
                             RequirementID = (int) NPermitRequirment.SignVariance,
                             RequestNumber = RequestNumber,
                             RequestBy = _signPermit.BaseValue.RequestBy,
                             RequestDate = DateTime.Now,
                             StatusID = (int) NPermitStatus.New,
                             Deadline = DateTime.Now,
                             Version = 1,
                             JobTitle = _signPermit.BaseValue.JobTitle,
                             JobNumber = _signPermit.BaseValue.JobNumber,
                             Description = _signPermit.BaseValue.Description,
                             InstallToCustomerID = _signPermit.BaseValue.InstallToCustomerID,
                         };
            Db.PermitBases.Add(ab);
            Db.SaveChanges();
            NewlyInsertedBaseAppID = ab.BaseAppID;

        }

        public override void CreateDetail()
        {
            var sp = new PermitForSignVariance()
                         {
                             BaseAppID = NewlyInsertedBaseAppID,
                             LandlordID = _signPermit.Value.LandlordID,
                             LandlordContactID = _signPermit.Value.LandlordContactID,
                             NumberOfSigns = _signPermit.Value.NumberOfSigns,
                             ProjectValueEstimated = _signPermit.Value.ProjectValueEstimated,

                         };

            Db.PermitForSignVariances.Add(sp);
            try
            {
                //Check Validation Errors
                var error = Db.GetValidationErrors();
                Db.SaveChanges();
                NewlyInsertedID = sp.AppID;
            }
            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
            }
        }

    }

    //public class ttCreatePermitForSignVariance
    //{
    //    public int NewlyInsertedID { get; private set; }
    //    public int NewlyInsertedBaseAppID { get; set; }

    //    private readonly PermitDbEntities _db = new PermitDbEntities();

    //    private readonly MyApplicationForSignPermit _signPermit; 
    //    public CreatePermitForSignVariance(int signPermitBaseID)
    //    {
    //        _signPermit = new MyApplicationForSignPermit(signPermitBaseID);
    //    }

    //    public void Create()
    //    {
    //        CreateBaseApplication();
    //        CreateSignVariance();
    //    }

    //    private void CreateBaseApplication()
    //    {
    //        var ab = new PermitBase()
    //                     {
    //                         JobID =_signPermit.Value.JobID,
    //                         RequirementID =(int)NPermitRequirment.SignVariance ,
    //                         RequestNumber = GetRequestNumber(),
    //                         RequestBy = _signPermit.Value.RequestBy,
    //                         RequestDate = DateTime.Now,
    //                         StatusID = (int) NPermitStatus.New,
    //                         Deadline = DateTime.Now,
    //                         Version = 1,
    //                         JobTitle = _signPermit.Value.JobTitle ,
    //                         JobNumber =_signPermit.Value.JobNumber,
    //                         Description = _signPermit.Value.Description,
    //                         InstallToCustomerID = _signPermit.Value.InstallToCustomerID ,
    //                     };
    //        _db.PermitBases.Add(ab);
    //        _db.SaveChanges();
    //        NewlyInsertedBaseAppID = ab.BaseAppID;

    //    }

    //    private void CreateSignVariance()
    //    {
    //        var sp = new PermitForSignVariance()
    //        {
    //            BaseAppID = NewlyInsertedBaseAppID,
    //            LandlordID = _signPermit.Value.LandlordID ,
    //            LandlordContactID = _signPermit .Value .LandlordContactID, 
    //            NumberOfSigns = _signPermit .Value .NumberOfSigns ,
    //            ProjectValueEstimated =_signPermit .Value .ProjectValueEstimated ,

    //        };

    //        _db.PermitForSignVariances.Add(sp);
    //        try
    //        {
    //            //Check Validation Errors
    //            var error = _db.GetValidationErrors();
    //            _db.SaveChanges();
    //            NewlyInsertedID = sp.AppID;
    //        }
    //        catch (DbEntityValidationException dbEx)
    //        {
    //            var s = dbEx.Message;
    //        }
    //    }




    //}

    
}

