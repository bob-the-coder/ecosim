﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataLayer
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
    
        public virtual DbSet<DecisionChance> DecisionChances { get; set; }
        public virtual DbSet<Decision> Decisions { get; set; }
        public virtual DbSet<Need> Needs { get; set; }
        public virtual DbSet<Node> Nodes { get; set; }
        public virtual DbSet<Production> Productions { get; set; }
        public virtual DbSet<Simulation> Simulations { get; set; }
        public virtual DbSet<SimulationLog> SimulationLogs { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<NodeLink> NodeLinks { get; set; }
    }
}