﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace agskeys.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class agsfinancialsEntities : DbContext
    {
        public agsfinancialsEntities()
            : base("name=agsfinancialsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<admin_table> admin_table { get; set; }
        public virtual DbSet<vendor_table> vendor_table { get; set; }
        public virtual DbSet<emp_category_table> emp_category_table { get; set; }
        public virtual DbSet<proof_table> proof_table { get; set; }
        public virtual DbSet<bank_table> bank_table { get; set; }
        public virtual DbSet<customer_profile_table> customer_profile_table { get; set; }
        public virtual DbSet<loan_table> loan_table { get; set; }
        public virtual DbSet<loantype_table> loantype_table { get; set; }
        public virtual DbSet<loan_track_table> loan_track_table { get; set; }
        public virtual DbSet<vendor_track_table> vendor_track_table { get; set; }
        public virtual DbSet<assigned_table> assigned_table { get; set; }
        public virtual DbSet<proof_customer_table> proof_customer_table { get; set; }
        public virtual DbSet<external_comment_table> external_comment_table { get; set; }
        public virtual DbSet<process_executive> process_executive { get; set; }
        public virtual DbSet<notification_table> notification_table { get; set; }
    }
}
