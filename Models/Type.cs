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

    using System.ComponentModel.DataAnnotations;

    public partial class Type
    {
        public Type()
        {
            this.Artists = new HashSet<Artist>();
            this.ArtistTypes = new HashSet<ArtistType>();
            this.Roles = new HashSet<Role>();
        }

        public int Type_Id { get; set; }

        [Display(Name = "Type")]
        public string Type_Name { get; set; }

        public virtual ICollection<Artist> Artists { get; set; }
        public virtual ICollection<ArtistType> ArtistTypes { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}
