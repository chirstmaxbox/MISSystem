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

namespace PermitDomain.Model
{
    public partial class PermitStatu
    {
        public PermitStatu()
        {
            this.PermitBases = new HashSet<PermitBase>();
        }
    
        public int StatusID { get; set; }
        public string StatusName { get; set; }
    
        public virtual ICollection<PermitBase> PermitBases { get; set; }
    }
    
}
