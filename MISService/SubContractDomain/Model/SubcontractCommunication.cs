//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace SubContractDomain.Model
{
    public partial class SubcontractCommunication
    {
        public int CommunicationID { get; set; }
        public int SubcontractID { get; set; }
        public int PostBy { get; set; }
        public Nullable<System.DateTime> PostAt { get; set; }
        public string PostContents { get; set; }
    
        public virtual SubContract SubContract { get; set; }
    }
    
}