//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SpecDomain.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class EST_Cost_Type_Category
    {
        public EST_Cost_Type_Category()
        {
            this.EST_Cost_Type = new HashSet<EST_Cost_Type>();
        }
    
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryAbbr { get; set; }
    
        public virtual ICollection<EST_Cost_Type> EST_Cost_Type { get; set; }
    }
}
