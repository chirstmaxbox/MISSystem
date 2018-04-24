using System;
using System.Collections.Generic;
using System.Linq;
using MyCommon;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.EstItem
{
    public class ValidationEstItem
    {
        public bool IsValidated { get; private set; }
        public string  ErrorMessage { get; private set; } 
        
        public ValidationEstItem (EST_Item estItem)
        {
            ErrorMessage = "";
            
            //Common Fields
            ErrorMessage += ValidateCommonFields(estItem );

            //Size
            ErrorMessage += ValidateSizes(estItem);
            
            //Special Fields;
            ErrorMessage += ValidateSpecialFields(estItem);
            
            IsValidated = ErrorMessage.Length == 0;
        }

        private string  ValidateCommonFields(EST_Item estItem)
        {
            // Is Previous Estimation Available
            // Qty
            var msg = "";
            if (estItem.IsPreviousEstimationAvailable == (int) NYesNoNotApplicable.NotApplicable)
            {
                msg+="Item "+estItem.EstItemNo.ToString("D0") +": "+ "Is Previous Estimation Avaliable?"+Environment .NewLine ;
            }

            if (estItem.Qty <= 0)
            {
               msg +="Item "+estItem.EstItemNo.ToString("D0") +": "+ "Qty Required"+Environment .NewLine ;
            }

            return msg;
        }

        private string ValidateSizes(EST_Item estItem)
        {
            return "";
            var mandatorySize = estItem.EST_Item_Specification_Size.FirstOrDefault();
            if (mandatorySize == null) return "";
            if (mandatorySize.IsValidated) return "";
            return "Item " + estItem.EstItemNo.ToString("D0") + ": " + mandatorySize.ErrorMessage;
        }

        private string  ValidateSpecialFields(EST_Item estItem)
        {
            var msg = "";
            if (estItem.IsTemplateApplicable) return msg;

            var specs = estItem.EST_Item_Specification.ToList();
            foreach (var spec in specs)
            {
                if (spec.IsMandatory & !spec.IsValidated)
                {
                    msg += "Item " + estItem.EstItemNo.ToString("D0") + ": " + spec.ErrorMessage + Environment.NewLine;
                }
            }
            return msg;
        }
    }

    public class ValidationEstItemSize
    {
        public bool IsValidated { get; private set; }
        public string ErrorMessage { get; private set; }

        public ValidationEstItemSize(EST_Item estItem )
        {
            IsValidated = true;
            ErrorMessage = "";
            if (estItem.SizeRows == 0) return;

            var itemSizes = estItem.EST_Item_Specification_Size.ToList();
            if (!itemSizes.Any())
            {
                IsValidated = false;
                ErrorMessage = "Size Required"+ Environment .NewLine ;
                return;
            }

            var size = itemSizes.First();
  
            if (IsSizeValidated(size)) return;

               IsValidated = false;
               ErrorMessage = "Size Details Required" + Environment.NewLine;
        }


        //Single Size
        private bool IsSizeValidated(EST_Item_Specification_Size size )
        {
            var b = true;
            if (size.IsWidthMandatory)
            {
                if (MyConvert.ConvertToInteger(size.WidthFeet) ==0 && MyConvert.IsNullString(size.WidthInch))
                {
                    b = false;
                }
            }

            if (size.IsHeightEnabled)
            {
                if (MyConvert.ConvertToInteger(size.WidthFeet ) == 0 && MyConvert.IsNullString(size.WidthInch))
                {
                    b = false;
                }

            }

            if (size.IsThicknessEnabled)
            {
                if (MyConvert.ConvertToInteger(size.HeightFeet) == 0 && MyConvert.IsNullString(size.HeightInch))
                {
                    b = false;
                }
            }

            if (size.IsPcEnabled)
            {
                if (MyConvert.ConvertToDouble(size.Pc) < 1)
                {
                    b = false;
                }
            }
            return b;
        }

    }
}