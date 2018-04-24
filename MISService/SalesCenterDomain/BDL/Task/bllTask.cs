using System;
using System.Data;
using MyCommon;
using MyCommon.MyEnum;
using SalesCenterDomain.BDL.Project;
using SalesCenterDomain.BDL.Workorder;
using SalesCenterDomain.BO;
using SalesCenterDomain.SalesCenter;
using SpecDomain.BLL;
using SpecDomain.BLL.EstTitle;

namespace SalesCenterDomain.BDL.Task
{
    /// <summary>
    /// To add a new type of request:
    /// 1.  RequestType GetRequestResponse()
    /// 2.  Reject.FinishStatus
    /// </summary>
    //public class TaskCategory
    //{
    //    public int Estimation;
    //    public int GraphicDrawing;
    //    public int StructureDrawing;
    //    public int ApprovalWorkorder;
    //    //Other Type Request
    //}
    public abstract class TaskSubmit
    {
        //Output

        private readonly int _taskCategory;
        public DispatchingTask ParameterDispatchingTask;

        public TaskSubmit(int taskCategory, int submitBy, string taskFromWhere)
        {
            _taskCategory = taskCategory;
            var dt = new DispatchingTask
                         {
                             TaskCategory = taskCategory,
                             RequestType = taskFromWhere,
                             SubmitBy = submitBy,
                             LeadTime = GetLeadtime(),
                             Status = GetStatus(),
                         };
            ParameterDispatchingTask = dt;
        }

        public int ActionResultCode { get; set; }

        //Input
        //Check existing task request
        public Boolean EnableDuplicateSubmit { private get; set; }

        private int GetStatus()
        {
            var tp = new TaskPropertyNewStatus(_taskCategory);
            return tp.Value;
        }

        private int GetLeadtime()
        {
            var tp = new TaskPropertyResponseLeadtime(_taskCategory);
            return tp.Leadtime;
        }


        public Boolean GetRush()
        {
            bool b = false;
            int hours = MyDateTime.GetDiffHoursOfWeekday(ParameterDispatchingTask.SubmitTime,
                                                         ParameterDispatchingTask.RequiredTime);
            if (hours < ParameterDispatchingTask.LeadTime)
            {
                b = true;
            }
            return b;
        }

        public Boolean IsThisRecordExisting()
        {
            //if existing, udpate
            //esle return False to insert New
            var dtv = new DispatchingTaskValidationDAL(ParameterDispatchingTask);

            bool b = true;
            if (ParameterDispatchingTask.RequestType == "job")
            {
                b = dtv.IsJobRequestExisting();
            }
            else
            {
                b = dtv.IsWorkorderRequestExisting();
            }
            return b;
        }


        public virtual void Insert()
        {
            ParameterDispatchingTask.RefreshResponsible();

            var dti = new DispatchingTaskInsertDAL(ParameterDispatchingTask);

            ActionResultCode = -1;


            if (!IsThisRecordExisting())
            {
                dti.InsertRecord();
                ActionResultCode = 1;
            }
            else
            {
                ActionResultCode = 10010; //existing item
                if (EnableDuplicateSubmit)
                {
                    dti.InsertRecord();
                    ActionResultCode = 1;
                }
            }

            if (ActionResultCode == 1)
            {
                var ps = new ProjectStatus(ParameterDispatchingTask.JobId);
                ps.ChnageTo(ParameterDispatchingTask.TaskCategory, ParameterDispatchingTask.SubmitBy);


                TaskSubmitted();
            }
        }

        public virtual void TaskSubmitted()
        {
        }

        public virtual int GetValidateResult()
        {
            return 0;
        }
    }

    #region Get property according to Category: Response, Leadtime, FinishStatus

    //Get LeadTime according to Type
    public class TaskPropertyResponseLeadtime
    {
        private readonly int _taskCategory;

        public TaskPropertyResponseLeadtime(int taskCategory)
        {
            _taskCategory = taskCategory;
        }

        public int Leadtime
        {
            get { return GetLeadTime(); }
        }

        private int GetLeadTime()
        {
            int value = 0;
            switch (_taskCategory)
            {
                case (int) DispatchingTaskEN.NResponseTaskCategory.ApprovalWorkorder:
                    value = SalesCenterConfiguration.ResponseTimeWorkorderApprove;
                    break;
                case (int) DispatchingTaskEN.NResponseTaskCategory.Estimation:
                    value = SalesCenterConfiguration.ResponseTimeEstimation;
                    ;
                    break;
                case (int) DispatchingTaskEN.NResponseTaskCategory.GraphicDrawing:
                    value = SalesCenterConfiguration.ResponseTimeArtDrawing;
                    break;
                case (int) DispatchingTaskEN.NResponseTaskCategory.StructureDrawing:
                    value = SalesCenterConfiguration.ResponseTimeStructuralDrawing;
                    break;
                default:
                    value = SalesCenterConfiguration.ResponseTimeStructuralDrawing; 
                    break;
            }
            return value;
        }
    }

