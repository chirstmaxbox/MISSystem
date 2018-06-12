using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CustomerDomain.BLL;
using MyCommon;
using SpecDomain.BLL.EstItem;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.Task
{
    public class DrawingRequisitionFormPreValidation
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
      
        public string ValidationMessage { get; set; }
        public int InstallToCustomerID { get; set; }
        public int MallID { get; set; }
        public List<EST_Item> EstItems { get; set; }

        private readonly int _projectID;
        private readonly int _estRevID;
        
        public DrawingRequisitionFormPreValidation(int projectID, int estRevID)
        {
            _projectID = projectID;
            _estRevID = estRevID;

            SetEstItems();

            SetInstallToCustomerID();
            SetMallID(InstallToCustomerID);

        }
        
        public bool GetIsValid()
        {
            var b = true;
            if (InstallToCustomerID <100)
            {
                b = false;
                ValidationMessage = "Request Failed, Install To Customer Required." + Environment .NewLine ;
            }

            if (EstItems ==null )
            {
                b = false;
                ValidationMessage += "Rquest Failed, At least one Item Required" + Environment.NewLine;
            } 
            else if (!EstItems.Any())
            {
                b = false;
                ValidationMessage += "Rquest Failed, At least one Item Required" + Environment.NewLine;
            }

            return b;
        }

       // ViewBag.Error =
        private void SetEstItems()
        {
            EstItems  = _db.EST_Item.Where(x => x.EstRevID == _estRevID &&
                                                x.ItemPurposeID == (short)NEstItemPurpose.ForEstimation 

                ).ToList();
        }

        private void SetInstallToCustomerID()
        {
             var mjc = new MySalesJobMasterListCustomer(_projectID );

            // Install To
            mjc.SetInstallTo();
            InstallToCustomerID = mjc.CustomerID;
        }

        private void SetMallID(int customerID)
        {
            if (customerID < 100)
            {
                MallID = 0;
                return;
            }

            var cm = new CustomerMall(customerID);
            MallID=cm.MallID;
        }
    }

    //public class DrawingRequisitionSubmitValidation:MyValidationError 
    //{
    //    private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        
    //    private readonly int _requisitionID;

    //    public DrawingRequisitionSubmitValidation(int requisitionID)
    //    {
    //        _projectID = projectID;
    //        _estRevID = estRevID;

    //        SetEstItems();

    //        SetInstallToCustomerID();
    //        SetMallID(InstallToCustomerID);

    //    }

    //    public bool GetIsValid()
    //    {
    //        var b = true;
    //        if (InstallToCustomerID < 100)
    //        {
    //            b = false;
    //            ValidationMessage = "Request Failed, Install To Customer Required." + Environment.NewLine;
    //        }

    //        if (EstItems == null)
    //        {
    //            b = false;
    //            ValidationMessage += "Rquest Failed, At least one Item Required" + Environment.NewLine;
    //        }
    //        else if (!EstItems.Any())
    //        {
    //            b = false;
    //            ValidationMessage += "Rquest Failed, At least one Item Required" + Environment.NewLine;
    //        }

    //        return b;
    //    }

    //    // ViewBag.Error =
    //    private void SetEstItems()
    //    {
    //        EstItems = _db.EST_Item.Where(x => x.EstRevID == _estRevID &&
    //                                            x.ItemPurposeID == (short)NEstItemPurpose.ForEstimation

    //            ).ToList();
    //    }

    //    private void SetInstallToCustomerID()
    //    {
    //        var mjc = new MySalesJobMasterListCustomer(_projectID);

    //        // Install To
    //        mjc.SetInstallTo();
    //        InstallToCustomerID = mjc.CustomerID;
    //    }

    //    private void SetMallID(int customerID)
    //    {
    //        if (customerID < 100)
    //        {
    //            MallID = 0;
    //            return;
    //        }

    //        var cm = new CustomerMall(customerID);
    //        MallID = cm.MallID;
    //    }
   
    //}

    public class DrawingRequisitionItemVm
    {
        //Input
        public int RequisitionID { get; set; }
        public bool IsThereActiveItems { get; set; }

        //EST_Items that can be add to the requistion
        public List<MyLongKeyValueBool> AvailableEstItems { get; set; }

        //Existing Items, can be added from AvailableEstItems 
        public List<Sales_Dispatching_DrawingRequisition_EstimationItem> DrawingRequisitionItems { get; private set; }

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        
        public  void Refresh(int estRevID)
        {
            //Drawing Items
           DrawingRequisitionItems = _db.Sales_Dispatching_DrawingRequisition_EstimationItem.Where(x => x.RequisitionID == RequisitionID).ToList();
           if (DrawingRequisitionItems.Any())
           {
               foreach (var drwItem in DrawingRequisitionItems)
               {
                   drwItem.ItemName = EstItemCommon.GetFullItemName(drwItem.EST_Item);
               }                 
           }

            IsThereActiveItems = DrawingRequisitionItems.Where(x => x.IsIncludedWhenPrint).ToList().Any();

          var estItems = _db.EST_Item.Where(x => x.EstRevID == estRevID &&
                                             x.ItemPurposeID == (short)NEstItemPurpose.ForEstimation &&
                                             !x.Sales_Dispatching_DrawingRequisition_EstimationItem.Any()
                                             ).ToList();

          AvailableEstItems = new List<MyLongKeyValueBool>();
            if(!estItems.Any( )) return;
     
            foreach (var estItem in estItems)
            {
                var sli = new MyLongKeyValueBool
                              {
                               Key=estItem.EstItemID,
                               Value1 = EstItemCommon.GetFullItemName(estItem),
                               Value2 =estItem.Description ,
                               IsChecked =true,
                              };
                AvailableEstItems.Add(sli);
            }
        }


        public void CreateRequisitionItems()
        {
            foreach (var lkv in AvailableEstItems)
            {
                var estItem = _db.EST_Item.Find(lkv.Key);
                var reqItem = GetNewSalesDispatchingDrawingRequisitionEstimationItem(estItem, RequisitionID, lkv.IsChecked);
                _db.Sales_Dispatching_DrawingRequisition_EstimationItem.Add(reqItem);
                _db.SaveChanges();
                lkv.Value2 = reqItem.RequisitionItemID.ToString();
            }

            //_db.SaveChanges();
        }

        private Sales_Dispatching_DrawingRequisition_EstimationItem GetNewSalesDispatchingDrawingRequisitionEstimationItem(EST_Item item, int requisitionID, bool isIncludedWhenPrint = true)
        {
            var reqItem = new Sales_Dispatching_DrawingRequisition_EstimationItem()
            {
                //  public int RequisitionItemID { get; set; }
                RequisitionID = requisitionID,
                EstItemID = item.EstItemID,
                Status = (int)NEstItemStatus.New,
                Qty = item.Qty.ToString(""),
                PowerVoltage = GetPowerVoltage(item),
                Description = item.Description,
                IsIncludedWhenPrint = isIncludedWhenPrint,
                //IsIncludedWhenPrint =true ,
            };
            return reqItem;
        }

        private static string GetPowerVoltage(EST_Item item)
        {
            if (item.IsTemplateApplicable) return "NA";
            if (!item.EST_Item_Specification.Any()) return "NA";

            foreach (var spec in item.EST_Item_Specification)
            {
                if (string.IsNullOrEmpty(spec.Title)) continue;
                if (spec.Title.ToUpper().Trim() == "VOLTAGE" | spec.Title.ToUpper().Trim() == "POWER VOLTAGE")
                {
                    return spec.Contents;
                }
            }
            return "NA";
        }
    }

    public class DrawingRequisitionFormVm
    {
        //Title
        public Sales_Dispatching_DrawingRequisition_Estimation DrawingRequisitionTitle { get; set; }
        public IEnumerable<SelectListItem> Questions { get; set; }
        public IEnumerable<SelectListItem> DrawingPurposes { get; set; }
        public IEnumerable<SelectListItem> Targets { get; set; }
        
        public DrawingRequisitionItemVm RequisitionItemVm { get; set; }

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        private DrawingRequisitionFormPreValidation _validation; 
        private readonly int _projectID;
        private readonly int _estRevID;
        //public int RequisitionID { get; private set; }

         public DrawingRequisitionFormVm(int projectID, int estRevID)
         {
             _projectID = projectID;
             _estRevID = estRevID;

         }

#region ************************** First Time, Run Once**********************
         public void Initialization()
         {
             DrawingRequisitionTitle =_db.Sales_Dispatching_DrawingRequisition_Estimation.FirstOrDefault(x => x.EstRevID == _estRevID);
             _validation = new DrawingRequisitionFormPreValidation(_projectID, _estRevID);

             if (DrawingRequisitionTitle == null)
             {
                 DrawingRequisitionTitle = CreateNewTitle();
             }
             else
             {
                 if (DrawingRequisitionTitle.InstalltoCustomerID != _validation.InstallToCustomerID |
                     DrawingRequisitionTitle.MallID != _validation.MallID)
                 {
                     DrawingRequisitionTitle.InstalltoCustomerID = _validation.InstallToCustomerID;
                     DrawingRequisitionTitle.MallID = _validation.MallID;

                     _db.Entry(DrawingRequisitionTitle).State = EntityState.Modified;
                     _db.SaveChanges();
                 }
             }

        }

        private Sales_Dispatching_DrawingRequisition_Estimation CreateNewTitle()
        {
            var drw = new Sales_Dispatching_DrawingRequisition_Estimation()
                          {
                              //public int RequisitionID { get, set, }
                              DrawingType = "Graphic",
                              ProjectID = _projectID,
                              EstRevID = _estRevID,
                              InstalltoCustomerID =_validation .InstallToCustomerID,
                              MallID = _validation .MallID,
                              SubmitID = 0,
                              Version = 0,
                              DrawingPurpose =0,
                              IsQuotationAvailable = 0,
                              IsGCorDesignerDrawingAvailable = 0,
                              IsSiteCheckPhotoAvailable = 0,
                              IsSiteCheckReportAvailable = 0,
                              IsLandordOrMallCriteriaAvailable = 0,
                              IsElectronicFillFromClientAvailable = 0,

                          };

            _db.Sales_Dispatching_DrawingRequisition_Estimation.Add(drw);
            _db.SaveChanges();
            return drw;
        }


    
       
#endregion


#region **************************** View ******************************


        private void InitializeYesNoList()
        {
            var s0 = new SelectListItem()
                         {
                             Value = "0",
                             Text = ""
                         };
            var s1 = new SelectListItem()
                         {
                             Value = ((int) NYesNoNotApplicable.Yes).ToString(""),
                             Text = "Yes"
                         };
            var s2 = new SelectListItem()
                         {
                             Value = ((int) NYesNoNotApplicable.No).ToString(""),
                             Text = "No"
                         };

            Questions = new List<SelectListItem>() {s0, s1, s2};

            var s3 = new SelectListItem()
            {
                Value = "0",
                Text = "Graphic"
            };
            var s4 = new SelectListItem()
            {
                Value = "1",
                Text = "Structure"
            };

            Targets = new List<SelectListItem>() { s3, s4 };


        }

        private void InitializeDropdownlist()
        {

            var temp02 = _db.Sales_Dispatching_DrawingReq_Purpose.Where(x => x.step == "1" || x.RowID == 0).ToList();
            DrawingPurposes = temp02.Select(x => new SelectListItem
                                                     {
                                                         Value = Convert.ToString(x.RowID),
                                                         Text = x.dpName
                                                     }
                );
        }


        public void Refresh()
        {
            InitializeYesNoList();
            InitializeDropdownlist();

            RequisitionItemVm = new DrawingRequisitionItemVm();
            RequisitionItemVm.RequisitionID = DrawingRequisitionTitle.RequisitionID;
            RequisitionItemVm.Refresh(_estRevID);
        }
#endregion

#region  ************************** Postback, Update ***************************************
        public DrawingRequisitionFormVm()
        {
        }

        public void Save()
        {
            DrawingRequisitionTitle.IsValid = GetIsValid();
            _db.Entry(DrawingRequisitionTitle).State = EntityState.Modified;
            _db.SaveChanges();
        }

        private bool GetIsValid()
        {
            //if (DrawingRequisitionTitle.InstalltoCustomerID == 0) return false;
            //if (DrawingRequisitionTitle.BillToCustomerID == 0) return false;
            if (DrawingRequisitionTitle.DrawingPurpose == 0) return false;
            if (DrawingRequisitionTitle.IsQuotationAvailable == 0) return false;
            if (DrawingRequisitionTitle.IsGCorDesignerDrawingAvailable == 0) return false;
            if (DrawingRequisitionTitle.IsSiteCheckPhotoAvailable == 0) return false;
            if (DrawingRequisitionTitle.IsSiteCheckReportAvailable == 0) return false;
            if (DrawingRequisitionTitle.IsLandordOrMallCriteriaAvailable == 0) return false;
            if (DrawingRequisitionTitle.IsElectronicFillFromClientAvailable == 0) return false;
            return true;
        }

#endregion


    }

}
