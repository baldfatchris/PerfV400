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
    
    public partial class vPerformanceArtist_RoleCount
    {
        public int PerformanceArtist_ArtistId { get; set; }
        public int PerformanceArtist_RoleId { get; set; }
        public Nullable<int> PerformanceCount { get; set; }
    
        public virtual Artist Artist { get; set; }
        public virtual Role Role { get; set; }
    }
}
