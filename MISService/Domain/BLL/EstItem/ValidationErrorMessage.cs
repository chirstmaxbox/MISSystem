using System;
using System.Collections.Generic;
using System.Linq;
using MyCommon;

namespace SpecDomain.BLL.EstItem
{

    public class ValidationErrorValue
    {
        //ok value
        //ID=0
        //Text="ok"
        public string GetError(int errorID)
        {
            if (errorID == 0) return "ok";
            var error = GetErrorRepository().FirstOrDefault(x => x.Key == errorID);
            return error == null ? "Unknow Error, Contact IT please." : error.Value ;
        }

        public string GetError(List<int> errorIDs)
        {
            if (!errorIDs.Any()) return "ok";
            string msg = "";
            var errors = GetErrorRepository().ToList();
            foreach (var errorID in errorIDs)
            {
                var error = errors.FirstOrDefault(x => x.Key == errorID);
                msg += error == null ? "Unknow Error, Contact IT please." : error.Value;
                msg += Environment.NewLine;
            }

            return msg;
        }

        private IEnumerable<MyKeyValuePair> GetErrorRepository()
        {
            var errors = new List<MyKeyValuePair>();

            var e0 = new MyKeyValuePair() {Key =  0, Value =  "ok"};

            var e10 = new MyKeyValuePair()
                          {
                              Key =  10,
                              Value =  "Request Failed, Please select a project and try agian. "
                          };
            var e12 = new MyKeyValuePair()
                          {
                              Key =  12,
                              Value = "Request Failed, you are not authorized, please contact HR to get the authorization"
                          };

            var e14 = new MyKeyValuePair()
                          {
                              Key =  15,
                              Value =  "Request Failed, at least one item. "
                          };


            var e50 = new MyKeyValuePair()
                          {
                              Key =  50,
                              Value =  "Request Failed, the item has been locked and can not be deleted."
                          };

            var e101 = new MyKeyValuePair(){Key =  101,Value =  "Request Faild, Install/ship to store or location required "};
            var e102 = new MyKeyValuePair() {Key =  102, Value =  "Request Faild, Bill to Company Required  "};
            var e103 = new MyKeyValuePair() {Key =  103, Value =  "Request Faild, Quote to Company required "};
            var e104 = new MyKeyValuePair() {Key =  104, Value =  "Request Faild, Quote to Contact required "};

            errors.Add(e10);
            errors.Add(e12);
            errors.Add(e14);
            errors.Add(e50);
            errors.Add(e101);
            errors.Add(e102);
            errors.Add(e103);
            errors.Add(e104);


            //Create Project
            var e1002 = new MyKeyValuePair() {Key =  1002, Value =  "You had selected a sub project, sub project can not have sub project"};
            errors.Add(e1002);

            //Estimation
            var e1101 = new MyKeyValuePair()
                            {
                                Key =  1101,
                                Value =  "Request Failed, The Estimation has been locked, Please Copy to a new Version. "
                            };
            var e1102 = new MyKeyValuePair()
                            {
                                Key =  1102,
                                Value = "Request Failed, The Estimation has not been estimated, please fill out the reason you don't need it. "
                            };
            var e1103 = new MyKeyValuePair()
                            {
                                Key =  1103,
                                Value =  "Request Failed, The Estimation has been locked and can not be deleted. "
                            };

            var e1104 = new MyKeyValuePair()
                            {
                                Key =  1104,
                                Value =  "Request Failed, More details information required for Estimation, Please See the Warning ICON. "
                            };


			var e1105 = new MyKeyValuePair()
			{
				Key =  1105,
				Value =  "Request Failed, Estimation exist, a Project Can only have One Estimation. "
			};

			var e1106 = new MyKeyValuePair()
			{
				Key =  1106,
				Value =  "Request Failed, This Command Only Apply to Sub-Project."
			};

			var e1107 = new MyKeyValuePair()
			{
				Key =  1107,
				Value =  "Request Failed, Estimator Is Working on this Project, Can not be Submitted."
			};

			//
			var e1108 = new MyKeyValuePair()
			{
				Key =  1108,
				Value =  "Request Failed, At Least One Active Item Required.  Active: Satatus=New / Contents Changed and Purpose=For Estimation "
			};


            errors.Add(e1101);
            errors.Add(e1102);
            errors.Add(e1103);
            errors.Add(e1104);
        	errors.Add(e1105);
			errors.Add(e1106);
			errors.Add(e1107);
        	errors.Add(e1108);


            return errors;

        }

    }

    public enum NValidationErrorKey
    {
        OK = 0,
        SessionExpired = 10,
        UnAuthorization = 12,
        AtLeastOneItem = 15,
        Delete = 50,

        InstallTo = 101,
        BillTo = 102,
        QuoteTo = 103,
        QouteToContact = 104,

        SubProjectCannotHaveSubProject = 1002,
        EstimationLockedNoNewItem = 1101,
        EstimationLockedCannotBeDeleted = 1103,
        ProductionNotBeEstimated = 1102,
        ProductItemDetailsRequired = 1104,
        ProjectCanOnlyHaveOneEstimation = 1105,
        SubProjectCreateNewEstimation = 1106,
        EstimationLockedCannotBeSubmited = 1107,
        EstimationAtLeastOneActiveItemRequired = 1108,
        

        QuoteChangeFromEstimation = 1301,
        QuoteCannotFindContract = 1302,



        EstItemIsPreviousEstimationAvaliable = 1110,
        EstItemQtyRequired = 1111,
        EstItemPositionRequired = 1112,
        EstItemSizeRequired = 1113,
        EstItemSpecificFieldsError = 1114,


    }

}

