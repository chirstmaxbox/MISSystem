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
    
    public partial class EST_Item_Specification_Template
    {
        public int TemplateID { get; set; }
        public int ProductID { get; set; }
        public int OrderNumber { get; set; }
        public string Title { get; set; }
        public int OptionGroupID { get; set; }
        public int DefaultValue { get; set; }
        public bool IsMandatory { get; set; }
        public string DefaultValueString { get; set; }
        public string OptionGroupName { get; set; }
    
        public virtual EST_Item_Specification_Template_OptionGroup EST_Item_Specification_Template_OptionGroup { get; set; }
        public virtual Product Product { get; set; }
    }
}
