//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PerfV400.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BandType
    {
        public BandType()
        {
            this.Bands = new HashSet<Band>();
        }
    
        public int BandType_Id { get; set; }
        public string BandType_Name { get; set; }
    
        public virtual ICollection<Band> Bands { get; set; }
    }
}