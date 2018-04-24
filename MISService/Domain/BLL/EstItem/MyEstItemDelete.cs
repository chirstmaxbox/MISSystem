
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using MyCommon;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.EstItem
{
    public class MyEstItemDelete
    {
        public EST_Item Value { get; set; }
        private readonly long _estItemID;
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public MyEstItemDelete(long estItemID)
        {
            _estItemID = estItemID;
            Value = _db.EST_Item.Find(_estItemID);
        }


                public void Delete()
                {
                    DeleteDocument();
                    DeleteDrawingRequisitionItem();
                    DeleteSize();
                    DeleteSpecialFields();

                    DeleteCostSummary();
                    DeleteCost();

                    DeleteCrEstimationRequisitionItem();

                    DeleteBase();
                }


                private void DeleteDrawingRequisitionItem()
                {
                    try
                    {
                        var objDeletes =
                            _db.Sales_Dispatching_DrawingRequisition_EstimationItem.Where(x => x.EstItemID == _estItemID)
                                .ToList();
                        foreach (var obj in objDeletes)
                        {
                            _db.Entry(obj).State = EntityState.Deleted;
                        }
                        _db.SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        throw;
                    }

                }



        public void DeleteDynamaticSubTablesForChangeName()
        {
            DeleteSize();
            DeleteSpecialFields();
            DeleteCrEstimationRequisitionItem();
        }

        private void DeleteDocument()
        {
            try
            {
                var objDeletes = _db.EST_Item_Drawing.Where(x => x.ParentID == _estItemID).ToList();
                foreach (var obj in objDeletes)
                {
                    _db.Entry(obj).State = EntityState.Deleted;
                }
                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw;
            }

        }

        private void DeleteCostSummary()
        {
            try
            {
                var objDeletes = _db.CR_Cost_Summary.Where(x => x.EstItemID == _estItemID).ToList();
                foreach (var obj in objDeletes)
                {
                    _db.Entry(obj).State = EntityState.Deleted;
                }
                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw;
            }

        }

        private void DeleteCost()
        {
            try
            {
                var objDeletes = _db.EST_Cost.Where(x => x.EstItemID == _estItemID).ToList();
                foreach (var obj in objDeletes)
                {
                    _db.Entry(obj).State = EntityState.Deleted;
                }
                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
             //   Result = dbEx.Message;
                throw;
            }

        }

        private void DeleteCrEstimationRequisitionItem()
        {
            try
            {
                var objDeletes = _db.CR_EstimationRequisitionItem.Where(x => x.EstItemID == _estItemID).ToList();
                foreach (var obj in objDeletes)
                {
                    _db.Entry(obj).State = EntityState.Deleted;
                }
                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
               // Result = dbEx.Message;
                throw;
            }

        }

        private void DeleteSize()
        {
            try
            {
                var objDeletes = _db.EST_Item_Specification_Size.Where(x => x.EstItemID == _estItemID).ToList();
                foreach (var obj in objDeletes)
                {
                    _db.Entry(obj).State = EntityState.Deleted;
                }
                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
             //   Result = dbEx.Message;
                throw;
            }
        }

        private void DeleteSpecialFields()
        {
            try
            {
                var objDeletes = _db.EST_Item_Specification.Where(x => x.EstItemID == _estItemID).ToList();
                foreach (var obj in objDeletes)
                {
                    _db.Entry(obj).State = EntityState.Deleted;
                }
                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
              //  Result = dbEx.Message;
                throw;
            }

        }

        private void DeleteBase()
        {
            try
            {
                var obj = _db.EST_Item.Find(_estItemID);
                _db.Entry(obj).State = EntityState.Deleted;

                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
                throw;
            }
        }

    }

 
    public class MyEstItemDeleteValidationSales : IValidation
    {
        public bool IsValidated { get; set; }
        public string ErrorMessage { get; set; }

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        private readonly EST_Item _value;

        public MyEstItemDeleteValidationSales(long estItemID)
        {
            _value = _db.EST_Item.Find(estItemID);
            SetIsDeletable();
        }      

        //Input: (EST_Item) Value
        private void SetIsDeletable()
        {
            IsValidated = true;
            if (_value.EstItemID == 0)
            {
                ErrorMessage = "Request Failed, Can not find the Specified Item ";
                IsValidated = false;
            }

            //Not In Quotation and Not in Workorder and Not in 


            //Sales: Can be deleted when status=New and Purpose=Request
            if (_value .StatusID !=(int)NEstItemStatus.New && _value .StatusID !=(int)NEstItemStatus.ContentsChanged)
            {
                IsValidated = false;
                ErrorMessage += "Request Failed, This item has been submitted";
            }
        }
    }

    public class MyEstItemDeleteValidationEstimator : IValidation
    {
        public bool IsValidated { get; set; }
        public string ErrorMessage { get; set; }

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        private readonly EST_Item _value;

        public MyEstItemDeleteValidationEstimator(long estItemID)
        {
            _value = _db.EST_Item.Find(estItemID);
            SetIsDeletable();
        }

        //Input: (EST_Item) Value
        private void SetIsDeletable()
        {
            IsValidated = true;
            if (_value.EstItemID == 0)
            {
                ErrorMessage = "Delete Failed, Can not find The Specified Item ";
                IsValidated = false;
            }

            //Not In Quotation and Not in Workorder and Not in 
            //Sales: Can be deleted when status=New and Purpose=Request

            if (_value.StatusID == (int)NEstItemStatus.Estimated)
            {
                IsValidated = false;
                ErrorMessage += "Delete Failed,  This item has been Finsheed";
            }
        }

    }



    //public class MyEstItemChangeNameValidationSales : IValidation
    //{
    //    public bool IsValidated { get; set; }
    //    public string ErrorMessage { get; set; }

    //    private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
    //    private readonly EST_Item _value;

    //    public MyEstItemChangeNameValidationSales(long estItemID)
    //    {
    //        _value = _db.EST_Item.Find(estItemID);
    //        SetIsDeletable();
    //    }

    //    //Input: (EST_Item) Value
    //    private void SetIsDeletable()
    //    {
    //        IsValidated = true;
    //        if (_value.EstItemID == 0)
    //        {
    //            ErrorMessage = "Request Failed, Can not find The Specified Item ";
    //            IsValidated = false;
    //        }

    //        //Not In Quotation and Not in Workorder and Not in 


    //        //Sales: Can be deleted when status=New and Purpose=Request
    //        if (_value.StatusID != (int)NEstItemStatus.New)
    //        {
    //            IsValidated = false;
    //            ErrorMessage += "Request Failed, This item has been submitted";
    //        }
    //    }
    //}

 
}