//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CustomerDomain.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Customer_Activity
    {
        public int ActivityID { get; set; }
        public int ROWID { get; set; }
        public int CategoryID { get; set; }
        public int AE { get; set; }
        public System.DateTime Date { get; set; }
        public string Contents { get; set; }
    
        public virtual CUSTOMER CUSTOMER { get; set; }
    }
}
