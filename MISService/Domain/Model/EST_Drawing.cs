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
    
    public partial class EST_Drawing
    {
        public long DrawingID { get; set; }
        public int ParentID { get; set; }
        public string DrawingType { get; set; }
        public short DrawingPurpose { get; set; }
        public string DrawingName { get; set; }
        public string DrawingHyperlink { get; set; }
        public bool IsFinalDrawing { get; set; }
        public string Note { get; set; }
    
        public virtual Sales_JobMasterList_EstRev Sales_JobMasterList_EstRev { get; set; }
    }
}
