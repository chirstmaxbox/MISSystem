using System;
using System.Data;
using MyCommon.MyEnum;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Quote
{
    public class QuoteTitleProperty
    {
        private readonly int _myID;


        private int _jobID;

        public QuoteTitleProperty(int quoteRevID)
        {
            _myID = quoteRevID;
        }

        public int JobID
        {
            get
            {
                DataRow row = QuoteDataRow.GetQuoteTitleDataRow(_myID);
                if (row != null)
                {
                    _jobID = Convert.ToInt32(row["jobID"]);
                }

                return _jobID;
            }
        }


        public bool IsContracted
        {
            get
            {
                bool b = false;
                DataRow row = QuoteDataRow.GetQuoteTitleDataRow(_myID);
                if (row != null)
                {
                    if (row["ContractNumber"] != null)
                    {
                        string s = Convert.ToString(row["ContractNumber"]);
                        if (s.Length > 3)
                        {
                            b = true;
                        }
                    }
                }
                return b;
            }
        }

        public int QuoteStatus
        {
            get
            {
                DataRow row = QuoteDataRow.GetQuoteTitleDataRow(_myID);
                var quoteStatus = (int) NJobStatus.qProcessing;
                if (row != null)
                {
                    quoteStatus = Convert.ToInt32(row["quoteStatus"]);
                }
                return quoteStatus;
            }
        }


        public int PrintOption
        {
            get
            {
                DataRow row = QuoteDataRow.GetQuoteTitleDataRow(_myID);
                var printOption = (int) NQuotePrintOption.DetailsOnly;
                if (row != null)
                {
                    printOption = Convert.ToInt32(row["PrintOption"]);
                }
                return printOption;
            }
        }


        public int EstRevID
        {
            get
            {
                DataRow row = QuoteDataRow.GetQuoteTitleDataRow(_myID);
                int i = 0;
                if (row != null)
                {
                    i = Convert.ToInt32(row["estRevID"]);
                }
                return i;
            }

            set
            {
                var f = new FieldUpdate(_myID);
                f.UpdateEstRevID(value);
            }
        }


        public string ReasonOfAddNewProduct
        {
            get
            {
                DataRow row = QuoteDataRow.GetQuoteTitleDataRowOriginal(_myID);
                string s = "";
                if (row != null)
                {
                    s = Convert.ToString(row["ReasonOfAddProduct"]);
                }
                return s;
            }

            set
            {
                var f = new FieldUpdate(_myID);
                f.UpdateAddNewItemReason(value);
            }
        }

        public double QuoteAmount
        {
            set
            {
                var f = new FieldUpdate(_myID);
                f.UpdateQuoteAmount(value);
            }
        }

        public DateTime ContractDate
        {
            set
            {
                var f = new FieldUpdate(_myID);
                f.UpdateContractDate(value);
            }
        }


        public int TaxOption
        {
            set
            {
                var f = new FieldUpdate(_myID);
                f.UpdateTaxOption(value);
            }
        }


        public string ContractFileName
        {
            set
            {
                var f = new FieldUpdate(_myID);
                f.UpdateContractFileName(value);
            }
        }


        //public bool IsLost
        //{
        //    set
        //    {
        //        var f = new FieldUpdate(_myID);
        //        f.UpdateIsLost(value, JobID);
        //    }
        //}

        //public bool IsFinished
        //{
        //    get
        //    {
        //        var b = false;
        //        var row = QuoteDataRow.GetQuoteTitleDataRowOriginal(_myID);
        //        if (row != null)
        //        {
        //            b = Convert.ToBoolean(row["IsFinished"]);
        //        }

        //        return b;
        //    }

        //    set
        //    {
        //        var f = new FieldUpdate(_myID);
        //        f.UpdateIsFinished(value);
        //    }
        //}

        //public int OrderConformationPrinted 
        //{
        //    set
        //    {
        //        var f = new FieldUpdate(_myID);
        //        f.UpdateOrderConfirmationPrinted(value);
        //    }
        //}  
    }
}