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
    
    public partial class ReviewPerformanceArtist
    {
        public int ReviewPerformanceArtist_Id { get; set; }
        public int ReviewPerformanceArtist_ReviewId { get; set; }
        public int ReviewPerformanceArtist_PerformanceArtistId { get; set; }
        public string ReviewPerformanceArtist_Comment { get; set; }
        public decimal ReviewPerformanceArtist_Rating { get; set; }
    
        public virtual PerformanceArtist PerformanceArtist { get; set; }
        public virtual Review Review { get; set; }
    }
}
