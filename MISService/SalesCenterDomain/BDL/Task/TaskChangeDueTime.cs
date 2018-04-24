using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using MyCommon;
using ProjectDomain;

namespace SalesCenterDomain.BDL.Task
{
    public class TaskChangeDueTime
    {
        private readonly ProjectModelDbEntities _db;
        private readonly Sales_Dispatching _task;

        public TaskChangeDueTime(int taskID)
        {
            _db = new ProjectModelDbEntities();

            Sales_Dispatching task = _db.Sales_Dispatching.SingleOrDefault(x => x.TaskID == taskID);

            if (task != null)
            {
                _task = task;
            }
        }

        public DateTime DueTime
        {
            set { UpdateNewDueTime(value); }
        }

        private void UpdateNewDueTime(DateTime requiredTime)
        {
            int taskCategory = Convert.ToInt32(_task.TaskType);
            var tp = new TaskPropertyResponseLeadtime(taskCategory);
            int leadTime = tp.Leadtime;
            _task.Rush = false;
            int hours = MyDateTime.GetDiffHoursOfWeekday(Convert.ToDateTime(_task.SubmitTime), requiredTime);
            if (hours < leadTime)
            {
                _task.Rush = true;
            }

            _task.RequiredTime = requiredTime;
            _task.LastUpdateTime = DateTime.Now;

            _db.Entry(_task).State = EntityState.Modified;
            _db.SaveChanges();
        }
    }
}