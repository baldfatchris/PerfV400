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
    
    public partial class VenueUser
    {
        public int VenueUser_Id { get; set; }
        public int VenueUser_VenueId { get; set; }
        public int VenueUser_UserId { get; set; }
        public string VenueUser_Note { get; set; }
        public Nullable<System.DateTime> VenueUser_DateTime { get; set; }
        public Nullable<decimal> VenueUser_UserRating { get; set; }
    
        public virtual Venue Venue { get; set; }
    }
}