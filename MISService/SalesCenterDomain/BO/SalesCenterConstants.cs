using System;

namespace SalesCenterDomain.BO
{
    public class SalesCenterConstants
    {
        //Jan 3, 15101
        public const int BEGIN_PROJECT_STARTER_SEED = 0;
        public const int BEGIN_WORKORDER_NUMBER = 0;
        public const int BEGIN_INVOICE_NUMBER = 0;

        public const int BEGIN_PROFORMA_INVOICE_NUMBER_BASE = 0;
        public const int BEGIN_CUSTOMER_ID = 1000;
        public const int BEGIN_CUSTOMER_CONTACT_ID = 1000;
        public const int BEGIN_PROJECT_ID = 100;

        public const int DATABASE_TABLE_IDENTITY_SEED = 100;

        public const int BEGIN_INVOICE_ID = 1000;

        public const string MAIN_CURRENCY = "CAD";

        public const int INVOICE_WORKORDER_MAX_NUMBER = 10;
        public const string PrefixWorkorder = "J";
        public const string PrefixProformaInvoice = "PRO";
        public const string PrefixInvoice = "V";


        public const int MinCharactersOfReason = 8;

        public const int NOT_APPLICABLE = -1;
        public const double ServiceJobDefaultHour = 8;

        public const double ServiceJobDefaultHourWorkshop = 4;

        public const int StringLengthOfReason = 10;
        public const double TaxGstRate = 0.05;
        public const double TaxPstRate = 0.08;


        public const int TOTAL_RECORD_COUNT_PER_YEAR = 5000;
        public const int WORKING_DATE_COUNT_PER_BIWEEK = 12;
        //wSelect
        public const int WorkshopScheduleSelect = 1;
        public const int WorkorderItemAddReasonLength = 10;
        public const string WORKORDER_APPROVAL_REQUEST_TEXT = "Workorder approval";
        public const int WORKORDER_VALIDATION_ITEM_COUNT_MAX = 50;

        public const int WORKORDER_LAST_PROCEDURE_DEPARTMENT = 4;

        public const int DEFAULT_TERRITORY = 1;
        public const int RESPONSE_GENERATETASK_JOBORDERDEADLINE_MONTH = -3;
        public const int RESPONSE_GENERATETASK_DEFAULT_JOBID = 1157;

        public const int RESPONSE_GENERATETASK_DEFAULT_WORKORDERID = 2436;

        public const int WIP_FINISHED_STATUS = 90;


        public const int OTHER_ITEM = 65531;
        public const int PRODUCT_CATEGORY_OTHER = 121;
        public static DateTime NULL_DATE = new DateTime(1900, 1, 1);
    }
}