using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyCommon;
using SpecDomain.BLL.EstTitle;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.EstItem
{

    public class MyEstItem
    {
        public MyEstItemBase Value { get; set; }
        public ParameterSpecification  MyAuthorization { get; set; }

        public List<EST_Item_Specification_Size> ItemSizes { get; set; }
        public List<EST_Item_Specification> SpecialFields { get; set; }
        public List<EST_Item_Drawing> Drawings { get; set; }

        public bool IsValidated { get; set; }
        public string ValidatedResult { get; set; }
        public bool IsTemplateEnabled { get; set; }
        public bool IsBiddingJob { get; set; }

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        
        public MyEstItem(long estItemID)
        {
            var estItem = _db.EST_Item.Find(estItemID);
            Value  = new MyEstItemBase(estItem );
            //Specification
            SpecialFields = _db.EST_Item_Specification.Where(x => x.EstItemID == estItemID).OrderBy(x => x.OrderNumber).ToList();
            IsTemplateEnabled = SpecialFields.Any();

            ItemSizes = _db.EST_Item_Specification_Size.Where(x => x.EstItemID ==estItemID).OrderBy(x => x.EstItemSizeID).ToList();
            
            //Drawing
            Drawings = _db.EST_Item_Drawing.Where(x => x.ParentID == estItemID).ToList();
            IsBiddingJob = GetIsBiddingJob(Value.EstRevID);

        }

        private bool GetIsBiddingJob(int estRevID)
        {
            //Is Bidding Job
            var estRev = _db.Sales_JobMasterList_EstRev.Find(estRevID);
            var job = new SpecProjectDetail(estRev.JobID);
            return job.IsBidTo;
        }

        #region ************************ EDIT ********************************
        //Base Dropdownlists
        public IEnumerable<SelectListItem> ItemRequirements { get; set; }
        public IEnumerable<SelectListItem> ItemPositions { get; set; }
        public IEnumerable<SelectListItem> PreviousEstimationAvailables { get; set; }
        public IEnumerable<SelectListItem> Status { get; set; }



        
        public void RefreshDropdownlist()
        {
           var positions = _db.RequiredItemPositions.OrderBy(x => x.Name).ToList();
            ItemPositions = positions.Select(x => new SelectListItem
                                                      {
                                                          Value = Convert.ToString(x.PositionID),
                                                          Text = x.Name
                                                      });

            var requirements = _db.FW_JOB_TYPE.Where(x => x.TYPE_ID >= 0).OrderBy(x => x.JOB_TYPE).ToList();
            ItemRequirements = requirements.Select(x => new SelectListItem
                                                            {
                                                                Value = Convert.ToString(x.TYPE_ID),
                                                                Text = x.JOB_TYPE
                                                            });

            var s0 = new SelectListItem()
                         {
                             Value = "0",
                             Text = ""
                         };
            var s1 = new SelectListItem()
            {
                Value = ((int) NYesNoNotApplicable.Yes).ToString("") ,
                Text = "Yes"
            };
            var s2 = new SelectListItem()
            {
                Value = ((int)NYesNoNotApplicable.No).ToString(""),
                Text = "No"
            };

            PreviousEstimationAvailables = new List<SelectListItem>() { s0, s1, s2 };

            var status = _db.EST_Item_Status.OrderBy(x => x.Name).ToList();
            Status = status.Select(x => new SelectListItem
            {
                Value = Convert.ToString(x.StatusID),
                Text = x.Name
            });

        }

        public MyEstItem()
        {
        }

        public void UpdateBase()
        {
            try
            {
                //MyReflection.Copy(Value, newValue );
                var item = _db.EST_Item.Find(Value.EstItemID);
                item.EstItemID = Value.EstItemID;
                item.IsPreviousEstimationAvailable = Value.IsPreviousEstimationAvailable;

                item.EstItemNo = Value.EstItemNo;
                item.EstPart = Value.EstPart;
                item.ItemOption = Value.ItemOption;
                item.IsFinalOption = Value.IsFinalOption;
                item.PositionID = Value.PositionID;
                item.RequirementID = Value.RequirementID;
                item.BySubcontractor = Value.BySubcontractor;
                item.Qty = Value.Qty;
                
                //              $(" #SpecialMaterialLeadTime, #SpecialMaterial, #IsThereSpecialMaterial").attr('disabled', true);
                if (MyAuthorization.IsResponseOwner)
                {
                    item.Description = Value.Description;
                    item.Remark = Value.Remark;
                    item.IsThereSpecialMaterial = Value.IsThereSpecialMaterial;
                    item.SpecialMaterial = Value.SpecialMaterial;
                    item.SpecialMaterialLeadTime = Value.SpecialMaterialLeadTime;               
                }
               
                if (MyAuthorization.IsRequestOwner )
                {
                    item.SalesDescription = Value.SalesDescription;      
                }

                if (IsBiddingJob)
                {
                    item.BidSignIdCode = Value.BidSignIdCode;
                    item.BidReferenceDrawing = Value.BidReferenceDrawing;
                    item.BidDescription = Value.BidDescription;
                    item.BidRemark = Value.BidRemark;
                }


                item.IsTemplateApplicable = Value.IsTemplateApplicable;
                item.StatusID = Value.StatusID;
                
                _db.Entry(item).State = EntityState.Modified;
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                throw;
            }
        }

        public void UpdateSpecialFields(List<MyKeyValuePair> keyValues)
        {
            SpecialFields = _db.EST_Item_Specification.Where(x => x.EstItemID == Value.EstItemID).OrderBy(x => x.EstItemSpecificationID).ToList();
            foreach (var field in SpecialFields)
            {
                var optionGroupID = field.OptionGroupID;
                var id = field.EstItemSpecificationID;
                var kv = keyValues.First(x => x.Key == id);
                //Contents
                field.Contents = kv.Value;
                //OptionID
                var options = _db.EST_Item_Specification_Template_Option.Where(x => x.OptionGroupID == optionGroupID);
                var option = options.FirstOrDefault(x => x.OptionName.Trim().ToUpper() == kv.Value.Trim().ToUpper());
                field.OptionID = 0;
                if (option !=null )
                {
                    field.OptionID = option.OptionID;
                }
                
                //IsValidated & ErrorMessage if Ismandatory
                field.IsValidated = true;
                field.ErrorMessage = "";

                if (field.IsMandatory & MyConvert.IsNullString(kv.Value))
                {
                       field.IsValidated = false;
                       field.ErrorMessage =field.Title + " Required.";
                }

                _db.Entry(field).State = EntityState.Modified;
            }
            _db.SaveChanges();
        }
        
        public void UpdateSizes(List<EST_Item_Specification_Size> sizes)
        {
            foreach (var size in sizes)
            {
                var tobeupdatedSize = _db.EST_Item_Specification_Size.Find(size.EstItemSizeID);

                tobeupdatedSize.WidthFeet = size.WidthFeet;
                tobeupdatedSize.WidthInch = size.WidthInch;
                tobeupdatedSize.Width = size.Width;

                tobeupdatedSize.HeightFeet = size.HeightFeet;
                tobeupdatedSize.HeightInch = size.HeightInch;
                tobeupdatedSize.Height = size.Height;

                tobeupdatedSize.ThicknessFeet = size.ThicknessFeet;
                tobeupdatedSize.ThicknessInch  =size.ThicknessInch ;
                tobeupdatedSize.Thickness = size.Thickness;


                tobeupdatedSize.Pc = size.Pc;
                size.IsValidated = true;
                
                _db.Entry(tobeupdatedSize).State = EntityState.Modified;
            }
            _db.SaveChanges();

          
        }

        //private void ValidateSize()
        //{
        //    //Is There Mandatory Row
        //    var estItem = _db.EST_Item.Find(Value.EstItemID);
        //    var mandatorySize = estItem.EST_Item_Specification_Size.FirstOrDefault();
        //    if (mandatorySize == null) return;

        //    //Yes, Save the Validation Result
        //    var vSize = new ValidationEstItemSize(estItem);
        //    mandatorySize.IsValidated = vSize.IsValidated;
        //    mandatorySize.ErrorMessage =vSize.ErrorMessage;
        //    _db.Entry(mandatorySize).State = EntityState.Modified;
        //    _db.SaveChanges();
        //}

        //Invoke after Update
        public void Validation()
        {
            var estItem = _db.EST_Item.Find(Value.EstItemID);
            var vItem = new ValidationEstItem(estItem);
            estItem .IsValidated =vItem .IsValidated;
            estItem.ErrorMessage = vItem.ErrorMessage;
            _db.Entry(estItem).State = EntityState.Modified;
            _db.SaveChanges();
        }
       
        public void UpdateBidField()
        {
            

        }
        
        #endregion


        #region  *********************** Quotation Generation

        public string GetDescription()
        {
            var s = MyConvert.ConvertToString(Value.Description);
            if (Value.IsTemplateApplicable) return s;
            if (!SpecialFields.Any()) return s;
            s += Environment.NewLine;
            foreach (var field in SpecialFields)
            {
                s += Convert.ToString(field.OrderNumber) + ") " + field.Title + ": " + field.Contents + Environment.NewLine;
            }
            return s;
        }

        public string GetDescriptionWithHtmlBreak()
        {
            var s = MyConvert.ConvertToString(Value.Description);
            if (Value.IsTemplateApplicable) return s;
            if (!SpecialFields.Any()) return s;
            s += Environment.NewLine;

            var i = 1;
            foreach (var field in SpecialFields)
            {
                if (!MyConvert .IsNullString( field.Contents) )
                {
                    s += Convert.ToString(i) + ") " + field.Title + ": " + field.Contents + "<br />";
                    i++;
                }
            }
            return s;
        }
        #endregion

    }
}