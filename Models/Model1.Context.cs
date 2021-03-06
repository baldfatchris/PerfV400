﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PerfV100Entities : DbContext
    {
        public PerfV100Entities()
            : base("name=PerfV100Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<ArtistBand> ArtistBands { get; set; }
        public DbSet<ArtistMedia> ArtistMedias { get; set; }
        public DbSet<ArtistMediaType> ArtistMediaTypes { get; set; }
        public DbSet<ArtistType> ArtistTypes { get; set; }
        public DbSet<ArtistUser> ArtistUsers { get; set; }
        public DbSet<Band> Bands { get; set; }
        public DbSet<BandType> BandTypes { get; set; }
        public DbSet<BandUser> BandUsers { get; set; }
        public DbSet<CountryRegion> CountryRegions { get; set; }
        public DbSet<Default> Defaults { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventUser> EventUsers { get; set; }
        public DbSet<EventUserStatus> EventUserStatus { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Performance> Performances { get; set; }
        public DbSet<PerformanceArtist> PerformanceArtists { get; set; }
        public DbSet<PerformanceArtistUser> PerformanceArtistUsers { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Piece> Pieces { get; set; }
        public DbSet<PieceArtist> PieceArtists { get; set; }
        public DbSet<PieceArtistType> PieceArtistTypes { get; set; }
        public DbSet<Production> Productions { get; set; }
        public DbSet<ProductionArtist> ProductionArtists { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewPerformance> ReviewPerformances { get; set; }
        public DbSet<ReviewPerformanceArtist> ReviewPerformanceArtists { get; set; }
        public DbSet<ReviewProduction> ReviewProductions { get; set; }
        public DbSet<ReviewVenue> ReviewVenues { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<StateProvince> StateProvinces { get; set; }
        public DbSet<Type> Types { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserFriend> UserFriends { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<VenueUser> VenueUsers { get; set; }
        public DbSet<webpages_Membership> webpages_Membership { get; set; }
        public DbSet<webpages_OAuthMembership> webpages_OAuthMembership { get; set; }
        public DbSet<webpages_Roles> webpages_Roles { get; set; }
        public DbSet<vPerformanceArtist_RoleCount> vPerformanceArtist_RoleCount { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<AgentComment> AgentComments { get; set; }
        public DbSet<AgentPhoto> AgentPhotoes { get; set; }
        public DbSet<ArtistComment> ArtistComments { get; set; }
        public DbSet<ArtistPhoto> ArtistPhotoes { get; set; }
        public DbSet<BandComment> BandComments { get; set; }
        public DbSet<BandPhoto> BandPhotoes { get; set; }
        public DbSet<EventComment> EventComments { get; set; }
        public DbSet<EventPhoto> EventPhotoes { get; set; }
        public DbSet<PerformanceComment> PerformanceComments { get; set; }
        public DbSet<PerformancePhoto> PerformancePhotoes { get; set; }
        public DbSet<PieceComment> PieceComments { get; set; }
        public DbSet<PiecePhoto> PiecePhotoes { get; set; }
        public DbSet<ProductionPhoto> ProductionPhotoes { get; set; }
        public DbSet<ReviewComment> ReviewComments { get; set; }
        public DbSet<ReviewPhoto> ReviewPhotoes { get; set; }
        public DbSet<RoleComment> RoleComments { get; set; }
        public DbSet<RolePhoto> RolePhotoes { get; set; }
        public DbSet<VenueComment> VenueComments { get; set; }
        public DbSet<VenuePhoto> VenuePhotoes { get; set; }
        public DbSet<PhotoComment> PhotoComments { get; set; }
        public DbSet<ProductionComment> ProductionComments { get; set; }
    }
}
