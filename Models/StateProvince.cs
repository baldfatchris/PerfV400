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
    
    public partial class StateProvince
    {
        public StateProvince()
        {
            this.Artists = new HashSet<Artist>();
            this.Bands = new HashSet<Band>();
            this.Venues = new HashSet<Venue>();
        }
    
        public int StateProvince_Id { get; set; }
        public Nullable<int> StateProvince_CountryRegionId { get; set; }
        public string StateProvince_Code { get; set; }
        public bool StateProvince_IsOnlyStateProvinceFlag { get; set; }
        public string StateProvince_Name { get; set; }
    
        public virtual ICollection<Artist> Artists { get; set; }
        public virtual ICollection<Band> Bands { get; set; }
        public virtual CountryRegion CountryRegion { get; set; }
        public virtual ICollection<Venue> Venues { get; set; }
    }
}
