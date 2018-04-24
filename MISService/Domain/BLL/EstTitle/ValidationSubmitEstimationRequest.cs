using System;
using System.Linq;
using MyCommon;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.EstTitle
{
    public class ValidationSubmitEstimationRequest:IValidation
    {
        public bool IsValidated { get;  set; }
        public string ErrorMessage { get;  set; }

        private readonly Sales_JobMasterList_EstRev _value;
        private readonly int _estRevID;
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

		public ValidationSubmitEstimationRequest(int estRevID)
		{
			_estRevID = estRevID;
            _value= _db.Sales_JobMasterList_EstRev.Find(estRevID);

            //Validation
		    ErrorMessage = "";

		    ErrorMessage+=IsLocked();
            ErrorMessage +=IsThereInstallOrShipTo();
            ErrorMessage +=IsThereActiveItem();

		    IsValidated =ErrorMessage.Length ==0;
		}

        //EstRev
        private string  IsLocked()
        {
            if (!_value.erLocked) return "";
            return "Request Failed, This Estimation has been locked." +Environment.NewLine ;
        }

        private string  IsThereInstallOrShipTo()
        {
            var cp = new SpecProjectCompany(_value.JobID);

            if (cp.IsThereAnInstallToCompany) return "";
            return "Request Failed, Installation Address Required." + Environment.NewLine;
        }

        //Is There Item
        private string IsThereActiveItem()
        {
            var existingItems = _db.EST_Item.Where(x => 
                                                    x.EstRevID == _estRevID & 
                                                    (x.StatusID == (int)NEstItemStatus.New |
                                                     x.StatusID == (int)NEstItemStatus.ContentsChanged)  &
                                                    x.ItemPurposeID == (int)NEstItemPurpose.ForEstimation
                                                    ).ToList();
     

            if (!existingItems.Any()) return "Request Failed, At Least One Active Item Required"+Environment .NewLine  ;
            
            var msg = "";
            foreach (var estItem in existingItems)
            {
                if (estItem.IsValidated) continue;
                msg += estItem.ErrorMessage + Environment.NewLine;
            }
            return msg;

        }

    }
}
