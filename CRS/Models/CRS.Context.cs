﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CRS.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CRSEntities : DbContext
    {
        public CRSEntities()
            : base("name=CRSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CodeSubmission> CodeSubmissions { get; set; }
        public virtual DbSet<CodeSuggestion> CodeSuggestions { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}
