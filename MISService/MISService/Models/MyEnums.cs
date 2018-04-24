using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISService.Models
{
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
