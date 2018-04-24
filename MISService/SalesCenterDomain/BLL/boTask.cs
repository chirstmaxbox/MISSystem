using System;
using MyCommon.MyEnum;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.SalesCenter
{
    public class DispatchingTask
    {
        //Insert is automatic, can be inilization as 0;
        public DispatchingTask()
        {
            RequestType = "job";
            Subject = "Concept Drawing";
            Description = "";

            SubmitTime = DateTime.Now;
            LastUpdateTime = DateTime.Now;
            RequiredTime = DateTime.Now;
            Rush = false;
            InCompleteInfo = false;
            WorkedHour = 0;
            FinishedDate = DateTime.Now;
            NumberOfDrawing = 0;
            Note = "";
            IiReason = "";
            WorkorderType = (int) NWorkorderType.Production;
        }

        public int TaskId { get; set; }
        //Response is decided by TaskType as submit by
        //Leadtime is decided by taskType
        //Task Categroy
        public int TaskCategory { get; set; }

        //inserted fields
        public int JobId { get; set; }
        public int EstRevId { get; set; }
        public int WoId { get; set; }

        public int SubmitBy { get; set; }
        public int Responsible { get; set; }
        public int LeadTime { get; set; }
        //
        public int Status { get; set; }
        //job or wip, when submit in estimation it's job; when submit in workorder, it's wip
        public String RequestType { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        // = System.DateTime.Now;
        public DateTime SubmitTime { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public DateTime RequiredTime { get; set; }
        //Calcualte
        public Boolean Rush { get; set; }

        //To Be Upated, should not be, maybe 
        public Boolean InCompleteInfo { get; set; }
        public double WorkedHour { get; set; }
        public DateTime FinishedDate { get; set; }
        public int NumberOfDrawing { get; set; }
        public string Note { get; set; }
        public string IiReason { get; set; }

        public int WorkorderType { get; set; } //Sales_JobMasterList.WO.woType

        public void RefreshResponsible()
        {
            Responsible = GetRequestResponsibleFromDefaultConfiguration();
        }

        ////Get Responsible
        //public class TaskPropertyResponsible
        //{
        //    private int _responsible;
        //    public int Responsible
        //    {
        //        get { return _responsible; }
        //    }

        //    private int _workorderType;

        //    public TaskPropertyResponsible(int taskCategory, int submitBy, int workorderType)
        //    {
        //        _workorderType = workorderType;
        //        //Read datatable
        //        //     var team = new FsEmployeeTeam(0, submitBy);
        //        //    DataRow row = team.Datarow;
        //        //if can not find definition in db, then use default configuration
        //        //    _responsible = row != null ? GetRequestResponsibleFromDataBase(taskCategory, row) : GetRequestResponsibleFromDefaultConfiguration(taskCategory);
        //        _responsible = GetRequestResponsibleFromDefaultConfiguration(taskCategory);
        //    }

        private int GetRequestResponsibleFromDefaultConfiguration()
        {
            int value = 0;

            switch (TaskCategory)
            {
                case (int) DispatchingTaskEN.NResponseTaskCategory.Estimation:
                    value = SalesCenterConfiguration.ESTIMATION_EID;
                    break;
                case (int) DispatchingTaskEN.NResponseTaskCategory.GraphicDrawing:
                    value = SalesCenterConfiguration.GRAPHIC_DRAWING_EID;
                    break;
                case (int) DispatchingTaskEN.NResponseTaskCategory.StructureDrawing:
                    value = SalesCenterConfiguration.STRUCTURAL_DRAWING_EID;
                    break;
                case (int) DispatchingTaskEN.NResponseTaskCategory.ApprovalWorkorder:
                    //SiteCheck Workorder
                    //Service Workorder

                    value = SalesCenterConfiguration.WORKORDER_APPROVE_Production_EID;
                    if (WorkorderType == (int) NWorkorderType.Service)
                    {
                        value = SalesCenterConfiguration.WORKORDER_APPROVE_Service_EID;
                    }

                    if (WorkorderType == (int) NWorkorderType.Sitecheck)
                    {
                        value = SalesCenterConfiguration.WORKORDER_APPROVE_Sitecheck_EID;
                    }
                    break;
                default:
                    break;
            }
            return value;
        }
    }
}