    public class TaskPropertyNewStatus
    {
        private readonly int _taskCategory;

        public TaskPropertyNewStatus(int taskCategory)
        {
            _taskCategory = taskCategory;
        }

        public int Value
        {
            get { return GetStatusValue(); }
        }

        private int GetStatusValue()
        {
            int value = 0;
            switch (_taskCategory)
            {
                case (int) DispatchingTaskEN.NResponseTaskCategory.ApprovalWorkorder:
                    value = (int) NJobStatus.woPrepared;
                    break;
                case (int) DispatchingTaskEN.NResponseTaskCategory.Estimation:
                    value = (int) NJobStatus.EstimationNew;
                    break;
                case (int) DispatchingTaskEN.NResponseTaskCategory.GraphicDrawing:
                    value = (int) NJobStatus.artDrwNew;
                    break;
                case (int) DispatchingTaskEN.NResponseTaskCategory.StructureDrawing:
                    value = (int) NJobStatus.sDrwNew;
                    break;
                default:
                    break;
            }
            return value;
        }
    }

    #endregion

    #region Submit

    /// <summary>
    /// Work order Approval
    /// </summary>
    public class TaskSubmitWorkorderApprove : TaskSubmit
    {
        public TaskSubmitWorkorderApprove(int taskCategory, int submitBy, string taskFromWhere)
            : base(taskCategory, submitBy, taskFromWhere)
        {
        }


        public override void TaskSubmitted()
        {
            var wo = new WorkorderTitleUpdate(ParameterDispatchingTask.WoId);
            wo.ChangeWorkorderStatus(Convert.ToInt32(NJobStatus.woPrepared));
        }
    }

    //public class TaskSubmitEstimation : TaskSubmit
    //{
    //    public TaskSubmitEstimation(int taskCategory, int submitBy, string taskFromWhere)
    //        : base(taskCategory, submitBy, taskFromWhere)
    //    {
    //    }


    //    public override void TaskSubmitted()
    //    {
    //        var et = new MyEstRev(ParameterDispatchingTask.EstRevId);
    //        et.Submitted(ParameterDispatchingTask.Rush);


    //        //
    //        //var jobID = et.JobID;
    //        //var jobProperty = new ProjectDetails(jobID);

    //        //var leadID = jobProperty.LeadID;
    //        //var lead = new  Lead(leadID);
    //        //lead.UpdateProjectCreatedAt();
    //    }
    //}

    public class TaskSubmitGraphicDrawingRequest : TaskSubmit
    {
        public TaskSubmitGraphicDrawingRequest(int taskCategory, int submitBy, string taskFromWhere)
            : base(taskCategory, submitBy, taskFromWhere)
        {
        }

        public override void TaskSubmitted()
        {
        }
    }

    public class TaskSubmitStructuralDrawingRequest : TaskSubmit
    {
        public TaskSubmitStructuralDrawingRequest(int taskCategory, int submitBy, string taskFromWhere)
            : base(taskCategory, submitBy, taskFromWhere)
        {
        }

        public override void TaskSubmitted()
        {
        }
    }

    public class TaskSubmitInternalTask : TaskSubmit
    {
        public TaskSubmitInternalTask(int taskCategory, int submitBy, string taskFromWhere)
            : base(taskCategory, submitBy, taskFromWhere)
        {
        }
    }


    public class TaskSubmitFactory
    {
        private readonly TaskSubmit _objTaskSubmit;

        public TaskSubmitFactory(int taskCategory, int submitBy, String taskFromWhere)
        {
            _objTaskSubmit = GetObjectTaskSubmitAccordingToTaskCategory(taskCategory, submitBy, taskFromWhere);
        }

        public TaskSubmit ObjTaskSubmit
        {
            get { return _objTaskSubmit; }
        }

        public TaskSubmit GetObjectTaskSubmitAccordingToTaskCategory(int taskCategory, int submitBy,
                                                                     String taskFromWhere)
        {
            TaskSubmit value;
            switch (taskCategory)
            {

                case (int) DispatchingTaskEN.NResponseTaskCategory.GraphicDrawing:
                    value = new TaskSubmitGraphicDrawingRequest(taskCategory, submitBy, taskFromWhere);
                    break;
                case (int) DispatchingTaskEN.NResponseTaskCategory.StructureDrawing:
                    value = new TaskSubmitStructuralDrawingRequest(taskCategory, submitBy, taskFromWhere);
                    break;
                case (int) DispatchingTaskEN.NResponseTaskCategory.ApprovalWorkorder:
                    value = new TaskSubmitWorkorderApprove(taskCategory, submitBy, taskFromWhere);
                    break;
                default:
                    value = new TaskSubmitInternalTask(taskCategory, submitBy, taskFromWhere);

                    break;
            }
            return value;
        }
    }

