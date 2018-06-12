﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISService.Models
{

    public class TableName
    {
        public static string Sales_JobMasterList = "Sales_JobMasterList";
        public static string Customer = "Customer";
        public static string Customer_Contact = "CUSTOMER_CONTACT";
        public static string Sales_JobMasterList_Customer = "Sales_JobMasterList_Customer";
        public static string EST_Item = "EST_Item";
        public static string Est_Service = "Est_Service";
        public static string Sales_Dispatching_DrawingRequisition_EstimationItem = "Sales_Dispatching_DrawingRequisition_EstimationItem";
        public static string Sales_JobMasterList_quoteRev = "Sales_JobMasterList_quoteRev";
        public static string Quote_Item = "Quote_Item";
        public static string FW_Quote_Service = "FW_QUOTE_SERVICE";
    }

    public class SalesType
    {
        public const string Repeat = "Repeat";
        public const string New = "New";
        public const string Bid = "Bid";
    }

    public class BiddingType
    {
        public const string Soft_Bid = "Soft Bid";
        public const string Hard_Bid = "Hard Bid";
    }

    public class BiddingSource
    {
        public const string BiddingGo = "BiddingGo";
        public const string Merx = "Merx";
        public const string GC = "G.C.";
        public const string Government = "Government";
        public const string Developer = "Developer";
        public const string Others = "Others";
    }

    public class YesNo
    {
        public const string Yes = "Yes";
        public const string No = "No";
    }

    public class ItemPosition
    {
        public const string Indoor = "Indoor";
        public const string Outdoor = "Outdoor";
    }

    public class DrawingType
    {
        public const string Graphic = "Graphic";
        public const string Structure = "Structure";
    }

    public class DrawingPurpose
    {
        public const string Estimation_Drawing = "Estimation Drawing";
        public const string Permit_Drawing = "Permit Drawing";
        public const string Workorder_Drawing = "Work-Order Drawing";
        public const string Concept_Design = "Concept Design";
    }

    public class BusinessObjectEN
    {
        public enum NJobStatus
        {
            ProjectNew = 101,
            EstimationNew = 151,
        }
    }

    public class LeadENumber
    {
        public enum NStage : int
        {
            Null = 0, //choose
            All = 1,
            New = 10,
            Introduction = 21,
            Presentation = 31,
            PreQualifiedApproval = 32,

            UnQualifiedByMarketing = 45,
            QuoteRequest = 51, //Quote Request
            UnQualifiedBySales = 81,

            ProjectCreated = 101,
            RequisitionSubmited = 151
        }

        public enum NWaringOfAeAssignedDay : int
        {
            Alert = 1,
            Waring = 2,
        }

        public enum NHistoryEvent : int
        {
            Created = 10,
            UnQualifiedByMarketing = 45,
            QuoteRequest = 51,
            AEAssigned = 52,
            UnQualifiedBySales = 81,
            ProjectCreated = 101,
            EstimationRequisitionSubmited = 151
        }


    }
}
