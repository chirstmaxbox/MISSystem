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
    
    public partial class EST_Item_Specification_Size
    {
        public long EstItemSizeID { get; set; }
        public long EstItemID { get; set; }
        public bool IsWidthEnabled { get; set; }
        public bool IsWidthMandatory { get; set; }
        public Nullable<int> WidthFeet { get; set; }
        public string WidthInch { get; set; }
        public bool IsHeightEnabled { get; set; }
        public bool IsHeightMandatory { get; set; }
        public Nullable<int> HeightFeet { get; set; }
        public string HeightInch { get; set; }
        public bool IsThicknessEnabled { get; set; }
        public bool IsThicknessMandatory { get; set; }
        public Nullable<int> ThicknessFeet { get; set; }
        public string ThicknessInch { get; set; }
        public bool IsPcEnabled { get; set; }
        public bool IsPcMandatory { get; set; }
        public int Pc { get; set; }
        public bool IsValidated { get; set; }
        public string ErrorMessage { get; set; }
        public Nullable<double> Height { get; set; }
        public Nullable<double> Width { get; set; }
        public Nullable<double> Thickness { get; set; }
    
        public virtual EST_Item EST_Item { get; set; }
    }
}
