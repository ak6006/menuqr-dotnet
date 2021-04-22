﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EgyptMenu.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<address> addresses { get; set; }
        public virtual DbSet<city> cities { get; set; }
        public virtual DbSet<extra> extras { get; set; }
        public virtual DbSet<hour> hours { get; set; }
        public virtual DbSet<item> items { get; set; }
        public virtual DbSet<language> languages { get; set; }
        public virtual DbSet<migration> migrations { get; set; }
        public virtual DbSet<model_has_permissions> model_has_permissions { get; set; }
        public virtual DbSet<model_has_roles> model_has_roles { get; set; }
        public virtual DbSet<notification> notifications { get; set; }
        public virtual DbSet<option> options { get; set; }
        public virtual DbSet<order_has_items> order_has_items { get; set; }
        public virtual DbSet<order_has_status> order_has_status { get; set; }
        public virtual DbSet<order> orders { get; set; }
        public virtual DbSet<page> pages { get; set; }
        public virtual DbSet<payment> payments { get; set; }
        public virtual DbSet<permission> permissions { get; set; }
        public virtual DbSet<plan> plans { get; set; }
        public virtual DbSet<rating> ratings { get; set; }
        public virtual DbSet<role> roles { get; set; }
        public virtual DbSet<setting> settings { get; set; }
        public virtual DbSet<sms_verifications> sms_verifications { get; set; }
        public virtual DbSet<status> status { get; set; }
        public virtual DbSet<translation> translations { get; set; }
        public virtual DbSet<user> users { get; set; }
        public virtual DbSet<variant> variants { get; set; }
        public virtual DbSet<variants_has_extras> variants_has_extras { get; set; }
        public virtual DbSet<password_resets> password_resets { get; set; }
        public virtual DbSet<options_details> options_details { get; set; }
        public virtual DbSet<variant_has_option> variant_has_option { get; set; }
        public virtual DbSet<category> categories { get; set; }
        public virtual DbSet<restorant> restorants { get; set; }
        public virtual DbSet<theme> themes { get; set; }
        public virtual DbSet<phone_book> phone_book { get; set; }
    }
}
