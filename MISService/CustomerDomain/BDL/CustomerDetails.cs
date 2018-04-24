using System;
using System.Data;
using CustomerDomain.BO;

using MyCommon;

namespace CustomerDomain.BDL
{
    public class CustomerDetails 
    {

        private readonly DataRow _row;
        public CustomerDetails(int rowID)
       
        {

            var cf = new CustomerDataRow(rowID);
            _row = cf.CustomerRow;

        }


        public int CompanyType
        {
            get { return GetCustomerSelectType(); }
        }

        public bool IsThisCompanyACustomer
        {
            get { return GetIsThisCompanyACustomer(); }
        }


        public int TermDeposit
        {
            get { return GetTermDeposit(); }
        }

        public int TermID
        {
            get { return GetTermID(); }
        }


        public string Currency
        {
            get { return GetCurrency(); }
        }


        public int ReferByEmployee
        {
            get { return GetReferByEmployee(); }
        }

        



        private int GetCustomerSelectType()
        {
            int companyType = (int)CustomerEN.NCustomerSelectType.c1IndividualStore;
            if (_row != null)
            {
                if (Convert.ToInt32( _row["rowID"]) > (int)CustomerDomainConstants.BEGIN_CUSTOMER_ID)
                {
                    companyType =Convert.ToInt32 ( _row["selectType"]);
                }
            }
            return companyType;
        }

        private bool GetIsThisCompanyACustomer()
        {
            bool b = true;
            if (_row != null)
            {
                if (Convert.IsDBNull(_row["FirstContractDate"]))
                {
                    b = false;
                }
                else if (!MyConvert.IsDate(_row["FirstContractDate"]))
                {
                    b = false;
                }
            }

            return b;
        }

        private int GetTermDeposit()
        {
            int termDeposit = 50;
            if (!string.IsNullOrEmpty(_row["depositRequired"].ToString()) & !Convert.IsDBNull(_row["depositRequired"]))
            {
                termDeposit =Convert .ToInt32(  _row["depositRequired"]);
            }
            return termDeposit;
        }

        private int GetTermID()
        {
            int termID = 0;
            //COD
            if (!Convert.IsDBNull(_row["termID"]))
            {
                if (Convert.ToInt32(_row["termID"]) < 1000)
                {
                    termID = MyConvert.ConvertToInteger(_row["termID"]);
                }
            }
            return termID;
        }


        private string GetCurrency()
        {
            string currency = "CAD";
            if (!string.IsNullOrEmpty(_row["CURRENCY_ID"].ToString()) & !Convert.IsDBNull(_row["CURRENCY_ID"]))
            {
                if (Convert.ToString(_row["CURRENCY_ID"]) == "USD")
                {
                    currency = "USD";
                }
            }
            return currency;
        }


        private int GetReferByEmployee()
        {
            int i = 0;
            if (!Convert.IsDBNull(_row["ReferByEmployee"]))
            {
                i =Convert.ToInt32(  _row["ReferByEmployee"]);
            }
            return i;
        }

        public string GetCompanyName()
        {
            string s = "";
            if (_row != null)
            {
                if (!Convert.IsDBNull(_row["NAME"]))
                {
                    s = Convert.ToString(_row["NAME"]);
                }
            }

            return s;
        }


        public string ZipCode
        {

            get { return GetZipcode(); }
        }

        public string Intersection
        {

            get { return GetIntersection(); }
        }


        private string GetZipcode()
        {
            string s = "";
            if (_row != null)
            {
                if (!Convert.IsDBNull(_row["zipcode"]))
                {
                    s =Convert .ToString(  _row["zipcode"]);
                }
            }

            return s;
        }

        private string GetIntersection()
        {
            string s = "";
            if (_row != null)
            {
                if (!Convert.IsDBNull(_row["Intersection"]))
                {
                    s =Convert.ToString(_row["Intersection"]);
                }
            }

            return s;
        }


        public string Address
        {

            get { return GetAddress(); }
        }

        private string GetAddress()
        {
            string s = "";
            if (_row != null)
            {
                if (!Convert.IsDBNull(_row["Addr_1"]))
                {
                    s =Convert .ToString(  _row["Addr_1"]);
                }
            }

            return s;
        }



        public string City
        {

            get { return GetCity(); }
        }

        private string GetCity()
        {
            string s = "";
            if (_row != null)
            {
                if (!Convert.IsDBNull(_row["City"]))
                {
                    s =Convert .ToString(  _row["City"]);
                }
            }

            return s;
        }

        public int AE
        {
            get
            {
                int id = 0;
                if (_row != null)
                {
                    id = MyConvert.ConvertToInteger(_row["SalesID"]);
                }
                return id;
            }
        }

        public int OP
        {
            get
            {

                int id = 0;
                if (_row != null)
                {
                    {
                        id = MyConvert.ConvertToInteger(_row["Sa1ID"]);
                    }

                }
                return id;
            }

        }

        
    }
}