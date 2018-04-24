using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using ProjectDomain;

namespace SalesCenterDomain.BLL
{
    public class MySalesSpecialMaterialRequest
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private readonly int _jobID;

        public MySalesSpecialMaterialRequest(int jobID)
        {
            _jobID = jobID;
            Value = _db.Sales_SpecialMaterialRequest.FirstOrDefault(x => x.JobID == _jobID);
        }

        public Sales_SpecialMaterialRequest Value { get; set; }

        public void Submit()
        {
            Value.Status = 603;
            Value.SubmitTime = DateTime.Now;

            _db.Entry(Value).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void ClearSubmit()
        {
            List<Sales_Dispatching> requests =
                _db.Sales_Dispatching.Where(x => x.JobID == _jobID & x.Subject == "Special Material").ToList();
            if (requests.Count == 0)
            {
                Value.Status = 601;
                Value.SubmitTime = DateTime.Now;

                _db.Entry(Value).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }

        public void SubmitToPurchasing(int taskType, bool isSubmitted)
        {
            switch (taskType)
            {
                case 201:
                    Value.Estimation = isSubmitted;
                    Value.EstimationSubmitDate = null;
                    if (isSubmitted)
                    {
                        Value.EstimationSubmitDate = DateTime.Now;
                    }
                    break;
                case 501:
                    Value.GraphicDrawing = isSubmitted;
                    Value.GraphicDrawingSubmitDate = null;
                    if (isSubmitted)
                    {
                        Value.GraphicDrawingSubmitDate = DateTime.Now;
                    }

                    break;
                case 551:
                    Value.StructuralDrawing = isSubmitted;
                    Value.StructuralDrawingSubmitDate = null;
                    if (isSubmitted)
                    {
                        Value.StructuralDrawingSubmitDate = DateTime.Now;
                    }
                    break;
                default:
                    break;
            }

            _db.Entry(Value).State = EntityState.Modified;

            _db.SaveChanges();
        }
    }
}