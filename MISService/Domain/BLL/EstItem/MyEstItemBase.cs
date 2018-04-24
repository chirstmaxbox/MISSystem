using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyCommon;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.EstItem
{
    public class MyEstItemBase : EST_Item
    {
        public string IsPreviousEstimationAvailableText { get; set; }
        public string EstItemNoText { get; set; }
        public string SerialNumberText { get; set; }
        public string QtyText { get; set; }
        public string Requirement { get; set; }
        public string Position { get; set; }
        public string Status { get; set; }

        public string ItemPurposeText { get; set; }
        public string ItemStatusText { get; set; }
        public bool IsChecked { get; set; }
        public string  CheckBoxID { get; set; }

        public MyEstItemBase()
        {
        }
        public MyEstItemBase(EST_Item estItem)
        {
            MyReflection.Copy(estItem, this);
            
            EstItemNoText = estItem.EstItemNo.ToString("D2");

            SerialNumberText  = "SN" + estItem.EstItemID.ToString("");
            
            QtyText = estItem.Qty.ToString("D0") + " Set(s)";
            Requirement = estItem.FW_JOB_TYPE.JOB_TYPE;
            Position = estItem.RequiredItemPosition.Name;
            Status = estItem.EST_Item_Status.Name;
            ItemPurposeText = estItem.EST_Item_TablePurpose.Name;
            ItemStatusText = estItem.EST_Item_Status.Name;
            IsPreviousEstimationAvailableText = "";
            if (IsPreviousEstimationAvailable ==(int) NYesNoNotApplicable.Yes )
            {
                IsPreviousEstimationAvailableText = "Yes";
            }

            if (IsPreviousEstimationAvailable ==(int) NYesNoNotApplicable.No )
            {
                IsPreviousEstimationAvailableText = "No";
            }
            
            if (PriceA ==null)
            {
                PriceA = 0;
            }

            if (PriceB == null)
            {
                PriceB = 0;
            }

            if (PriceExtra == null)
            {
                PriceExtra = 0;
            }

            CheckBoxID = "mycbxid" + estItem.EstItemID.ToString("");

        }
    }

    
}