    #endregion

    #region Response

    //Reject
    //Validation Input
    //Set request to finish
    public class DispatchingTaskRejectBLL
    {
        private readonly DispatchingTask _up = new DispatchingTask();

        public DispatchingTaskRejectBLL(int taskID, int taskCategory)
        {
            _up.TaskId = taskID;
            _up.Status = GetFinishStatus(taskCategory);
            _up.FinishedDate = DateTime.Now;
            _up.InCompleteInfo = true;
        }

        public void Reject(string reason)
        {
            _up.IiReason = reason;
            var dtR = new DispatchingTaskRejectDAL(_up);
            dtR.Reject();
        }

        private static int GetFinishStatus(int taskCategory)
        {
            var dtP = new TaskPropertyFinishStatus((taskCategory));
            return dtP.Value;
        }
    }

    public class DispatchingTaskFinishBLL
    {
        private readonly DispatchingTask _up = new DispatchingTask();

        public DispatchingTaskFinishBLL(int taskID)
        {
            _up.TaskId = taskID;
        }

        public Boolean IsInputValidated { get; set; }
        public int ActionResult { get; private set; }


        public void InputLabourHour(object workedHour, object numberOfDrawing, String note)
        {
            IsInputValidated = GetIsValidated(workedHour, numberOfDrawing, note);

            if (IsInputValidated)
            {
                var dtF = new DispatchingTaskFinishDAL(_up);
                dtF.InputLabourHour();
                ActionResult = 1;
            }
            else
            {
                ActionResult = 10020;
            }
        }

        public void FinishNormal(DateTime finishedDate)
        {
            //tbd:
            _up.FinishedDate = finishedDate;
            var dtF = new DispatchingTaskFinishDAL(_up);
            dtF.UpdateFinishTime();
        }

        private Boolean GetIsValidated(object workedHour, object numberOfDrawing, String note)
        {
            bool b = true;
            if (MyConvert.IsNumeric(workedHour))
            {
                _up.WorkedHour = Convert.ToDouble(workedHour);
            }
            else
            {
                b = false;
            }


            if (MyConvert.IsNumeric(numberOfDrawing))
            {
                _up.NumberOfDrawing = Convert.ToInt32(numberOfDrawing);
            }
            else
            {
                b = false;
            }

            _up.Note = note;


            return b;
        }
    }


    public class TaskPropertyFinishStatus
    {
        private readonly int _taskCategory;

        public TaskPropertyFinishStatus(int taskCategory)
        {
            _taskCategory = taskCategory;
        }

        public int Value
        {
            get { return GetStatusValue(); }
        }

        private int GetStatusValue()
        {
            int value = 0;
            switch (_taskCategory)
            {
                case (int) DispatchingTaskEN.NResponseTaskCategory.ApprovalWorkorder:
                    value = (int) NJobStatus.woApproved;
                    break;
                case (int) DispatchingTaskEN.NResponseTaskCategory.Estimation:
                    value = (int) NJobStatus.EstimationFinish;
                    break;
                case (int) DispatchingTaskEN.NResponseTaskCategory.GraphicDrawing:
                    value = (int) NJobStatus.artDrwFinish;
                    break;
                case (int) DispatchingTaskEN.NResponseTaskCategory.StructureDrawing:
                    value = (int) NJobStatus.sDrwFinish;
                    break;
                default:
                    break;
            }
            return value;
        }

        public bool IsFinished(int statusID)
        {
            var finishedStatusID=GetStatusValue();
            return finishedStatusID == statusID;
        }
    }

    #endregion

    #region Selection

    public class DispatchingTaskSelectionBLL : DispatchingTask
    {
        public DispatchingTaskSelectionBLL(int taskID)
        {
            TaskId = taskID;
            var dsd = new DispatchingTaskSelectionDAL(taskID);
            InitializationProperties(dsd.Row);
        }

        private void InitializationProperties(DataRow row)
        {
            if (row == null) return;
            JobId = MyConvert.ConvertToInteger(row["jobID"]);

            WorkedHour = MyConvert.ConvertToDouble(row["workedHour"]);
            FinishedDate = MyConvert.ConvertToDate(row["FinishedDate"]);
            NumberOfDrawing = MyConvert.ConvertToInteger(row["NumberOfDrawing"]);
            Note = MyConvert.ConvertToString(row["Note"]);
        }
    }

    #endregion
}