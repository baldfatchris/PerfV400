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

    public partial class Artist
    {
        public Artist()
        {
            this.ArtistBands = new HashSet<ArtistBand>();
            this.ArtistMedias = new HashSet<ArtistMedia>();
            this.ArtistTypes = new HashSet<ArtistType>();
            this.ArtistUsers = new HashSet<ArtistUser>();
            this.PerformanceArtists = new HashSet<PerformanceArtist>();
            this.PieceArtists = new HashSet<PieceArtist>();
            this.ProductionArtists = new HashSet<ProductionArtist>();
            this.Users = new HashSet<User>();
            this.vPerformanceArtist_RoleCount = new HashSet<vPerformanceArtist_RoleCount>();
            this.ArtistComments = new HashSet<ArtistComment>();
        }

        public int Artist_Id { get; set; }

        [Display(Name = "First Name")]
        public string Artist_FirstName { get; set; }

        [Display(Name = "Middle Names")]
        public string Artist_Middle_Names { get; set; }

        [Display(Name = "Last Name")]
        [Required]
        public string Artist_LastName { get; set; }

        [Display(Name = "Type")]
        public Nullable<int> Artist_TypeId { get; set; }

        [Display(Name = "Notes")]
        public string Artist_Notes { get; set; }

        public byte[] Artist_Photo { get; set; }
        public string Artist_PhotoFileName { get; set; }
        public string Artist_PhotoMimeType { get; set; }
        public Nullable<int> Artist_UserId { get; set; }

        [Display(Name = "Agent")]
        public Nullable<int> Artist_AgentId { get; set; }

        [Display(Name = "Web Site")]
        public string Artist_WebPage { get; set; }

        [Display(Name = "Born")]
        public Nullable<System.DateTime> Artist_Born { get; set; }

        [Display(Name = "Died")]
        public Nullable<System.DateTime> Artist_Died { get; set; }

        [Display(Name = "Email")]
        public string Artist_Email { get; set; }

        [Display(Name = "Phone")]
        public string Artist_Phone { get; set; }

        [Display(Name = "Country")]
        public Nullable<int> Artist_CountryId { get; set; }

        [Display(Name = "State/Province")]
        public Nullable<int> Artist_StateProvinceId { get; set; }

        [Display(Name = "City")]
        public string Artist_City { get; set; }

        [Display(Name = "Twitter")]
        public string Artist_TwitterName { get; set; }

        [Display(Name = "Facebook")]
        public Nullable<int> Artist_FacebookId { get; set; }

        public Nullable<int> Artist_CreatedBy { get; set; }
        public Nullable<System.DateTime> Artist_CreatedDate { get; set; }

        [Display(Name = "Wiki")]
        public string Artist_Wiki { get; set; }

    
        public virtual Agent Agent { get; set; }
        public virtual CountryRegion CountryRegion { get; set; }
        public virtual StateProvince StateProvince { get; set; }
        public virtual Type Type { get; set; }
        public virtual ICollection<ArtistBand> ArtistBands { get; set; }
        public virtual ICollection<ArtistMedia> ArtistMedias { get; set; }
        public virtual ICollection<ArtistType> ArtistTypes { get; set; }
        public virtual ICollection<ArtistUser> ArtistUsers { get; set; }
        public virtual ICollection<PerformanceArtist> PerformanceArtists { get; set; }
        public virtual ICollection<PieceArtist> PieceArtists { get; set; }
        public virtual ICollection<ProductionArtist> ProductionArtists { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<vPerformanceArtist_RoleCount> vPerformanceArtist_RoleCount { get; set; }
        public virtual ICollection<ArtistComment> ArtistComments { get; set; }
    }
}
