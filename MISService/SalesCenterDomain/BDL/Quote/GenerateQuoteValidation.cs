using SalesCenterDomain.BDL.Project;
using SalesCenterDomain.BO;
using SpecDomain.BLL.EstTitle;

namespace SalesCenterDomain.BDL.Quote
{
    public class GenerateQuoteValidation
    {
        private readonly int _estRevID;
        private readonly int _jobID;

        public GenerateQuoteValidation(int jobID, int estRevID)
        {
            _jobID = jobID;
            _estRevID = estRevID;
        }

        public int ValidationErrorID
        {
            get { return GetValidationErrorID(); }
        }

        private int GetValidationErrorID()
        {
            //Quote To
            var est = new MyEstRev(_estRevID);
            var cp = new ProjectCompany(_jobID);

            if (!cp.isThereAQuotetoCompany) return (int) NValidationErrorValue.QuoteTo;
            if (cp.QuoteToContactID < 1000) return (int) NValidationErrorValue.QouteToContact;
            if (!est.IsThisEstimationHaveChildren()) return (int) NValidationErrorValue.AtLeastOneItem;
            if (!est.IsThisEstimationHasBenSubmited()) return 1102; //Should be locked or have a reason

            return 0;
        }
    }
}