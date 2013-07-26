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
    
    public partial class Piece
    {
        public Piece()
        {
            this.Performances = new HashSet<Performance>();
            this.PieceArtists = new HashSet<PieceArtist>();
            this.Productions = new HashSet<Production>();
            this.Roles = new HashSet<Role>();
            this.PieceComments = new HashSet<PieceComment>();
        }
    
        public int Piece_Id { get; set; }
        public string Piece_Name { get; set; }
        public Nullable<int> Piece_Year { get; set; }
        public string Piece_Description { get; set; }
        public byte[] Piece_Photo { get; set; }
        public string Piece_PhotoFileName { get; set; }
        public string Piece_PhotoMimeType { get; set; }
        public Nullable<int> Piece_GenreId { get; set; }
        public Nullable<int> Piece_CreatedBy { get; set; }
        public Nullable<System.DateTime> Piece_CreatedDate { get; set; }
        public string Piece_Wiki { get; set; }
    
        public virtual Genre Genre { get; set; }
        public virtual ICollection<Performance> Performances { get; set; }
        public virtual ICollection<PieceArtist> PieceArtists { get; set; }
        public virtual ICollection<Production> Productions { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<PieceComment> PieceComments { get; set; }
    }
}
