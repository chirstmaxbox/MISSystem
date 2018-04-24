using System;

using System.Data.Entity;
using System.Linq;
using MyCommon;
using SpecDomain.Model;

namespace SpecDomain.BLL.Task
{
    public class SalesDispatchingDrawingRequisitionEstimation
    {
        
        public void OnSalesDispatchingTaskDeleted(long salesDispatchingTaskID)
        {
        
            var db = new SpecificationDbEntities();
            var reqTitle = db.Sales_Dispatching_DrawingRequisition_Estimation.FirstOrDefault(x => x.SubmitID==salesDispatchingTaskID);
            if (reqTitle==null ) return;

            reqTitle.SubmitID = 0;
            var ver = reqTitle.Version;
            if (ver >1)
            {
                ver = ver - 1;
                reqTitle.Version = ver;
            }
            else
            {
                reqTitle.Version = 0;
            }
            db.Entry(reqTitle).State = EntityState.Modified;
            db.SaveChanges();

            if (reqTitle.Version > 0) return;
 
            //Reset Item Status to Original
            var items = db.EST_Item.Where(x => x.EstRevID ==reqTitle .EstRevID ).ToList();
            foreach (var item in items)
            {
                if (item.Sales_Dispatching_DrawingRequisition_EstimationItem==null ) continue;
                if (!item.Sales_Dispatching_DrawingRequisition_EstimationItem.Any( )) continue;
                foreach (var req in item.Sales_Dispatching_DrawingRequisition_EstimationItem)
                {
                        req.Status = 0;
                        db.Entry(req).State = EntityState.Modified;
                 }
            }
            db.SaveChanges();
          
        }
    }


    public class SubmitDrawingRequestVm:Sales_Dispatching 
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        public string DrawingType { get; set; }
        public string FormatedRequiredTime { get; set; }
        public int RequisitionID { get; set; }
        public string Result { get; set; }
        public bool IsValidated { get; set; }

        public SubmitDrawingRequestVm()
        {}

        public SubmitDrawingRequestVm(int requisitionID, int eID)
        {
            RequisitionID = requisitionID;
            SubmitBy = eID;
          
            var req = _db.Sales_Dispatching_DrawingRequisition_Estimation.Find(requisitionID);
            EstRevID = req.EstRevID;
            JobID = req.ProjectID;

            DrawingType = req.DrawingType;

            IsValidated = _db.Sales_Dispatching_DrawingRequisition_EstimationItem.Where(x => x.IsIncludedWhenPrint).ToList().Any();
            if (!IsValidated) return;
        }
        
        public void Create()
        {
            InitializeCreateData();
            var task = new Sales_Dispatching();
            MyReflection.Copy(this,task);
            _db.Sales_Dispatching.Add(task);
            _db.SaveChanges();
            
            OnSubmitted(task.TaskID);
        }

        private void InitializeCreateData()
        {
            //3 fields from UI: RequiredTime, SubmitBy and Description
            if(DrawingType.Trim() =="Graphic")
            {
                TaskType = 501;
                RequestType = "job";
                Status = 501;
                Responsible = (short)SpecDomain.BO.SpecConfiguration.GRAPHIC_DRAWING_EID;
            }
            else
            {
                TaskType = 551;
                RequestType = "job";
                Status = 551;
                Responsible = (short)SpecDomain.BO.SpecConfiguration.STRUCTURAL_DRAWING_EID;
            }
            
            var est = _db.Sales_JobMasterList_EstRev.Find(EstRevID);
            JobID = est.JobID;

            var reqTitle = est.Sales_Dispatching_DrawingRequisition_Estimation.First();
            Subject = reqTitle.Sales_Dispatching_DrawingReq_Purpose.dpName;
 
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
            if (hours < SpecDomain.BO.SpecConfiguration.ResponseTimeArtDrawing)
            {
                b = true;
            }
            return b;
        
        }

        private void OnSubmitted(long taskID)
        {
            //Item
            var items = _db.EST_Item.Where(x => x.EstRevID == EstRevID ).ToList();
            
            foreach (var item in items)
            {
                if (item.Sales_Dispatching_DrawingRequisition_EstimationItem==null ) continue;
                if (!item.Sales_Dispatching_DrawingRequisition_EstimationItem.Any( )) continue;
                foreach (var req in item.Sales_Dispatching_DrawingRequisition_EstimationItem)
                {
                    if (!req.IsIncludedWhenPrint) continue;
                    if (req.Status == 0)
                    {
                        req.Status = 1;
                        _db.Entry(req).State = EntityState.Modified;
                    }
                }
            }
            _db.SaveChanges();

            //Update TaskID
            var title = _db.Sales_Dispatching_DrawingRequisition_Estimation.Find(RequisitionID);
            title.Version = title.Version + 1;
            title.SubmitID = taskID;
            _db.Entry(title).State = EntityState.Modified;
            _db.SaveChanges();
        }
        
        public SubmitDrawingRequestVm(long taskID)
        {
            //EstRev
            var task = _db.Sales_Dispatching.Find(taskID);
            MyReflection.Copy(task, this);
            //

            ////Estimatior Name
            //EstimatorName = "NA";
            //if (EstimatorID >0)
            //{
            //    var e3 = new FsEmployee(EstimatorID);
            //    EstimatorName = e3.NickName;
            //}

            ////TimeString
            //SubmitTimeString = MyConvert.ConvertNullableDateToString(LastSubmittedTime);
            //EstimationFinishedTimeString = MyConvert.ConvertNullableDateToString(LastFinishedTime);

            //StatusString = GetStatusString(est.Status);


            //         MyAuthorization = new SpecificationAuthorization(employeeID);



        }


    }
    
    

}



