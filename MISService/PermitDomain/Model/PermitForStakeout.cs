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
    public partial class PermitForStakeout
    {
        public int AppID { get; set; }
        public int BaseAppID { get; set; }
        public string DeptOfHoles { get; set; }
        public string WayofPointLocation { get; set; }
    
        public virtual PermitBase PermitBase { get; set; }
    }
    
}