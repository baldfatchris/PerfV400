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
    
    public partial class Role
    {
        public Role()
        {
            this.PerformanceArtists = new HashSet<PerformanceArtist>();
            this.vPerformanceArtist_RoleCount = new HashSet<vPerformanceArtist_RoleCount>();
            this.RoleComments = new HashSet<RoleComment>();
        }

        public int Role_Id { get; set; }
        public Nullable<int> Role_PieceId { get; set; }
        public Nullable<int> Role_GenreId { get; set; }

        [Display(Name = "Role")]
        public string Role_Name { get; set; }

        [Display(Name = "Notes")]
        public string Role_Notes { get; set; }

        [Display(Name = "Type")]
        public Nullable<int> Role_TypeId { get; set; }

        [Display(Name = "Rank")]
        public Nullable<int> Role_Rank { get; set; }
    
        public virtual Genre Genre { get; set; }
        public virtual ICollection<PerformanceArtist> PerformanceArtists { get; set; }
        public virtual Piece Piece { get; set; }
        public virtual Type Type { get; set; }
        public virtual ICollection<vPerformanceArtist_RoleCount> vPerformanceArtist_RoleCount { get; set; }
        public virtual ICollection<RoleComment> RoleComments { get; set; }
    }
}
