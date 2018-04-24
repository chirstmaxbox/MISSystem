using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using MyCommon;
using ProjectDomain;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BLL
{
    public class WipTaskCreateNew
    {
        private readonly ProjectModelDbEntities _dbml;
        private readonly Sales_Wip _task;

        public WipTaskCreateNew(Sales_Wip task)
        {
            _task = task;
            _dbml = new ProjectModelDbEntities();
        }

        //// Create and add a new Order to the Orders collection.

        public void AddNew()
        {
            _dbml.Sales_Wip.Add(_task);
            _dbml.SaveChanges();
        }
    }

    public class WipTaskSelect
    {
        private readonly ProjectModelDbEntities _dbml;
        private readonly Sales_Wip _task;


        public WipTaskSelect(int wipTaskID)
        {
            _dbml = new ProjectModelDbEntities();
            _task = _dbml.Sales_Wip.Find(wipTaskID);
        }

        public int Responsible
        {
            get { return GetResponsible(); }
        }

        private int GetResponsible()
        {
            return _task.Responsible;
        }
    }

    public class WipTaskDelete
    {
        private readonly ProjectModelDbEntities _dbml;
        private readonly Sales_Wip _task;


        public WipTaskDelete(int wipTaskID, int deletor)
        {
            _dbml = new ProjectModelDbEntities();
            _task = _dbml.Sales_Wip.SingleOrDefault(x => x.WipTaskID == wipTaskID);

            CheckIsDeletable(deletor);
        }

        public bool IsDeletable { get; private set; }
        public string ErrorString { get; private set; }


        public void Delete()
        {
            if (_task != null)
            {
                _dbml.Entry(_task).State = EntityState.Deleted;
                _dbml.SaveChanges();
            }
        }


        private void CheckIsDeletable(int deletor)
        {
            //Owner
            //Status !=Finish
            IsDeletable = true;

            if (_task.Status == (int) NWipTask.StatusFinish)
            {
                IsDeletable = false;
                ErrorString = "Request Failed, Finished task can not be deleted";
            }


            if (_task.CreatedBy != deletor)
            {
                IsDeletable = false;
                ErrorString = "Request Failed, You are not authorized, only creator can delete the task";
            }
        }
    }

    public class WipTaskSchedule
    {
        private readonly DateTime _targetDate; //In Most case, it's target date
        private readonly tblWipTask _task;
        private DateTime _scheduleStartDate;

        public WipTaskSchedule(int contentID, DateTime targetDate)
        {
            _targetDate = MyDateTime.GetDateOfMinusBusinessDays(targetDate, -1);


            var dbml = new ProjectModelDbEntities();
            _task = dbml.tblWipTasks.First(x => x.ContentID == contentID);
            _scheduleStartDate = DateTime.Today;
        }

        public WipTaskSchedule(int contentID, DateTime targetDate, DateTime scheduleStartDate)
        {
            _targetDate = MyDateTime.GetDateOfMinusBusinessDays(targetDate, -1);
            var dbml = new ProjectModelDbEntities();
            _task = dbml.tblWipTasks.Single(x => x.ContentID == contentID);
            _scheduleStartDate = scheduleStartDate;
            if (_scheduleStartDate < DateTime.Today)
            {
                _scheduleStartDate = DateTime.Today;
            }
        }

        public DateTime ScheduledStartDate
        {
            get { return GetStartDate(); }
        }

        public DateTime ScheduledFinishDate
        {
            get { return GetFinishDate(); }
        }


        private DateTime GetStartDate()
        {
            //lead time           
            DateTime dt = DateTime.Today;
            return dt;
        }


        private DateTime GetFinishDate()
        {
            DateTime dt = _targetDate;

            //if leadtime <>0 then leadTime+startDate
            if (_task.LeadTime != 0)
                dt = _scheduleStartDate.AddDays(_task.LeadTime);

            //should between today and _endDate
            if (dt < DateTime.Today)
            {
                dt = DateTime.Today;
            }

            if (dt > _targetDate)
            {
                dt = _targetDate;
            }

            return dt;
        }
    }

    public class WipTaskContent
    {
        private readonly ProjectModelDbEntities _dbml;
        private readonly tblWipTask _taskContent;


        public WipTaskContent(int contentID)
        {
            _dbml = new ProjectModelDbEntities();
            _taskContent = _dbml.tblWipTasks.First(x => x.ContentID == contentID);
        }

        public int StageID
        {
            get { return GetStageID(); }
        }

        private int GetStageID()
        {
            return _taskContent.StageID;
        }
    }


    public class WipTaskFinish
    {
        private readonly ProjectModelDbEntities _dbml;
        private readonly int _employeeID;
        private readonly int _jobID;

        public WipTaskFinish(int jobID, int employeeID)
        {
            _jobID = jobID;
            _employeeID = employeeID;
            _dbml = new ProjectModelDbEntities();
        }

        public void UpdateTaskes()
        {
            List<Sales_Wip> internalTaskes = _dbml.Sales_Wip.Where(x => x.JobID == _jobID).ToList();

            foreach (Sales_Wip task in internalTaskes)
            {
                task.Status = 90;
                task.LastUpdatedAt = DateTime.Now;
                task.LastUpdatedBy = _employeeID;
                _dbml.Entry(task).State = EntityState.Modified;
            }
            _dbml.SaveChanges();
        }
    }


    public class WipTaskPublicCreateNew
    {
        private readonly int _employeeID;
        private readonly int _jobID;

        public WipTaskPublicCreateNew(int jobID, int employeeID)
        {
            _jobID = jobID;
            _employeeID = employeeID;
        }

        public void CreateNewTaskes()
        {
            var dbml = new ProjectModelDbEntities();
            List<tblWipTask> taskes = dbml.tblWipTasks.Where(x => x.IsPublic).ToList();

            foreach (tblWipTask task in taskes)
            {
                var newTask = new Sales_WipPublic
                                  {
                                      JobID = _jobID,
                                      estRevID = 0,
                                      quoteRevID = 0,
                                      woID = 0,
                                      invoiceID = 0,
                                      Priority = 0,
                                      ContentID = task.ContentID,
                                      Contents = task.PublicContentName,
                                      Status = 0,
                                      ScheduleStartDate = DateTime.Today,
                                      ScheduleFinishDate = DateTime.Today.AddDays(task.LeadTime),
                                      Responsible = _employeeID,
                                      CreatedBy = _employeeID,
                                      LastUpdatedAt = DateTime.Now,
                                      LastUpdatedBy = _employeeID
                                  };

                dbml.Sales_WipPublic.Add(newTask);
            }
            dbml.SaveChanges();
        }

        public void AddOtherTask()
        {
            var dbml = new ProjectModelDbEntities();

            var newTask = new Sales_WipPublic
                              {
                                  JobID = _jobID,
                                  estRevID = 0,
                                  quoteRevID = 0,
                                  woID = 0,
                                  invoiceID = 0,
                                  Priority = 0,
                                  ContentID = 65531,
                                  Contents = "My Contents Here",
                                  Status = 0,
                                  ScheduleStartDate = DateTime.Today,
                                  ScheduleFinishDate = DateTime.Today,
                                  Responsible = _employeeID,
                                  CreatedBy = _employeeID,
                                  LastUpdatedAt = DateTime.Now,
                                  LastUpdatedBy = _employeeID
                              };

            dbml.Sales_WipPublic.Add(newTask);
            dbml.SaveChanges();
        }
    }


    public class WipTaskPublicUpdateFromInternal
    {
        private readonly ProjectModelDbEntities _dbml;
        private readonly int _employeeID;
        private readonly int _jobID;

        public WipTaskPublicUpdateFromInternal(int jobID, int employeeID)
        {
            _jobID = jobID;
            _employeeID = employeeID;
            _dbml = new ProjectModelDbEntities();
        }

        public void UpdateTaskes()
        {
            List<tblWipTask> taskes = _dbml.tblWipTasks.Where(x => x.IsPublic).ToList();
            List<Sales_Wip> internalTaskes = _dbml.Sales_Wip.Where(x => x.JobID == _jobID).ToList();

            foreach (tblWipTask task in taskes)
            {
                int contentID = task.ContentID;
                Sales_Wip internalTask = GetInternalTask(internalTaskes, contentID);
                Sales_WipPublic newTask;
                if (internalTask == null)
                {
                    newTask = GetNewTask(task);
                }
                else
                {
                    newTask = GetNewTask(internalTask);
                }
                _dbml.Sales_WipPublic.Add(newTask);
            }
            _dbml.SaveChanges();
        }


        private Sales_Wip GetInternalTask(IEnumerable<Sales_Wip> internalTaskes, int contentID)
        {
            return internalTaskes.FirstOrDefault(x => x.ContentID == contentID);
        }

        private Sales_WipPublic GetNewTask(tblWipTask task)
        {
            var newTask = new Sales_WipPublic
                              {
                                  JobID = _jobID,
                                  estRevID = 0,
                                  quoteRevID = 0,
                                  woID = 0,
                                  invoiceID = 0,
                                  Priority = 0,
                                  ContentID = task.ContentID,
                                  Contents = task.ContentName,
                                  Status = 0,
                                  ScheduleStartDate = DateTime.Today,
                                  ScheduleFinishDate = DateTime.Today.AddDays(task.LeadTime),
                                  Responsible = _employeeID,
                                  CreatedBy = _employeeID,
                                  LastUpdatedAt = DateTime.Now,
                                  LastUpdatedBy = _employeeID
                              };

            return newTask;
        }


        private Sales_WipPublic GetNewTask(Sales_Wip internalTask)
        {
            var newTask = new Sales_WipPublic
                              {
                                  JobID = _jobID,
                                  estRevID = 0,
                                  quoteRevID = 0,
                                  woID = 0,
                                  invoiceID = 0,
                                  Priority = 0,
                                  ContentID = internalTask.ContentID,
                                  Contents = GetContentName(internalTask.ContentID),
                                  Status = 0,
                                  ScheduleStartDate = internalTask.ScheduleStartDate,
                                  ScheduleFinishDate = internalTask.ScheduleFinishDate,
                                  Responsible = internalTask.Responsible,
                                  CreatedBy = _employeeID,
                                  LastUpdatedAt = DateTime.Now,
                                  LastUpdatedBy = _employeeID
                              };

            return newTask;
        }

        private string GetContentName(int contentID)
        {
            string contents = "";
            tblWipTask wiptask = _dbml.tblWipTasks.FirstOrDefault(x => x.ContentID == contentID);
            if (wiptask != null)
            {
                contents = wiptask.PublicContentName;
            }
            return contents;
        }
    }
}