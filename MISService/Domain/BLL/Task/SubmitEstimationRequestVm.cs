using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyCommon;
using SpecDomain.BLL.EstItem;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.Task
{
    public class SubmitEstimationRequestVm:Sales_Dispatching 
    {
        public IEnumerable<SelectListItem> Estimators { get; set; }
        public int EstimatorID { get; set; }
        public string FormatedRequiredTime { get; set; }
        public string Result { get; set; }
        //public long TaskID { get; set; }

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        public SubmitEstimationRequestVm()
        {}
        
        public void Create()
        {
            InitializeCreateData();
            var task = new Sales_Dispatching();
            MyReflection.Copy(this,task);
            task.Responsible = (short) EstimatorID; 
            _db.Sales_Dispatching.Add(task);
            _db.SaveChanges();

            TaskID = task.TaskID; 

        }


        public void RefreshDropdownlist()
        {
            var t1 = new SelectListItem() {Value = "8", Text = "Estimation Dept."};
            var t2 = new SelectListItem() { Value = "93", Text = "James" };
            Estimators = new List<SelectListItem>() {t1, t2};
        }

        private void InitializeCreateData()
        {
            //3 fields from UI: RequiredTime, SubmitBy and Description

            TaskType = 201;
            RequestType = "job";
            Subject = "Estimation";
            Status = 201;

            var est = _db.Sales_JobMasterList_EstRev.Find(EstRevID);
            JobID = est.JobID;

            //By Configuration
       //     

            SubmitTime = DateTime.Now;
            LastUpdateTime = DateTime.Now;
            RequiredTime = MyConvert.ConvertToDate(FormatedRequiredTime);

            Rush = GetRush();

            //No-Use
            QuoteRevID = 0;
            WoID = 0;
            InvoiceID = 0;
            Priority = 0;
            Importance = 0;
            OwnerFinish = false;
            ResponsibleFinish = false;
            IncompleteInfo = false;

        }

        public Boolean GetRush()
        {
            bool b = false;

            int hours = MyCommon.MyDateTime.GetDiffHoursOfWeekday(Convert.ToDateTime( SubmitTime), Convert .ToDateTime( RequiredTime));
            if (hours < SpecDomain.BO.SpecConfiguration .ResponseTimeEstimation)
            {
                b = true;
            }
            return b;
        }

        //public void OnEstimationSubmitted()
        //{
        //    var items = _db.EST_Item.Where(x => x.EstRevID == EstRevID &&
        //                                        (x.StatusID == (int)NEstItemStatus.New | x.StatusID == (int)NEstItemStatus.ContentsChanged) &&
        //                                        x.ItemPurposeID == (short)NEstItemPurpose.ForEstimation
        //                                        ).ToList();
            
        //    //Save Existing Item to backup
        //    foreach (var item in items)
        //    {
        //        item.StatusID = (short)NEstItemStatus.Submitted;
        //        item.ItemPurposeID = (short)NEstItemPurpose.ForBackup;
        //        item.Version += 1;
        //        _db.Entry(item).State = EntityState.Modified;
        //    }
        //    _db.SaveChanges();

        //    //Copy For Estimation
        //    foreach (var item in items)
        //    {
        //        MyEstItemCopy itemCopy = new MyEstItemCopyForEstimation(item.EstItemID);
        //        itemCopy.Copy();
        //    }
           
        //    //Version +=1
        //    var est = _db.Sales_JobMasterList_EstRev.Find(EstRevID);
        //    est.erRev += 1;
        //    _db.Entry(est).State = EntityState.Modified;
        //    _db.SaveChanges();

        //}


        public void OnEstimationSubmitted()
        {
            var items = _db.EST_Item.Where(x => x.EstRevID == EstRevID &&
                                                (x.StatusID == (int)NEstItemStatus.New | x.StatusID == (int)NEstItemStatus.ContentsChanged) &&
                                                x.ItemPurposeID == (short)NEstItemPurpose.ForEstimation
                                                ).ToList();

            //Save Existing Item to backup
            foreach (var item in items)
            {
                item.StatusID = (short)NEstItemStatus.Submitted;
                item.Version += 1;
                _db.Entry(item).State = EntityState.Modified;
            }
            _db.SaveChanges();

            //Version +=1
            var est = _db.Sales_JobMasterList_EstRev.Find(EstRevID);
            est.erRev += 1;
            _db.Entry(est).State = EntityState.Modified;
            _db.SaveChanges();

            var task = _db.Sales_Dispatching.Find(TaskID);
            task.Importance = est.erRev;
            _db.Entry(task).State = EntityState.Modified;
            _db.SaveChanges();

        }


        
        public SubmitEstimationRequestVm(long taskID)
        {
            //EstRev
            var task = _db.Sales_Dispatching.Find(taskID);
            MyReflection.Copy(task, this);


        }


    }

   
}



