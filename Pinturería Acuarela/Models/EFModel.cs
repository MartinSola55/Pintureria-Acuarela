using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Pinturería_Acuarela
{
    public partial class EFModel : DbContext
    {
        public EFModel()
            : base("name=EFModel")
        {
        }

        public virtual DbSet<Brand> Brand { get; set; }
        public virtual DbSet<Business> Business { get; set; }
        public virtual DbSet<Capacity> Capacity { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Color> Color { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Product_Business> Product_Business { get; set; }
        public virtual DbSet<Product_Order> Product_Order { get; set; }
        public virtual DbSet<Rol> Rol { get; set; }
        public virtual DbSet<Subcategory> Subcategory { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brand>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Brand>()
                .HasMany(e => e.Product)
                .WithRequired(e => e.Brand)
                .HasForeignKey(e => e.id_brand)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Business>()
                .Property(e => e.adress)
                .IsUnicode(false);

            modelBuilder.Entity<Business>()
                .HasMany(e => e.Product_Business)
                .WithRequired(e => e.Business)
                .HasForeignKey(e => e.id_business)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Business>()
                .HasMany(e => e.User)
                .WithOptional(e => e.Business)
                .HasForeignKey(e => e.id_business);

            modelBuilder.Entity<Capacity>()
                .HasMany(e => e.Product)
                .WithOptional(e => e.Capacity)
                .HasForeignKey(e => e.id_capacity);

            modelBuilder.Entity<Category>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<Category>()
                .HasMany(e => e.Product)
                .WithOptional(e => e.Category)
                .HasForeignKey(e => e.id_category);

            modelBuilder.Entity<Color>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Color>()
                .Property(e => e.rgb_hex_code)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Color>()
                .HasMany(e => e.Product)
                .WithOptional(e => e.Color)
                .HasForeignKey(e => e.id_color);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.Product_Order)
                .WithRequired(e => e.Order)
                .HasForeignKey(e => e.id_order)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Product_Business)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.id_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Product_Order)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.id_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Rol>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<Rol>()
                .HasMany(e => e.User)
                .WithRequired(e => e.Rol)
                .HasForeignKey(e => e.id_rol)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Subcategory>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<Subcategory>()
                .HasMany(e => e.Product)
                .WithOptional(e => e.Subcategory)
                .HasForeignKey(e => e.id_subcategory);

            modelBuilder.Entity<User>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.password)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Order)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.id_user)
                .WillCascadeOnDelete(false);
        }
    }
}
