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
    
    public partial class User
    {
        public User()
        {
            this.ArtistUsers = new HashSet<ArtistUser>();
            this.Groups = new HashSet<Group>();
            this.EventUsers = new HashSet<EventUser>();
            this.webpages_OAuthMembership = new HashSet<webpages_OAuthMembership>();
            this.Comments = new HashSet<Comment>();
        }
    
        public int User_Id { get; set; }
        public Nullable<bool> User_Active { get; set; }
        public string User_FirstName { get; set; }
        public string User_LastName { get; set; }
        public Nullable<System.DateTime> User_DateOfBirth { get; set; }
        public string User_Email { get; set; }
        public string User_ConfirmEmail { get; set; }
        public byte[] User_Photo { get; set; }
        public Nullable<int> User_LanguageId { get; set; }
        public Nullable<int> User_ArtistId { get; set; }
        public Nullable<int> User_CountryId { get; set; }
        public Nullable<int> User_FacebookId { get; set; }
        public string User_Name { get; set; }
        public string User_MiddleNames { get; set; }
    
        public virtual Artist Artist { get; set; }
        public virtual ICollection<ArtistUser> ArtistUsers { get; set; }
        public virtual CountryRegion CountryRegion { get; set; }
        public virtual Language Language { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
        public virtual ICollection<EventUser> EventUsers { get; set; }
        public virtual ICollection<webpages_OAuthMembership> webpages_OAuthMembership { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
