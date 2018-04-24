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
    
    public partial class MaterialPrice
    {
        public int PriceID { get; set; }
        public int MaterialID { get; set; }
        public int UnitID { get; set; }
        public Nullable<double> Price { get; set; }
        public int InvoicePriceUnitID { get; set; }
        public Nullable<double> InvoicePrice { get; set; }
        public Nullable<System.DateTime> InputDate { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public int VenderID { get; set; }
        public bool Active { get; set; }
    
        public virtual Material Material { get; set; }
        public virtual MaterialPriceUnit MaterialPriceUnit { get; set; }
    }
}
