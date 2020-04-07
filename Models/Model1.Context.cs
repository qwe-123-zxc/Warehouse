﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class WarehouseEntities : DbContext
    {
        public WarehouseEntities()
            : base("name=WarehouseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<BadReport> BadReport { get; set; }
        public virtual DbSet<BadReportType> BadReportType { get; set; }
        public virtual DbSet<CheckStock> CheckStock { get; set; }
        public virtual DbSet<CheckStockType> CheckStockType { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Depart> Depart { get; set; }
        public virtual DbSet<Function> Function { get; set; }
        public virtual DbSet<InStorage> InStorage { get; set; }
        public virtual DbSet<InStorageType> InStorageType { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<LocationType> LocationType { get; set; }
        public virtual DbSet<Measure> Measure { get; set; }
        public virtual DbSet<MoveReport> MoveReport { get; set; }
        public virtual DbSet<MoveReportType> MoveReportType { get; set; }
        public virtual DbSet<OutStorage> OutStorage { get; set; }
        public virtual DbSet<OutStorageType> OutStorageType { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<ReturnOrderStock> ReturnOrderStock { get; set; }
        public virtual DbSet<ReturnOrderType> ReturnOrderType { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RolePower> RolePower { get; set; }
        public virtual DbSet<Storage> Storage { get; set; }
        public virtual DbSet<Supplier> Supplier { get; set; }
        public virtual DbSet<SupplierType> SupplierType { get; set; }
    }
}
