using System.Collections.Generic;
using System.Linq;
using MyCommon;

namespace SalesCenterDomain.BDL
{
    /// <summary>
    /// Error Messages Base, The ID should be Less than 1000
    /// </summary>
    public abstract class ValidationErrorMessage
    {
        //ok value
        //ID=0
        //Text="ok"

        private readonly int _errorID;

        protected ValidationErrorMessage(int errorID)
        {
            Errors = GetErrorRepository();
            _errorID = errorID;
        }

        public List<MyKeyValuePair> Errors { get; set; }

        public string ErrorMessage
        {
            get { return GetError(); }
        }


        private string GetError()
        {
            if (_errorID == 0) return "ok";
            MyKeyValuePair error = Errors.FirstOrDefault(x => x.Key == _errorID);
            return error == null ? "Unknow Error, Contact IT please." : error.Value;
        }


        private List<MyKeyValuePair> GetErrorRepository()
        {
            var e0 = new MyKeyValuePair {Key = 0, Value = "ok"};

            var e10 = new MyKeyValuePair
                          {
                              Key = 10,
                              Value = "Request Failed, Please select a project and try agian. "
                          };
            var e12 = new MyKeyValuePair
                          {
                              Key = 12,
                              Value =
                                  "Request Failed, you are not authorized, please contact HR to get the authorization"
                          };

            var e14 = new MyKeyValuePair
                          {
                              Key = 15,
                              Value = "Request Failed, at least one item. "
                          };


            var e50 = new MyKeyValuePair
                          {
                              Key = 50,
                              Value = "Request Failed, the item has been locked and can not be deleted."
                          };

            var e101 = new MyKeyValuePair
                           {Key = 101, Value = "Request Faild, Install/ship to store or location required "};
            var e102 = new MyKeyValuePair {Key = 102, Value = "Request Faild, Bill to Company Required  "};
            var e103 = new MyKeyValuePair {Key = 103, Value = "Request Faild, Quote to Company required "};
            var e104 = new MyKeyValuePair {Key = 104, Value = "Request Faild, Quote to Contact required "};


            return new List<MyKeyValuePair> {e10, e12, e14, e50, e101, e102, e103, e104};
        }
    }


    public class ValidationErrorMessageProject : ValidationErrorMessage
    {
        public ValidationErrorMessageProject(int errorID)
            : base(errorID)
        {
            //Create Project
            var e1002 = new MyKeyValuePair
                            {Key = 1002, Value = "You had selected a sub project, sub project can not have sub project"};
            Errors.Add(e1002);

            //Estimation
            var e1101 = new MyKeyValuePair
                            {
                                Key = 1101,
                                Value = "Request Failed, The Estimation has been locked, Please Copy to a new Version. "
                            };
            var e1102 = new MyKeyValuePair
                            {
                                Key = 1102,
                                Value =
                                    "Request Failed, The Estimation has not been estimated, please fill out the reason you don't need it. "
                            };
            var e1103 = new MyKeyValuePair
                            {
                                Key = 1103,
                                Value = "Request Failed, The Estimation has been locked and can not be deleted. "
                            };

            var e1104 = new MyKeyValuePair
                            {
                                Key = 1104,
                                Value =
                                    "Request Failed, More details information required for Estimation, Please See the Warning ICON. "
                            };


            var e1105 = new MyKeyValuePair
                            {
                                Key = 1105,
                                Value = "Request Failed, Estimation exist, a Project Can only have One Estimation. "
                            };

            var e1106 = new MyKeyValuePair
                            {
                                Key = 1106,
                                Value = "Request Failed, This Command Only Apply to Sub-Project."
                            };

            var e1107 = new MyKeyValuePair
                            {
                                Key = 1107,
                                Value = "Request Failed, Estimator Is Working on this Project, Can not be Submitted."
                            };

            //
            var e1108 = new MyKeyValuePair
                            {
                                Key = 1108,
                                Value =
                                    "Request Failed, At Least One Active Item Required.  Active: Satatus=New / Contents Changed and Purpose=For Estimation "
                            };


            Errors.Add(e1101);
            Errors.Add(e1102);
            Errors.Add(e1103);
            Errors.Add(e1104);
            Errors.Add(e1105);
            Errors.Add(e1106);
            Errors.Add(e1107);
            Errors.Add(e1108);


            //Quotation
            var e1301 = new MyKeyValuePair
                            {
                                Key = 1301,
                                Value = "Request Failed, Please fill out why you change quote from estimation."
                            };
            var e1302 = new MyKeyValuePair
                            {
                                Key = 1302,
                                Value =
                                    "Request Failed, Can not find Contract, Please go to the Quotation and Set it as Final"
                            };

            Errors.Add(e1301);
            Errors.Add(e1302);

            //Workorder(int errorID)
            var e1501 = new MyKeyValuePair
                            {
                                Key = 1501,
                                Value =
                                    "Request Failed, Can not copy Submitted Workorder to a new Revision, you can delete the request or ask authorized personnel to unlock the Workorder."
                            };
            var e1502 = new MyKeyValuePair
                            {Key = 1502, Value = "Request Failed, Only Approved Work order can be copied to Revise."};
            var e1503 = new MyKeyValuePair
                            {Key = 1503, Value = "Request Failed, Only Approved Workorder can be copied to ReDo."};
            var e1504 = new MyKeyValuePair
                            {
                                Key = 1504,
                                Value = "Request Failed, Work order has been  locked, no more item can be added."
                            };
            var e1505 = new MyKeyValuePair {Key = 1505, Value = "No Work Order Found."};
            var e1506 = new MyKeyValuePair {Key = 1506, Value = "Please fill out ---Reason for Add New Items--- first"};


            Errors.Add(e1501);
            Errors.Add(e1502);
            Errors.Add(e1503);
            Errors.Add(e1504);
            Errors.Add(e1505);
            Errors.Add(e1506);

            //Invoice(int errorID)
            var e1601 = new MyKeyValuePair
                            {
                                Key = 1601,
                                Value =
                                    "Delete faild, this invoice is in status of Prepared or Approved, can not be deleted"
                            };

            var e1602 = new MyKeyValuePair
                            {
                                Key = 1602,
                                Value =
                                    "Request failed, Product Item amount fields should be numeric or  number with prefix $, please check and try again."
                            };

            var e1603 = new MyKeyValuePair
                            {
                                Key = 1603,
                                Value =
                                    "Request failed, Service amount fields should be numeric or  number with prefix $, please check and try again."
                            };


            var e1604 = new MyKeyValuePair
                            {
                                Key = 1604,
                                Value =
                                    "Request failed, Only Prepared Invoice Can be Approved, please check and try again."
                            };
            Errors.Add(e1601);
            Errors.Add(e1602);
            Errors.Add(e1603);
            Errors.Add(e1604);
        }
    }
}