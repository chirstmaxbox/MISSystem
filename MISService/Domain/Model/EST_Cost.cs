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
    
    public partial class EST_Cost
    {
        public int CostItemID { get; set; }
        public long EstItemID { get; set; }
        public int TypeID { get; set; }
        public double OrderNumber { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public double UnitPrice { get; set; }
        public double Qty { get; set; }
        public double SubTotal { get; set; }
        public int DbItemID { get; set; }
        public int WoID { get; set; }
    
        public virtual EST_Cost_Type EST_Cost_Type { get; set; }
        public virtual EST_Item EST_Item { get; set; }
    }
}
