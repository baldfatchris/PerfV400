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
    
    public partial class EventUser
    {
        public int EventUser_Id { get; set; }
        public int EventUser_EventId { get; set; }
        public int EventUser_UserId { get; set; }
        public string EventUser_Note { get; set; }
        public Nullable<System.DateTime> EventUser_DateTime { get; set; }
        public Nullable<int> EventUser_StatusId { get; set; }
        public Nullable<decimal> EventUser_UserRating { get; set; }
    
        public virtual Event Event { get; set; }
        public virtual EventUserStatus EventUserStatus { get; set; }
        public virtual User User { get; set; }
    }
}
